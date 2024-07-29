using System.Text.Json.Serialization;

namespace AWX.Resources
{
    public class HostMetric(ulong id, string hostname, string url, DateTime? firstAutomation, DateTime? lastAutomation,
                             DateTime? lastDeleted, int automatedCounter, int deletedCounter, bool deleted,
                             int? usedInInventories)
    {
        public const string PATH = "/api/v2/host_metrics/";

        public ulong Id { get; } = id;
        public string Hostname { get; } = hostname;
        public string Url { get; } = url;
        [JsonPropertyName("first_automation")]
        public DateTime? FirstAutomation { get; } = firstAutomation;
        [JsonPropertyName("last_automation")]
        public DateTime? LastAutomation { get; } = lastAutomation;
        [JsonPropertyName("last_deleted")]
        public DateTime? LastDeleted { get; } = lastDeleted;
        [JsonPropertyName("automated_counter")]
        public int AutomatedCounter { get; } = automatedCounter;
        [JsonPropertyName("deleted_counter")]
        public int DeletedCounter { get; } = deletedCounter;
        public bool Deleted { get; } = deleted;
        [JsonPropertyName("used_in_inventories")]
        public int? UsedInInventories { get; } = usedInInventories;
    }
}
