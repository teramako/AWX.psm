using System.Collections.Specialized;
using System.Text.Json.Serialization;

namespace AnsibleTower.Resources
{
    public interface ISystemJob : IUnifiedJob
    {
        [JsonPropertyName("execution_node")]
        string ExecutionNode { get; }

        [JsonPropertyName("system_job_template")]
        ulong SystemJobTemplate { get; }
        [JsonPropertyName("job_type")]
        string JobType { get; }
        [JsonPropertyName("extra_vars")]
        string ExtraVars { get; }
        [JsonPropertyName("result_stdout")]
        string ResultStdout { get; }

    }
    public interface IJobDetail
    {
        [JsonPropertyName("job_args")]
        string JobArgs { get; }
        [JsonPropertyName("job_cwd")]
        string JobCwd { get; }
        [JsonPropertyName("job_env")]
        Dictionary<string, string> JobEnv { get; }
        [JsonPropertyName("result_traceback")]
        string ResultTraceback { get; }
        [JsonPropertyName("event_processing_finished")]
        bool EventProcessingFinished { get; }
    }

    public class SystemJob(ulong id, ResourceType type, string url, RelatedDictionary related,
                           SystemJob.Summary summaryFields, DateTime created, DateTime? modified, string name,
                           string description, ulong unifiedJobTemplate, JobLaunchType launchType, JobStatus status,
                           ulong? executionEnvironment, bool failed, DateTime? started, DateTime? finished,
                           DateTime? canceledOn, double elapsed, string jobExplanation, string executionNode,
                           LaunchedBy launchedBy, string? workUnitId, ulong systemJobTemplate, string jobType,
                           string extraVars, string resultStdout)
        : ISystemJob, IResource<SystemJob.Summary>
    {
        public const string PATH = "/api/v2/system_jobs/";
        public static async Task<Detail> Get(ulong id)
        {
            var apiResult = await RestAPI.GetAsync<Detail>($"{PATH}{id}/");
            return apiResult.Contents;
        }
        public static async IAsyncEnumerable<SystemJob> Find(NameValueCollection? query, bool getAll = false)
        {
            await foreach(var result in RestAPI.GetResultSetAsync<SystemJob>(PATH, query, getAll))
            {
                foreach (var app in result.Contents.Results)
                {
                    yield return app;
                }
            }
        }
        public record Summary(
            [property: JsonPropertyName("execution_environment")] EnvironmentSummary ExecutionEnvironment,
            ScheduleSummary? Schedule,
            [property: JsonPropertyName("unified_job_template")] UnifiedJobTemplateSummary UnifiedJobTemplate,
            [property: JsonPropertyName("instance_group")] InstanceGroupSummary InstanceGroup,
            [property: JsonPropertyName("user_capabilities")] Capability UserCapabilities);

        public class Detail(ulong id, ResourceType type, string url, RelatedDictionary related,
                            Summary summaryFields, DateTime created, DateTime? modified, string name,
                            string description, ulong unifiedJobTemplate, JobLaunchType launchType, JobStatus status,
                            ulong? executionEnvironment, bool failed, DateTime? started, DateTime? finished,
                            DateTime? canceledOn, double elapsed, string jobArgs, string jobCwd,
                            Dictionary<string, string> jobEnv, string jobExplanation, string executionNode,
                            string resultTraceback, bool eventProcessingFinished, LaunchedBy launchedBy,
                            string? workUnitId, ulong systemJobTemplate, string jobType, string extraVars,
                            string resultStdout)
            : ISystemJob, IJobDetail, IResource<Summary>
        {

            public ulong Id { get; } = id;
            public ResourceType Type { get; } = type;
            public string Url { get; } = url;
            public RelatedDictionary Related { get; } = related;
            public Summary SummaryFields { get; } = summaryFields;

            #region UnifiedJob Properties
            public DateTime Created { get; } = created;
            public DateTime? Modified { get; } = modified;
            public string Name { get; } = name;
            public string Description { get; } = description;
            public ulong UnifiedJobTemplate { get; } = unifiedJobTemplate;
            public JobLaunchType LaunchType { get; } = launchType;
            public JobStatus Status { get; } = status;
            public ulong? ExecutionEnvironment { get; } = executionEnvironment;
            public bool Failed { get; } = failed;
            public DateTime? Started { get; } = started;
            public DateTime? Finished { get; } = finished;
            public DateTime? CanceledOn { get; } = canceledOn;
            public double Elapsed { get; } = elapsed;
            public string JobArgs { get; } = jobArgs;
            public string JobCwd { get; } = jobCwd;
            public Dictionary<string, string> JobEnv { get; } = jobEnv;
            public string JobExplanation { get; } = jobExplanation;
            public string ExecutionNode { get; } = executionNode;
            public string ResultTraceback { get; } = resultTraceback;
            public bool EventProcessingFinished { get; } = eventProcessingFinished;
            public LaunchedBy LaunchedBy { get; } = launchedBy;
            public string? WorkUnitId { get; } = workUnitId;
            #endregion

            public ulong SystemJobTemplate { get; } = systemJobTemplate;
            public string JobType { get; } = jobType;
            public string ExtraVars { get; } = extraVars;
            public string ResultStdout { get; } = resultStdout;
        }

        public ulong Id { get; } = id;
        public ResourceType Type { get; } = type;
        public string Url { get; } = url;
        public RelatedDictionary Related { get; } = related;
        public Summary SummaryFields { get; } = summaryFields;

        #region UnifiedJob Properties
        public DateTime Created { get; } = created;
        public DateTime? Modified { get; } = modified;
        public string Name { get; } = name;
        public string Description { get; } = description;
        public ulong UnifiedJobTemplate { get; } = unifiedJobTemplate;
        public JobLaunchType LaunchType { get; } = launchType;
        public JobStatus Status { get; } = status;
        public ulong? ExecutionEnvironment { get; } = executionEnvironment;
        public bool Failed { get; } = failed;
        public DateTime? Started { get; } = started;
        public DateTime? Finished { get; } = finished;
        public DateTime? CanceledOn { get; } = canceledOn;
        public double Elapsed { get; } = elapsed;
        public string JobExplanation { get; } = jobExplanation;
        public string ExecutionNode { get; } = executionNode;
        public LaunchedBy LaunchedBy { get; } = launchedBy;
        public string? WorkUnitId { get; } = workUnitId;
        #endregion

        public ulong SystemJobTemplate { get; } = systemJobTemplate;
        public string JobType { get; } = jobType;
        public string ExtraVars { get; } = extraVars;
        public string ResultStdout { get; } = resultStdout;
    }
}

