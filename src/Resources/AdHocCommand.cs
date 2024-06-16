using System.Collections.Specialized;
using System.Text.Json.Serialization;

namespace AWX.Resources
{
    public interface IAdHocCommand : IUnifiedJob
    {
        [JsonPropertyName("execution_node")]
        string ExecutionNode { get; }
        [JsonPropertyName("controller_node")]
        string ControllerNode { get; }

        [JsonPropertyName("job_type")]
        string JobType { get; }
        ulong Inventory { get; }
        string Limit { get; }
        ulong Credential { get; }
        [JsonPropertyName("module_name")]
        string ModuleName { get; }
        [JsonPropertyName("module_args")]
        string ModuleArgs { get; }
        byte Forks { get; }
        JobVerbosity Verbosity { get; }
        [JsonPropertyName("extra_vars")]
        string ExtraVars { get; }
        [JsonPropertyName("become_enabled")]
        bool BecomeEnabled { get; }
        [JsonPropertyName("diff_mode")]
        bool DiffMode { get; }
    }

    public class AdHocCommand(ulong id, ResourceType type, string url, RelatedDictionary related,
                              AdHocCommand.Summary summaryFields, DateTime created, DateTime? modified, string name,
                              JobLaunchType launchType, JobStatus status, ulong? executionEnvironment, bool failed,
                              DateTime? started, DateTime? finished, DateTime? canceledOn, double elapsed,
                              string jobExplanation, LaunchedBy launchedBy, string? workUnitId, string executionNode,
                              string controllerNode, string jobType, ulong inventory, string limit, ulong credential,
                              string moduleName, string moduleArgs, byte forks, JobVerbosity verbosity, string extraVars,
                              bool becomeEnabled, bool diffMode)
        : UnifiedJob(id, type, url, created, modified, name, launchType, status, executionEnvironment, failed,
                     started, finished, canceledOn, elapsed, jobExplanation, launchedBy, workUnitId),
          IAdHocCommand, IResource<AdHocCommand.Summary>
    {
        public new const string PATH = "/api/v2/ad_hoc_commands/";
        public static async Task<Detail> Get(ulong id)
        {
            var apiResult = await RestAPI.GetAsync<Detail>($"{PATH}{id}/");
            return apiResult.Contents;
        }
        public static new async IAsyncEnumerable<AdHocCommand> Find(NameValueCollection? query, bool getAll = false)
        {
            await foreach(var result in RestAPI.GetResultSetAsync<AdHocCommand>(PATH, query, getAll))
            {
                foreach (var job in result.Contents.Results)
                {
                    yield return job;
                }
            }
        }

        public record Summary(
            InventorySummary Inventory,
            [property: JsonPropertyName("execution_environment")] EnvironmentSummary? ExecutionEnvironment,
            CredentialSummary Credential,
            [property: JsonPropertyName("instance_group")] InstanceGroupSummary InstanceGroup,
            [property: JsonPropertyName("created_by")] UserSummary? CreatedBy,
            [property: JsonPropertyName("user_capabilities")] Capability UserCapabilities);

        public class Detail(ulong id, ResourceType type, string url, RelatedDictionary related,
                            AdHocCommand.Summary summaryFields, DateTime created, DateTime? modified, string name,
                            JobLaunchType launchType, JobStatus status, ulong? executionEnvironment, bool failed,
                            DateTime? started, DateTime? finished, DateTime? canceledOn, double elapsed,
                            string jobExplanation, LaunchedBy launchedBy, string? workUnitId, string executionNode,
                            string controllerNode, string jobType, ulong inventory, string limit, ulong credential,
                            string moduleName, string moduleArgs, byte forks, JobVerbosity verbosity, string extraVars,
                            bool becomeEnabled, bool diffMode, string jobArgs, string jobCwd,
                            Dictionary<string, string> jobEnv, string resultTraceback, bool eventProcessingFinished,
                            Dictionary<string, int> hostStatusCounts)
            : AdHocCommand(id, type, url, related, summaryFields, created, modified, name, launchType, status,
                           executionEnvironment, failed, started, finished, canceledOn, elapsed, jobExplanation,
                           launchedBy, workUnitId, executionNode, controllerNode, jobType, inventory, limit, credential,
                           moduleName, moduleArgs, forks, verbosity, extraVars, becomeEnabled, diffMode),
              IAdHocCommand, IJobDetail, IResource<Summary>
        {
            public string JobArgs { get; } = jobArgs;
            public string JobCwd { get; } = jobCwd;
            public Dictionary<string, string> JobEnv { get; } = jobEnv;
            public string ResultTraceback { get; } = resultTraceback;
            public bool EventProcessingFinished { get; } = eventProcessingFinished;
            [JsonPropertyName("host_status_counts")]
            public Dictionary<string, int> HostStatusCounts { get; } = hostStatusCounts;
        }
        public RelatedDictionary Related { get; } = related;
        public Summary SummaryFields { get; } = summaryFields;
        public string ExecutionNode { get; } = executionNode;
        public string ControllerNode { get; } = controllerNode;
        public string JobType { get; } = jobType;
        public ulong Inventory { get; } = inventory;
        public string Limit { get; } = limit;
        public ulong Credential { get; } = credential;
        public string ModuleName { get; } = moduleName;
        public string ModuleArgs { get; } = moduleArgs;
        public byte Forks { get; } = forks;
        public JobVerbosity Verbosity { get; } = verbosity;
        public string ExtraVars { get; } = extraVars;
        public bool BecomeEnabled { get; } = becomeEnabled;
        public bool DiffMode { get; } = diffMode;

    }
}
