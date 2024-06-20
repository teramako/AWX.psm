using System.Collections.Specialized;
using System.Text.Json.Serialization;

namespace AWX.Resources
{
    public class InventoryUpdateJobEvent(ulong id, ResourceType type, string url, RelatedDictionary related,
                                         InventoryUpdateJobEvent.Summary summaryFields, DateTime created,
                                         DateTime? modified, JobEventEvent @event, int counter, string eventDisplay,
                                         OrderedDictionary eventData, bool failed, bool changed, string uUID,
                                         string stdout, int startLine, int endLine, JobVerbosity verbosity,
                                         ulong inventoryUpdate)
        : IJobEventBase, IResource<InventoryUpdateJobEvent.Summary>
    {
        /// <summary>
        /// List Inventory Update Events for an Inventory Update.<br/>
        /// API Path: <c>/api/v2/inventory_updates/<paramref name="inventoryUpdateJobId"/>/events/</c>
        /// </summary>
        /// <param name="inventoryUpdateJobId"></param>
        /// <param name="query"></param>
        /// <param name="getAll"></param>
        /// <returns></returns>
        public static async IAsyncEnumerable<InventoryUpdateJobEvent> FindFromInventoryUpdateJob(ulong inventoryUpdateJobId,
                                                                                  NameValueCollection? query = null,
                                                                                  bool getAll = false)
        {
            var path = $"{InventoryUpdateJob.PATH}{inventoryUpdateJobId}/events/";
            await foreach (var result in RestAPI.GetResultSetAsync<InventoryUpdateJobEvent>(path, query, getAll))
            {
                foreach (var jobEvent in result.Contents.Results)
                {
                    yield return jobEvent;
                }
            }
        }

        public record Summary();

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
        public OrderedDictionary EventData { get; } = eventData;
        public bool Failed { get; } = failed;
        public bool Changed { get; } = changed;
        public string UUID { get; } = uUID;
        public string Stdout { get; } = stdout;
        public int StartLine { get; } = startLine;
        public int EndLine { get; } = endLine;
        public JobVerbosity Verbosity { get; } = verbosity;
        [JsonPropertyName("inventory_update")]
        public ulong InventoryUpdate { get; } = inventoryUpdate;
    }
}
