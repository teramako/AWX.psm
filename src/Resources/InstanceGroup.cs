using System.Collections.Specialized;
using System.Text.Json.Serialization;

namespace AWX.Resources
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
        /// <summary>
        /// Retrieve an Instance Group.<br/>
        /// API Path: <c>api/v2/instance_groups/<paramref name="id"/>/</c>
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static async Task<InstanceGroup> Get(ulong id)
        {
            var apiResult = await RestAPI.GetAsync<InstanceGroup>($"{PATH}{id}/");
            return apiResult.Contents;
        }
        /// <summary>
        /// List Instance Groups.<br/>
        /// API Path: <c>/api/v2/instance_groups/</c>
        /// </summary>
        /// <param name="query"></param>
        /// <param name="getAll"></param>
        /// <returns></returns>
        public static async IAsyncEnumerable<InstanceGroup> Find(NameValueCollection? query, bool getAll = false)
        {
            await foreach (var result in RestAPI.GetResultSetAsync<InstanceGroup>(PATH, query, getAll))
            {
                foreach (var instanceGroup in result.Contents.Results)
                {
                    yield return instanceGroup;
                }
            }
        }
        /// <summary>
        /// List Instance Groups for an Instance.<br/>
        /// API Path: <c>/api/v2/instance/<paramref name="instanceId"/>/instance_groups/</c>
        /// </summary>
        /// <param name="instanceId"></param>
        /// <param name="query"></param>
        /// <param name="getAll"></param>
        /// <returns></returns>
        public static async IAsyncEnumerable<InstanceGroup> FindFromInstance(ulong instanceId,
                                                                             NameValueCollection? query = null,
                                                                             bool getAll = false)
        {
            var path = $"{Instance.PATH}{instanceId}/instance_groups/";
            await foreach (var result in RestAPI.GetResultSetAsync<InstanceGroup>(path, query, getAll))
            {
                foreach (var instanceGroup in result.Contents.Results)
                {
                    yield return instanceGroup;
                }
            }
        }
        /// <summary>
        /// List Instace Groups for an Organization.<br/>
        /// API Path: <c>/api/v2/organizations/<paramref name="organizationId"/>/instance_groups/</c>
        /// </summary>
        /// <param name="organizationId"></param>
        /// <param name="query"></param>
        /// <param name="getAll"></param>
        /// <returns></returns>
        public static async IAsyncEnumerable<InstanceGroup> FindFromOrganization(ulong organizationId,
                                                                                 NameValueCollection? query = null,
                                                                                 bool getAll = false)
        {
            var path = $"{Organization.PATH}{organizationId}/instance_groups/";
            await foreach (var result in RestAPI.GetResultSetAsync<InstanceGroup>(path, query, getAll))
            {
                foreach (var instanceGroup in result.Contents.Results)
                {
                    yield return instanceGroup;
                }
            }
        }
        /// <summary>
        /// List Instance Groups for an Inventory.<br/>
        /// API Path: <c>/api/v2/inventories/<paramref name="inventoryId"/>/instance_groups/</c>
        /// </summary>
        /// <param name="inventoryId"></param>
        /// <param name="query"></param>
        /// <param name="getAll"></param>
        /// <returns></returns>
        public static async IAsyncEnumerable<InstanceGroup> FindFromInventory(ulong inventoryId,
                                                                              NameValueCollection? query = null,
                                                                              bool getAll = false)
        {
            var path = $"{Inventory.PATH}{inventoryId}/instance_groups/";
            await foreach (var result in RestAPI.GetResultSetAsync<InstanceGroup>(path, query, getAll))
            {
                foreach (var instanceGroup in result.Contents.Results)
                {
                    yield return instanceGroup;
                }
            }
        }
        /// <summary>
        /// List Instance Groups for a Job Template.<br/>
        /// API Path: <c>/api/v2/job_templates/<paramref name="jobTemplateId"/>/instance_groups/</c>
        /// </summary>
        /// <param name="jobTemplateId"></param>
        /// <param name="query"></param>
        /// <param name="getAll"></param>
        /// <returns></returns>
        public static async IAsyncEnumerable<InstanceGroup> FindFromJobTemplate(ulong jobTemplateId,
                                                                                NameValueCollection? query = null,
                                                                                bool getAll = false)
        {
            var path = $"{JobTemplate.PATH}{jobTemplateId}/instance_groups/";
            await foreach (var result in RestAPI.GetResultSetAsync<InstanceGroup>(path, query, getAll))
            {
                foreach (var instanceGroup in result.Contents.Results)
                {
                    yield return instanceGroup;
                }
            }
        }
        /// <summary>
        /// List Instance Groups for a Schedule.<br/>
        /// API Path: <c>/api/v2/schedules/<paramref name="scheduleId"/>/instance_groups/</c>
        /// </summary>
        /// <param name="scheduleId"></param>
        /// <param name="query"></param>
        /// <param name="getAll"></param>
        /// <returns></returns>
        public static async IAsyncEnumerable<InstanceGroup> FindFromSchedule(ulong scheduleId,
                                                                             NameValueCollection? query = null,
                                                                             bool getAll = false)
        {
            var path = $"{Schedule.PATH}{scheduleId}/instance_groups/";
            await foreach (var result in RestAPI.GetResultSetAsync<InstanceGroup>(path, query, getAll))
            {
                foreach (var instanceGroup in result.Contents.Results)
                {
                    yield return instanceGroup;
                }
            }
        }
        /// <summary>
        /// List Instance Groups for a Workflow Job Template Node.<br/>
        /// API Path: <c>/api/v2/workflow_job_template_nodes/<paramref name="wjtnId"/>/instance_groups/</c>
        /// </summary>
        /// <param name="wjtnId">Id of Workflow Job Tempalte Node</param>
        /// <param name="query"></param>
        /// <param name="getAll"></param>
        /// <returns></returns>
        public static async IAsyncEnumerable<InstanceGroup> FindFromWorkflowJobTemplateNode(ulong wjtnId,
                                                                                            NameValueCollection? query = null,
                                                                                            bool getAll = false)
        {
            var path = $"{WorkflowJobTemplateNode.PATH}{wjtnId}/instance_groups/";
            await foreach (var result in RestAPI.GetResultSetAsync<InstanceGroup>(path, query, getAll))
            {
                foreach (var instanceGroup in result.Contents.Results)
                {
                    yield return instanceGroup;
                }
            }
        }
        /// <summary>
        /// List Instance Groups for a Workflow Job Template Node.<br/>
        /// API Path: <c>/api/v2/workflow_job_nodes/<paramref name="wjnId"/>/instance_groups/</c>
        /// </summary>
        /// <param name="wjnId">Id of Workflow Job Tempalte Node</param>
        /// <param name="query"></param>
        /// <param name="getAll"></param>
        /// <returns></returns>
        public static async IAsyncEnumerable<InstanceGroup> FindFromWorkflowJobNode(ulong wjnId,
                                                                                    NameValueCollection? query = null,
                                                                                    bool getAll = false)
        {
            var path = $"{WorkflowJobNode.PATH}{wjnId}/instance_groups/";
            await foreach (var result in RestAPI.GetResultSetAsync<InstanceGroup>(path, query, getAll))
            {
                foreach (var instanceGroup in result.Contents.Results)
                {
                    yield return instanceGroup;
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
