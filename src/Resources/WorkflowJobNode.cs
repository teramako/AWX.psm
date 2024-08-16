using System.Collections.Specialized;
using System.Text.Json.Serialization;

namespace AWX.Resources
{
    public interface IWorkflowJobNode
    {
        [JsonPropertyName("extra_data")]
        OrderedDictionary ExtraData { get; }
        /// <summary>
        /// Inventory applied as a prompt, assuming job template for inventory.
        /// </summary>
        ulong? Inventory { get; }
        [JsonPropertyName("scm_branch")]
        string? ScmBranch { get; }
        [JsonPropertyName("job_type")]
        string? JobType { get; }
        [JsonPropertyName("job_tags")]
        string? JobTags { get; }
        [JsonPropertyName("skip_tags")]
        string? SkipTags { get; }
        string? Limit { get; }
        [JsonPropertyName("diff_mode")]
        bool? DiffMode { get; }
        JobVerbosity? Verbosity { get; }
        /// <summary>
        /// The container image to be used for execution.
        /// </summary>
        [JsonPropertyName("execution_environment")]
        ulong? ExecutionEnvironment { get; }
        int? Forks { get; }
        [JsonPropertyName("job_slice_count")]
        int? JobSliceCount { get; }
        int? Timeout { get; }
        ulong? Job { get; }
        [JsonPropertyName("workflow_job")]
        ulong WorkflowJob { get; }
        [JsonPropertyName("unified_job_template")]
        ulong UnifiedJobTemplate { get; }
        [JsonPropertyName("success_nodes")]
        ulong[] SuccessNodes { get; }
        [JsonPropertyName("failure_nodes")]
        ulong[] FailureNodes { get; }
        [JsonPropertyName("always_nodes")]
        ulong[] AlwaysNodes { get; }
        [JsonPropertyName("all_parents_must_converge")]
        bool AllParentsMustConverge { get; }
        /// <summary>
        /// Indicates that a job will not be created when <c>True</c>.
        /// Workflow runtime sematics will mark this <c>True</c> if the node is in a path that will decidedly not be ran.
        /// A value of <c>False</c> means the node may not run.
        /// </summary>
        [JsonPropertyName("do_not_run")]
        bool DoNotRun { get; }
        /// <summary>
        /// An identifier coresponding to the workflow job template node that this node was created from.
        /// </summary>
        string Identifier { get; }
    }

    public class WorkflowJobNode(ulong id,
                                 ResourceType type,
                                 string url,
                                 RelatedDictionary related,
                                 WorkflowJobNode.Summary summaryFields,
                                 DateTime created,
                                 DateTime? modified,
                                 OrderedDictionary extraData,
                                 ulong? inventory,
                                 string? scmBranch,
                                 string? jobType,
                                 string? jobTags,
                                 string? skipTags,
                                 string? limit,
                                 bool? diffMode,
                                 JobVerbosity? verbosity,
                                 ulong? executionEnvironment,
                                 int? forks,
                                 int? jobSliceCount,
                                 int? timeout,
                                 ulong? job,
                                 ulong workflowJob,
                                 ulong unifiedJobTemplate,
                                 ulong[] successNodes,
                                 ulong[] failureNodes,
                                 ulong[] alwaysNodes,
                                 bool allParentsMustConverge,
                                 bool doNotRun,
                                 string identifier)
                : IWorkflowJobNode, IResource<WorkflowJobNode.Summary>
    {
        public const string PATH = "/api/v2/workflow_job_nodes/";
        /// <summary>
        /// Retrieve a Workflow Job Node.<br/>
        /// API Path: <c>/api/v2/workflow_job_nodes/<paramref name="id"/>/</c>
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static async Task<WorkflowJobNode> Get(ulong id)
        {
            var apiResult = await RestAPI.GetAsync<WorkflowJobNode>($"{PATH}{id}/");
            return apiResult.Contents;
        }
        /// <summary>
        /// List Workflow Job Nodes.<br/>
        /// API Path: <c>/api/v2/workflow_job_nodes/</c>
        /// </summary>
        /// <param name="query"></param>
        /// <param name="getAll"></param>
        /// <returns></returns>
        public static async IAsyncEnumerable<WorkflowJobNode> Find(NameValueCollection? query, bool getAll = false)
        {
            await foreach(var result in RestAPI.GetResultSetAsync<WorkflowJobNode>(PATH, query, getAll))
            {
                foreach (var jobNode in result.Contents.Results)
                {
                    yield return jobNode;
                }
            }
        }
        public record Summary(
            JobSummary? Job,
            [property: JsonPropertyName("workflow_job")] WorkflowJobSummary WorkflowJob,
            [property: JsonPropertyName("unified_job_template")] UnifiedJobTemplateSummary UnifiedJobTemplate);

        public ulong Id { get; } = id;
        public ResourceType Type { get; } = type;
        public string Url { get; } = url;
        public RelatedDictionary Related { get; } = related;
        public WorkflowJobNode.Summary SummaryFields { get; } = summaryFields;
        public DateTime Created { get; } = created;
        public DateTime? Modified { get; } = modified;

        public OrderedDictionary ExtraData { get; } = extraData;
        public ulong? Inventory { get; } = inventory;
        public string? ScmBranch { get; } = scmBranch;
        public string? JobType { get; } = jobType;
        public string? JobTags { get; } = jobTags;
        public string? SkipTags { get; } = skipTags;
        public string? Limit { get; } = limit;
        public bool? DiffMode { get; } = diffMode;
        public JobVerbosity? Verbosity { get; } = verbosity;
        public ulong? ExecutionEnvironment { get; } = executionEnvironment;
        public int? Forks { get; } = forks;
        public int? JobSliceCount { get; } = jobSliceCount;
        public int? Timeout { get; } = timeout;
        public ulong? Job { get; } = job;
        public ulong WorkflowJob { get; } = workflowJob;
        public ulong UnifiedJobTemplate { get; } = unifiedJobTemplate;
        public ulong[] SuccessNodes { get; } = successNodes;
        public ulong[] FailureNodes { get; } = failureNodes;
        public ulong[] AlwaysNodes { get; } = alwaysNodes;
        public bool AllParentsMustConverge { get; } = allParentsMustConverge;
        public bool DoNotRun { get; } = doNotRun;
        public string Identifier { get; } = identifier;
    }
}
