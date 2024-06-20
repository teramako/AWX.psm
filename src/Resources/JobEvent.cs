using System.Collections.Specialized;
using System.Reflection;
using System.Text.Json.Serialization;

namespace AWX.Resources
{
    public interface IJobEvent : IJobEventBase
    {
        [JsonPropertyName("event_level")]
        int EventLevel { get; }
        [JsonPropertyName("parent_uuid")]
        string ParentUUID { get; }
        ulong? Host { get; }
        [JsonPropertyName("host_name")]
        string HostName { get; }
        string Playbook { get; }
        string Play { get; }
        string Task { get; }
        string Role { get; }
        ulong Job { get; }
    }

    [JsonConverter(typeof(Json.EnumUpperCamelCaseStringConverter<JobEventEvent>))]
    public enum JobEventEvent
    {
        /// <summary>
        /// Host Failed
        /// </summary>
        RunnerOnFailed,
        /// <summary>
        /// Host Started
        /// </summary>
        RunnerOnStart,
        /// <summary>
        /// Host OK
        /// </summary>
        RunnerOnOK,
        /// <summary>
        /// Host Failure
        /// </summary>
        RunnerOnError,
        /// <summary>
        /// Host Skipped
        /// </summary>
        RunnerOnSkipped,
        /// <summary>
        /// Host Unreachable
        /// </summary>
        RunnerOnUnreachable,
        /// <summary>
        /// No Hosts Remaining
        /// </summary>
        RunnerOnNoHosts,
        /// <summary>
        /// Host Polling
        /// </summary>
        RunnerOnAsyncPoll,
        /// <summary>
        /// Host Async OK
        /// </summary>
        RunnerOnAsyncOK,
        /// <summary>
        /// Host Async Failure
        /// </summary>
        RunnerOnAsyncFailed,
        /// <summary>
        /// Item OK
        /// </summary>
        RunnerItemOnOK,
        /// <summary>
        /// Item Failed
        /// </summary>
        RunnerItemOnFailed,
        /// <summary>
        /// Item Skipped
        /// </summary>
        RunnerItemOnSkipped,
        /// <summary>
        /// Host Retry
        /// </summary>
        RunnerRetry,
        /// <summary>
        /// File Differerence
        /// </summary>
        RunnerOnFileDiff,
        /// <summary>
        /// Playbook Started
        /// </summary>
        PlaybookOnStart,
        /// <summary>
        /// Running Handlers
        /// </summary>
        PlaybookOnNotify,
        /// <summary>
        /// Including File
        /// </summary>
        PlaybookOnInclude,
        /// <summary>
        /// No Hosts Matched
        /// </summary>
        PlaybookOnNoHostsMatched,
        /// <summary>
        /// No Hosts Remaining
        /// </summary>
        PlaybookOnNoHostsRemaining,
        /// <summary>
        /// Task Started
        /// </summary>
        PlaybookOnTaskStart,
        /// <summary>
        /// Variables Prompted
        /// </summary>
        PlaybookOnVarsPrompt,
        /// <summary>
        /// Gathering Facts
        /// </summary>
        PlaybookOnSetup,
        /// <summary>
        /// Internal: on Import for Host
        /// </summary>
        PlaybookOnImportForHost,
        /// <summary>
        /// internal: on Not Import for Host
        /// </summary>
        PlaybookOnNotImportForHost,
        /// <summary>
        /// Play Started
        /// </summary>
        PlaybookOnPlayStart,
        /// <summary>
        /// Playbook Complete
        /// </summary>
        PlaybookOnStats,
        Debug,
        Verbose,
        Deprecated,
        Warnining,
        SystemWarning,
        Error
    }


    public class JobEvent(ulong id, ResourceType type, string url, RelatedDictionary related,
                          JobEvent.Summary summaryFields, DateTime created, DateTime? modified, ulong job,
                          JobEventEvent @event, int counter, string eventDisplay, OrderedDictionary eventData,
                          int eventLevel, bool failed, bool changed, string uuid, string parentUUID, ulong? host,
                          string hostName, string playbook, string play, string task, string role, string stdout,
                          int startLine, int endLine, JobVerbosity verbosity)
                : IJobEvent, IResource<JobEvent.Summary>
    {
        public const string PATH = "/api/v2/job_events/";

        public static async IAsyncEnumerable<JobEvent> Find(IUnifiedJob job, NameValueCollection? query, bool getAll = false)
        {
            var fieldInfo = typeof(ResourceType).GetField($"{job.Type}")
                ??  throw new NullReferenceException("fieldInfo is null");
            var attr = fieldInfo.GetCustomAttribute<ResourcePathAttribute>(false)
                ?? throw new NullReferenceException($"{job.Type} has no {nameof(ResourcePathAttribute)}");;
            var path = $"/api/v2/{attr.PathName}/{job.Id}/job_events/";
            await foreach (var apiResult in RestAPI.GetResultSetAsync<JobEvent>(path, query, getAll))
            {
                foreach (var jobEvent in apiResult.Contents.Results)
                {
                    yield return jobEvent;
                }
            }
        }
        public record Summary(NameDescriptionSummary? Host, JobExSummary Job, OrderedDictionary Role);

        public ulong Id { get; } = id;
        public ResourceType Type { get; } = type;
        public string Url { get; } = url;
        public RelatedDictionary Related { get; } = related;
        public Summary SummaryFields { get; } = summaryFields;

        public DateTime Created { get; } = created;
        public DateTime? Modified { get; } = modified;
        public ulong Job { get; } = job;
        public JobEventEvent Event { get; } = @event;
        public int Counter { get; } = counter;
        public string EventDisplay { get; } = eventDisplay;
        public OrderedDictionary EventData { get; } = eventData;
        public int EventLevel { get; } = eventLevel;
        public bool Failed { get; } = failed;
        public bool Changed { get; } = changed;
        public string UUID { get; } = uuid;
        public string ParentUUID { get; } = parentUUID;
        public ulong? Host { get; } = host;
        public string HostName { get; } = hostName;
        public string Playbook { get; } = playbook;
        public string Play { get; } = play;
        public string Task { get; } = task;
        public string Role { get; } = role;
        public string Stdout { get; } = stdout;
        public int StartLine { get; } = startLine;
        public int EndLine { get; } = endLine;
        public JobVerbosity Verbosity { get; } = verbosity;
    }
}
