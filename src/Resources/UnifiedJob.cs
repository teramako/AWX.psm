using System.Collections.Specialized;
using System.Text.Json.Serialization;
using System.Web;

namespace AWX.Resources
{
    public interface IUnifiedJob
    {
        ulong Id { get; }
        ResourceType Type { get; }
        string Url { get; }
        DateTime Created { get; }
        DateTime? Modified { get; }
        string Name { get; }
        [JsonPropertyName("launch_type")]
        JobLaunchType LaunchType { get; }
        JobStatus Status { get; }
        [JsonPropertyName("execution_environment")]
        ulong? ExecutionEnvironment { get; }
        bool Failed { get; }
        DateTime? Started { get; }
        DateTime? Finished { get; }
        [JsonPropertyName("canceled_on")]
        DateTime? CanceledOn { get; }
        double Elapsed { get; }
        [JsonPropertyName("job_explanation")]
        string JobExplanation { get; }
        /*
        [JsonPropertyName("execution_node")]
        string ExecutionNode { get; }
        */
        /*
        [JsonPropertyName("controller_node")]
        string ControllerNode { get; }
        */
        [JsonPropertyName("launched_by")]
        LaunchedBy LaunchedBy { get; }
        [JsonPropertyName("work_unit_id")]
        string? WorkUnitId { get; }
    }

    public abstract class UnifiedJob(ulong id,
                                     ResourceType type,
                                     string url,
                                     DateTime created,
                                     DateTime? modified,
                                     string name,
                                     JobLaunchType launchType,
                                     JobStatus status,
                                     ulong? executionEnvironment,
                                     bool failed,
                                     DateTime? started,
                                     DateTime? finished,
                                     DateTime? canceledOn,
                                     double elapsed,
                                     string jobExplanation,
                                     LaunchedBy launchedBy,
                                     string? workUnitId)
        : IUnifiedJob
    {
        public const string PATH = "/api/v2/unified_jobs/";

        public ulong Id { get; } = id;

        public ResourceType Type { get; } = type;
        public string Url { get; } = url;
        public DateTime Created { get; } = created;
        public DateTime? Modified { get; } = modified;
        public string Name { get; } = name;
        public JobLaunchType LaunchType { get; } = launchType;
        public JobStatus Status { get; } = status;
        public ulong? ExecutionEnvironment { get; } = executionEnvironment;
        public bool Failed { get; } = failed;
        public DateTime? Started { get; } = started;
        public DateTime? Finished { get; } = finished;
        public DateTime? CanceledOn { get; } = canceledOn;
        public double Elapsed { get; } = elapsed;
        public string JobExplanation { get; } = jobExplanation;
        public LaunchedBy LaunchedBy { get; } = launchedBy;
        public string? WorkUnitId { get; } = workUnitId;

        /// <summary>
        /// Retrieve a job.
        /// The job is one of:
        /// <list type="bullet">
        /// <item><term><see cref="JobTemplateJob"/></term><description>Type: <c>job</c></description></item>
        /// <item><term><see cref="WorkflowJob"/></term><description>Type: <c>workflow_job</c></description></item>
        /// <item><term><see cref="ProjectUpdateJob"/></term><description>Type: <c>project_update</c></description></item>
        /// <item><term><see cref="InventoryUpdateJob"/></term><description>Type: <c>inventory_update</c></description></item>
        /// <item><term><see cref="SystemJob"/></term><description>Type: <c>sytem_job</c></description></item>
        /// </list>
        /// </summary>
        /// <param name="id">Unified Job ID</param>
        /// <returns></returns>
        public static async Task<IUnifiedJob> Get(ulong id)
        {
            var query = HttpUtility.ParseQueryString($"id={id}&page_size=1");
            var apiResult = await RestAPI.GetAsync<ResultSet>($"{PATH}?{query}");
            return apiResult.Contents.Results.OfType<IUnifiedJob>().Single();
        }
        public static async Task<IUnifiedJob[]> Get(params ulong[] idList)
        {
            if (idList.Length > 200)
            {
                throw new ArgumentException($"too many items: {nameof(idList)} Length must be less than or equal to 200.");
            }
            var query = HttpUtility.ParseQueryString($"id__in={string.Join(',', idList)}&page_size={idList.Length}");
            var apiResult = await RestAPI.GetAsync<ResultSet>($"{PATH}?{query}");
            return apiResult.Contents.Results.OfType<IUnifiedJob>().ToArray();
        }
        /// <summary>
        /// List Unified Jobs.<br/>
        /// API Path: <c>/api/v2/unified_jobs/</c>
        /// </summary>
        /// <param name="query"></param>
        /// <param name="getAll"></param>
        /// <returns></returns>
        public static async IAsyncEnumerable<IUnifiedJob> Find(NameValueCollection? query, bool getAll = false)
        {
            await foreach (var result in RestAPI.GetResultSetAsync(PATH, query, getAll))
            {
                foreach (var obj in result.Contents.Results)
                {
                    if (obj is IUnifiedJob job)
                    {
                        yield return job;
                    }
                }
            }
        }
    }
}

