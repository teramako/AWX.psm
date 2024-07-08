using System.Collections.Specialized;
using System.Text.Json.Serialization;

namespace AWX.Resources
{
    public interface IProjectUpdateJobEvent : IJobEventBase
    {
        [JsonPropertyName("event_level")]
        int EventLevel { get; }
        [JsonPropertyName("host_name")]
        string HostName { get; }
        string Playbook { get; }
        string Play { get; }
        string Task { get; }
        string Role { get; }
        [JsonPropertyName("project_update")]
        ulong ProjectUpdate { get; }
    }

    public class ProjectUpdateJobEvent(ulong id, ResourceType type, string url, RelatedDictionary related,
                                       ProjectUpdateJobEvent.Summary summaryFields, DateTime created, DateTime? modified,
                                       JobEventEvent @event, int counter, string eventDisplay,
                                       Dictionary<string, object?> eventData, int eventLevel, bool failed, bool changed,
                                       string uuid, string hostName, string playbook, string play, string task,
                                       string role, string stdout, int startLine, int endLine, JobVerbosity verbosity,
                                       ulong projectUpdate)
        : IProjectUpdateJobEvent, IResource<ProjectUpdateJobEvent.Summary>
    {
        /// <summary>
        /// List Project Update Events for a Project Update.<br/>
        /// API Path: <c>/api/v2/project_updates/<paramref name="projectUpdateJobId"/>/events/</c>
        /// </summary>
        /// <param name="projectUpdateJobId"></param>
        /// <param name="query"></param>
        /// <param name="getAll"></param>
        /// <returns></returns>
        public static async IAsyncEnumerable<ProjectUpdateJobEvent> FindFromProjectUpdateJob(ulong projectUpdateJobId,
                                                                                NameValueCollection? query = null,
                                                                                bool getAll = false)
        {
            var path = $"{ProjectUpdateJob.PATH}{projectUpdateJobId}/events/";
            await foreach (var result in RestAPI.GetResultSetAsync<ProjectUpdateJobEvent>(path, query, getAll))
            {
                foreach (var jobEvent in result.Contents.Results)
                {
                    yield return jobEvent;
                }
            }
        }

        public record Summary(
            [property: JsonPropertyName("project_update")] ProjectUpdateSummary ProjectUpdate
            );
        public ulong Id { get; } = id;
        public ResourceType Type { get; } = type;
        public string Url { get; } = url;
        public RelatedDictionary Related { get; } = related;
        public Summary SummaryFields { get; } = summaryFields;
        public DateTime Created { get; } = created;
        public DateTime? Modified { get; } = modified;
        public JobEventEvent Event { get; } = @event;
        public int Counter { get; } = counter;
        public string EventDisplay { get; } = eventDisplay;
        public Dictionary<string, object?> EventData { get; } = eventData;
        public int EventLevel { get; } = eventLevel;
        public bool Failed { get; } = failed;
        public bool Changed { get; } = changed;
        public string UUID { get; } = uuid;
        public string HostName { get; } = hostName;
        public string Playbook { get; } = playbook;
        public string Play { get; } = play;
        public string Task { get; } = task;
        public string Role { get; } = role;
        public string Stdout { get; } = stdout;
        public int StartLine { get; } = startLine;
        public int EndLine { get; } = endLine;
        public JobVerbosity Verbosity { get; } = verbosity;
        public ulong ProjectUpdate { get; } = projectUpdate;
    }
}
