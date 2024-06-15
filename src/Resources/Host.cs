using System.Collections.Specialized;
using System.Text.Json.Serialization;

namespace AWX.Resources
{
    public interface IHost
    {
        /// <summary>
        /// Name of this host.
        /// </summary>
        string Name { get; }
        /// <summary>
        /// Optional description of this host.
        /// </summary>
        string Description { get; }
        /// <summary>
        /// Inventory ID.
        /// </summary>
        ulong Inventory { get; }
        /// <summary>
        /// Is this host online and available for running jobs?
        /// </summary>
        bool Enabled { get; }
        /// <summary>
        /// The value used by the remote inventory source to uniquely identify the host.
        /// </summary>
        [JsonPropertyName("instance_id")]
        string InstanceId { get; }
        /// <summary>
        /// Host variables in JSON or YAML format.
        /// </summary>
        string Variables { get; }
    }

    public class Host(ulong id,
                      ResourceType type,
                      string url,
                      RelatedDictionary related,
                      Host.Summary summaryFields,
                      DateTime created,
                      DateTime? modified,
                      string name,
                      string description,
                      ulong inventory,
                      bool enabled,
                      string instanceId,
                      string variables)
        : IHost, IResource<Host.Summary>
    {
        public const string PATH = "/api/v2/hosts/";

        public static async Task<Host> Get(ulong id)
        {
            var apiResult = await RestAPI.GetAsync<Host>($"{PATH}{id}/");
            return apiResult.Contents;
        }
        public static async IAsyncEnumerable<Host> Find(NameValueCollection? query, bool getAll = false)
        {
            await foreach(var result in RestAPI.GetResultSetAsync<Host>(PATH, query, getAll))
            {
                foreach (var host in result.Contents.Results)
                {
                    yield return host;
                }
            }
        }
        public record Summary(
            InventorySummary Inventory,
            [property: JsonPropertyName("last_job")] JobExSummary? LastJob,
            [property: JsonPropertyName("last_job_host_summary")] LastJobHostSummary? LastJobHostSummary,
            [property: JsonPropertyName("user_capabilities")] Capability UserCapabilities,
            ListSummary<NameSummary> Groups,
            [property: JsonPropertyName("recent_jobs")] HostRecentJobSummary[] RecentJobs);


        public ulong Id { get; } = id;
        public ResourceType Type { get; } = type;
        public string Url { get; } = url;
        public RelatedDictionary Related { get; } = related;
        public Summary SummaryFields { get; } = summaryFields;
        public DateTime Created { get; } = created;
        public DateTime? Modified { get; } = modified;
        public string Name { get; } = name;

        public string Description { get; } = description;

        public ulong Inventory { get; } = inventory;

        public bool Enabled { get; } = enabled;

        public string InstanceId { get; } = instanceId;

        public string Variables { get; } = variables;
    }
}
