using System.Collections.Specialized;
using System.Text.Json.Serialization;

namespace AnsibleTower.Resources
{
    public interface IInstanceGroup
    {
        string Name { get; }
        [JsonPropertyName("max_concurrent_jobs")]
        int MaxConcurrentJobs { get; }
        [JsonPropertyName("max_forks")]
        int MaxForks { get; }
        [JsonPropertyName("is_container_group")]
        bool IsContainerGroup { get; }
        ulong? Credential { get; }
        [JsonPropertyName("policy_instance_percentage")]
        double PolicyInstancePercentage { get; }
        [JsonPropertyName("policy_instance_minimum")]
        int PolicyInstanceMinimum { get; }
        [JsonPropertyName("policy_instance_list")]
        string[] PolicyInstanceList { get; }
        [JsonPropertyName("pod_spec_override")]
        string PodSpecOverride { get; }
    }

    [ResourceType(ResourceType.InstanceGroup)]
    public class InstanceGroup(ulong id,
                               ResourceType type,
                               string url,
                               RelatedDictionary related,
                               InstanceGroup.Summary summaryFields,
                               string name,
                               DateTime created,
                               DateTime? modified,
                               int capacity,
                               int consumedCapacity,
                               double percentCapacityRemaining,
                               int jobsRunning,
                               int maxConcurrentJobs,
                               int maxForks,
                               int jobsTotal,
                               int instances,
                               bool isContainerGroup,
                               ulong? credential,
                               double policyInstancePercentage,
                               int policyInstanceMinimum,
                               string[] policyInstanceList,
                               string podSpecOverride)
        : IInstanceGroup, IResource<InstanceGroup.Summary>
    {
        public const string PATH = "/api/v2/instance_groups/";
        public static async Task<InstanceGroup> Get(ulong id)
        {
            var apiResult = await RestAPI.GetAsync<InstanceGroup>($"{PATH}{id}/");
            return apiResult.Contents;
        }
        public static async IAsyncEnumerable<InstanceGroup> Find(NameValueCollection? query, bool getAll = false)
        {
            await foreach (var result in RestAPI.GetResultSetAsync<InstanceGroup>(PATH, query, getAll))
            {
                foreach (var app in result.Contents.Results)
                {
                    yield return app;
                }
            }
        }
        public record Summary(
            [property: JsonPropertyName("object_roles")] Dictionary<string, NameDescriptionSummary> ObjectRoles,
            [property: JsonPropertyName("user_capabilities")] Capability UserCapabilities);


        public ulong Id { get; } = id;
        public ResourceType Type { get; } = type;
        public string Url { get; } = url;
        public RelatedDictionary Related { get; } = related;
        public Summary SummaryFields { get; } = summaryFields;
        public string Name { get; } = name;
        public DateTime Created { get; } = created;
        public DateTime? Modified { get; } = modified;
        public int Capacity { get; } = capacity;
        [JsonPropertyName("consumed_capacity")]
        public int ConsumedCapacity { get; } = consumedCapacity;
        [JsonPropertyName("percent_capacity_remaining")]
        public double PercentCapacityRemaining { get; } = percentCapacityRemaining;
        [JsonPropertyName("jobs_running")]
        public int JobsRunning { get; } = jobsRunning;
        public int MaxConcurrentJobs { get; } = maxConcurrentJobs;
        public int MaxForks { get; } = maxForks;
        [JsonPropertyName("jobs_total")]
        public int JobsTotal { get; } = jobsTotal;
        public int Instances { get; } = instances;
        public bool IsContainerGroup { get; } = isContainerGroup;
        public ulong? Credential { get; } = credential;
        public double PolicyInstancePercentage { get; } = policyInstancePercentage;
        public int PolicyInstanceMinimum { get; } = policyInstanceMinimum;
        public string[] PolicyInstanceList { get; } = policyInstanceList;
        public string PodSpecOverride { get; } = podSpecOverride;
    }
}
