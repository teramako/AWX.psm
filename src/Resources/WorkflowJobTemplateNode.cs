using System.Collections.Specialized;
using System.Text.Json.Serialization;

namespace AWX.Resources
{
    public interface IWorkflowJobTemplateNode
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
        [JsonPropertyName("workflow_job_template")]
        ulong WorkflowJobTemplate { get; }
        [JsonPropertyName("unified_job_template")]
        ulong? UnifiedJobTemplate { get; }
        [JsonPropertyName("all_parents_must_converge")]
        bool AllParentsMustConverge { get; }
        string Identifier { get; }
    }


    public class WorkflowJobTemplateNode(ulong id,
                                         ResourceType type,
                                         string url,
                                         RelatedDictionary related,
                                         WorkflowJobTemplateNode.Summary summaryFields,
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
                                         ulong workflowJobTemplate,
                                         ulong? unifiedJobTemplate,
                                         ulong[] successNodes,
                                         ulong[] failureNodes,
                                         ulong[] alwaysNodes,
                                         bool allParentsMustConverge,
                                         string identifier)
                : IWorkflowJobTemplateNode, IResource<WorkflowJobTemplateNode.Summary>
    {
        public const string PATH = "/api/v2/workflow_job_template_nodes/";
        /// <summary>
        /// Retrieve a Workflow Job Template Node.<br/>
        /// API Path: <c>/api/v2/workflow_job_template_nodes/<paramref name="id"/>/</c>
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static async Task<WorkflowJobTemplateNode> Get(ulong id)
        {
            var apiResult = await RestAPI.GetAsync<WorkflowJobTemplateNode>($"{PATH}{id}/");
            return apiResult.Contents;
        }
        /// <summary>
        /// List Workflow Job Template Nodes.<br/>
        /// API Path: <c>/api/v2/workflow_job_templates_nodes/</c>
        /// </summary>
        /// <param name="query"></param>
        /// <param name="getAll"></param>
        /// <returns></returns>
        public static async IAsyncEnumerable<WorkflowJobTemplateNode> Find(NameValueCollection? query, bool getAll = false)
        {
            await foreach(var result in RestAPI.GetResultSetAsync<WorkflowJobTemplateNode>(PATH, query, getAll))
            {
                foreach (var jobTemplateNode in result.Contents.Results)
                {
                    yield return jobTemplateNode;
                }
            }
        }
        public record Summary(
            [property: JsonPropertyName("workflow_job_template")] WorkflowJobTemplateSummary WorkflowJobTemplate,
            [property: JsonPropertyName("unified_job_template")] UnifiedJobTemplateSummary UnifiedJobTemplate);

        public ulong Id { get; } = id;
        public ResourceType Type { get; } = type;
        public string Url { get; } = url;
        public RelatedDictionary Related { get; } = related;
        public Summary SummaryFields { get; } = summaryFields;
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
        public ulong WorkflowJobTemplate { get; } = workflowJobTemplate;
        public ulong? UnifiedJobTemplate { get; } = unifiedJobTemplate;
        [JsonPropertyName("success_nodes")]
        public ulong[] SuccessNodes { get; } = successNodes;
        [JsonPropertyName("failure_nodes")]
        public ulong[] FailureNodes { get; } = failureNodes;
        [JsonPropertyName("always_nodes")]
        public ulong[] AlwaysNodes { get; } = alwaysNodes;
        public bool AllParentsMustConverge { get; } = allParentsMustConverge;
        public string Identifier { get; } = identifier;
    }
}
