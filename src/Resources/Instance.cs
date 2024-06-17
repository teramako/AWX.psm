using System.Collections.Specialized;
using System.Text.Json.Serialization;

namespace AWX.Resources
{
    /// <summary>
    /// Role that this node plays in the mesh
    /// </summary>
    [JsonConverter(typeof(Json.EnumUpperCamelCaseStringConverter<InstanceNodeType>))]
    public enum InstanceNodeType
    {
        /// <summary>
        /// Control plane node
        /// </summary>
        Control,
        /// <summary>
        /// Execution plane node
        /// </summary>
        Excecution,
        /// <summary>
        /// Control and execution
        /// </summary>
        Hybrid,
        /// <summary>
        /// Message passing node, no execution capability
        /// </summary>
        Hop
    }

    public interface IInstance
    {
        string Hostname { get; }
        [JsonPropertyName("capacity_adjustment")]
        string CapacityAdjustment { get; }
        bool Enabled { get; }
        [JsonPropertyName("managed_by_policy")]
        bool ManagedByPolicy { get; }
        [JsonPropertyName("node_type")]
        InstanceNodeType NodeType { get; }
        [JsonPropertyName("node_state")]
        string NodeState { get; }
        [JsonPropertyName("listener_port")]
        int ListenerPort { get; }
    }


    public class Instance(ulong id,
                          ResourceType type,
                          string url,
                          RelatedDictionary related,
                          Instance.Summary summaryFields,
                          string hostname,
                          string uuid,
                          DateTime created,
                          DateTime? modified,
                          DateTime lastSeen,
                          DateTime? healthCheckStarted,
                          bool healthCheckPending,
                          DateTime? lastHealthCheck,
                          string errors,
                          string capacityAdjustment,
                          string version,
                          int capacity,
                          int consumedCapacity,
                          double percentCapacityRemaining,
                          int jobsRunning,
                          int jobsTotal,
                          string cpu,
                          ulong memory,
                          int cpuCapacity,
                          int memCapacity,
                          bool enabled,
                          bool managedByPolicy,
                          InstanceNodeType nodeType,
                          string nodeState,
                          string ipAddress,
                          int listenerPort)
        : IInstance, IResource<Instance.Summary>
    {
        public const string PATH = "/api/v2/instances/";
        public static async Task<Instance> Get(ulong id)
        {
            var apiResult = await RestAPI.GetAsync<Instance>($"{PATH}{id}/");
            return apiResult.Contents;
        }
        public static async IAsyncEnumerable<Instance> Find(NameValueCollection? query, bool getAll = false)
        {
            await foreach (var result in RestAPI.GetResultSetAsync<Instance>(PATH, query, getAll))
            {
                foreach (var instance in result.Contents.Results)
                {
                    yield return instance;
                }
            }
        }
        /// <summary>
        /// List instances for an Instance Group<br/>
        /// API Path: <c>/api/v2/instance_groups/<paramref name="instanceGroupId"/>/instances/</c>
        /// </summary>
        /// <param name="instanceGroupId"></param>
        /// <param name="query"></param>
        /// <param name="getAll"></param>
        /// <returns></returns>
        public static async IAsyncEnumerable<Instance> FindFromInstanceGroup(ulong instanceGroupId,
                                                                             NameValueCollection? query = null,
                                                                             bool getAll = false)
        {
            var path = $"{InstanceGroup.PATH}{instanceGroupId}/instances/";
            await foreach (var result in RestAPI.GetResultSetAsync<Instance>(path, query, getAll))
            {
                foreach (var instance in result.Contents.Results)
                {
                    yield return instance;
                }
            }
        }

        public record Summary([property: JsonPropertyName("user_capabilities")] Capability UserCapabilities);

        public ulong Id { get; } = id;
        public ResourceType Type { get; } = type;
        public string Url { get; } = url;
        public RelatedDictionary Related { get; } = related;
        public Summary SummaryFields { get; } = summaryFields;
        public string Hostname { get; } = hostname;
        public string Uuid { get; } = uuid;
        public DateTime Created { get; } = created;
        public DateTime? Modified { get; } = modified;
        [JsonPropertyName("last_seen")]
        public DateTime LastSeen { get; } = lastSeen;
        [JsonPropertyName("health_check_started")]
        public DateTime? HealthCheckStarted { get; } = healthCheckStarted;
        [JsonPropertyName("health_check_pending")]
        public bool HealthCheckPending { get; } = healthCheckPending;
        [JsonPropertyName("last_health_check")]
        public DateTime? LastHealthCheck { get; } = lastHealthCheck;
        public string Errors { get; } = errors;
        public string CapacityAdjustment { get; } = capacityAdjustment;
        public string Version { get; } = version;
        public int Capacity { get; } = capacity;
        [JsonPropertyName("consumed_capacity")]
        public int ConsumedCapacity { get; } = consumedCapacity;
        [JsonPropertyName("percent_capacity_remaining")]
        public double PercentCapacityRemaining { get; } = percentCapacityRemaining;
        [JsonPropertyName("jobs_running")]
        public int JobsRunning { get; } = jobsRunning;
        [JsonPropertyName("jobs_total")]
        public int JobsTotal { get; } = jobsTotal;
        public string Cpu { get; } = cpu;
        public ulong Memory { get; } = memory;
        [JsonPropertyName("cpu_capacity")]
        public int CpuCapacity { get; } = cpuCapacity;
        [JsonPropertyName("mem_capacity")]
        public int MemCapacity { get; } = memCapacity;
        public bool Enabled { get; } = enabled;
        public bool ManagedByPolicy { get; } = managedByPolicy;
        public InstanceNodeType NodeType { get; } = nodeType;
        public string NodeState { get; } = nodeState;
        [JsonPropertyName("ip_address")]
        public string IpAddress { get; } = ipAddress;
        public int ListenerPort { get; } = listenerPort;
    }
}
