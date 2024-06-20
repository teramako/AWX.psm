using System.Collections.Specialized;
using System.Text.Json.Serialization;

namespace AWX.Resources
{
    public interface IAdHocCommandJobEvent : IJobEventBase
    {
        [JsonPropertyName("ad_hoc_command")]
        ulong AdHocCommand { get; }
        ulong? Host { get; }
        [JsonPropertyName("host_name")]
        string HostName { get; }
    }

    public class AdHocCommandJobEvent(ulong id, ResourceType type, string url, RelatedDictionary related,
                                      AdHocCommandJobEvent.Summary summaryFields, DateTime created, DateTime? modified,
                                      ulong adHocCommand, JobEventEvent @event, int counter, string eventDisplay,
                                      OrderedDictionary eventData, bool failed, bool changed, string uuid, ulong? host,
                                      string hostName, string stdout, int startLine, int endLine, JobVerbosity verbosity)
        : IAdHocCommandJobEvent, IResource<AdHocCommandJobEvent.Summary>
    {
        /// <summary>
        /// List Ad Hoc Command Events for an Ad Hoc Command.<br/>
        /// API Path: <c>/api/v2/ad_hoc_commands/<paramref name="adHocCommandId"/>/events/</c>
        /// </summary>
        /// <param name="adHocCommandId"></param>
        /// <param name="query"></param>
        /// <param name="getAll"></param>
        /// <returns></returns>
        public static async IAsyncEnumerable<AdHocCommandJobEvent> FindFromAdHocCommand(ulong adHocCommandId,
                                                                                     NameValueCollection? query = null,
                                                                                     bool getAll = false)
        {
            var path = $"{Resources.AdHocCommand.PATH}{adHocCommandId}/events/";
            await foreach (var result in RestAPI.GetResultSetAsync<AdHocCommandJobEvent>(path, query, getAll))
            {
                foreach (var jobEvent in result.Contents.Results)
                {
                    yield return jobEvent;
                }
            }
        }

        public record Summary(NameDescriptionSummary Host);

        public ulong Id { get; } = id;
        public ResourceType Type { get; } = type;
        public string Url { get; } = url;
        public RelatedDictionary Related { get; } = related;
        public Summary SummaryFields { get; } = summaryFields;
        public DateTime Created { get; } = created;
        public DateTime? Modified { get; } = modified;
        public ulong AdHocCommand { get; } = adHocCommand;
        public JobEventEvent Event { get; } = @event;
        public int Counter { get; } = counter;
        public string EventDisplay { get; } = eventDisplay;
        public OrderedDictionary EventData { get; } = eventData;
        public bool Failed { get; } = failed;
        public bool Changed { get; } = changed;
        public string UUID { get; } = uuid;
        public ulong? Host { get; } = host;
        public string HostName { get; } = hostName;
        public string Stdout { get; } = stdout;
        public int StartLine { get; } = startLine;
        public int EndLine { get; } = endLine;
        public JobVerbosity Verbosity { get; } = verbosity;
    }
}
