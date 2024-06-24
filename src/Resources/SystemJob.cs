using System.Collections.Specialized;
using System.Text.Json.Serialization;

namespace AWX.Resources
{
    public interface ISystemJob : IUnifiedJob
    {
        string Description { get; }
        [JsonPropertyName("unified_job_template")]
        ulong UnifiedJobTemplate { get; }
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

    public class SystemJob(ulong id, ResourceType type, string url, RelatedDictionary related,
                           SystemJob.Summary summaryFields, DateTime created, DateTime? modified, string name,
                           string description, ulong unifiedJobTemplate, JobLaunchType launchType, JobStatus status,
                           ulong? executionEnvironment, bool failed, DateTime? started, DateTime? finished,
                           DateTime? canceledOn, double elapsed, string jobExplanation, string executionNode,
                           LaunchedBy launchedBy, string? workUnitId, ulong systemJobTemplate, string jobType,
                           string extraVars, string resultStdout)
        : UnifiedJob(id, type, url, created, modified, name, launchType, status, executionEnvironment, failed,
                     started, finished, canceledOn, elapsed, jobExplanation, launchedBy, workUnitId),
          ISystemJob, IResource<SystemJob.Summary>
    {
        public new const string PATH = "/api/v2/system_jobs/";
        /// <summary>
        /// Retrieve a System Job Template.<br/>
        /// API Path: <c>/api/v2/system_job_templates/<paramref name="id"/>/</c>
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static async Task<Detail> Get(ulong id)
        {
            var apiResult = await RestAPI.GetAsync<Detail>($"{PATH}{id}/");
            return apiResult.Contents;
        }
        /// <summary>
        /// List System Job Templates.<br/>
        /// API Path: <c>/api/v2/system_job_templates/</c>
        /// </summary>
        /// <param name="query"></param>
        /// <param name="getAll"></param>
        /// <returns></returns>
        public new static async IAsyncEnumerable<SystemJob> Find(NameValueCollection? query, bool getAll = false)
        {
            await foreach(var result in RestAPI.GetResultSetAsync<SystemJob>(PATH, query, getAll))
            {
                foreach (var systemJob in result.Contents.Results)
                {
                    yield return systemJob;
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
            : SystemJob(id, type, url, related, summaryFields, created, modified, name, description, unifiedJobTemplate,
                        launchType, status, executionEnvironment, failed, started, finished, canceledOn, elapsed,
                        jobExplanation, executionNode, launchedBy, workUnitId, systemJobTemplate, jobType, extraVars, resultStdout),
              ISystemJob, IJobDetail, IResource<Summary>
        {

            public string JobArgs { get; } = jobArgs;
            public string JobCwd { get; } = jobCwd;
            public Dictionary<string, string> JobEnv { get; } = jobEnv;
            public string ResultTraceback { get; } = resultTraceback;
            public bool EventProcessingFinished { get; } = eventProcessingFinished;
        }

        public RelatedDictionary Related { get; } = related;
        public Summary SummaryFields { get; } = summaryFields;
        public string Description { get; } = description;
        public ulong UnifiedJobTemplate { get; } = unifiedJobTemplate;

        public string ExecutionNode { get; } = executionNode;

        public ulong SystemJobTemplate { get; } = systemJobTemplate;
        public string JobType { get; } = jobType;
        public string ExtraVars { get; } = extraVars;
        public string ResultStdout { get; } = resultStdout;
    }
}

