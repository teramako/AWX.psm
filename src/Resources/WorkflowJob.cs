using System.Collections.Specialized;
using System.Text.Json.Serialization;

namespace AnsibleTower.Resources
{
    public interface IWorkflowJob : IUnifiedJob
    {
        [JsonPropertyName("workflow_job_template")]
        ulong WorkflowJobTemplate { get; }
        [JsonPropertyName("extra_vars")]
        string ExtraVars { get; }
        [JsonPropertyName("allow_simultaneous")]
        bool AllowSimultaneous { get; }
        /// <summary>
        /// If automatically created for a sliced job run, the job template the workflow job was created from.
        /// </summary>
        [JsonPropertyName("job_template")]
        ulong? JobTemplate { get; }
        [JsonPropertyName("is_sliced_job")]
        bool IsSlicedJob { get; }
        /// <summary>
        /// Inventory applied as a prompt, assuming job template prompts for inventory.
        /// </summary>
        ulong? Inventory { get; }
        string? Limit { get; }
        [JsonPropertyName("scm_branch")]
        string? ScmBranch { get; }
        [JsonPropertyName("webhook_service")]
        string WebhookService { get; }
        [JsonPropertyName("webhook_credential")]
        ulong? WebhookCredential { get; }
        [JsonPropertyName("webhook_guid")]
        string WebhookGuid { get; }
        [JsonPropertyName("skip_tags")]
        string? SkipTags { get; }
        [JsonPropertyName("job_tags")]
        string? JobTags { get; }
    }


    public class WorkflowJob(ulong id, ResourceType type, string url, RelatedDictionary related,
                             WorkflowJob.Summary summaryFields, DateTime created, DateTime? modified, string name,
                             string description, ulong unifiedJobTemplate, JobLaunchType launchType, JobStatus status,
                             ulong? executionEnvironment, bool failed, DateTime? started, DateTime? finished,
                             DateTime? canceledOn, double elapsed, string jobExplanation, LaunchedBy launchedBy,
                             string? workUnitId, ulong workflowJobTemplate, string extraVars, bool allowSimultaneous,
                             ulong? jobTemplate, bool isSlicedJob, ulong? inventory, string? limit, string? scmBranch,
                             string webhookService, ulong? webhookCredential, string webhookGuid, string? skipTags,
                             string? jobTags)
                : IWorkflowJob, IResource<WorkflowJob.Summary>
    {
        public const string PATH = "/api/v2/workflow_jobs/";
        public static async Task<WorkflowJob> Get(ulong id)
        {
            var apiResult = await RestAPI.GetAsync<WorkflowJob>($"{PATH}{id}/");
            return apiResult.Contents;
        }
        public static async IAsyncEnumerable<WorkflowJob> Find(NameValueCollection? query, bool getAll = false)
        {
            await foreach(var result in RestAPI.GetResultSetAsync<WorkflowJob>(PATH, query, getAll))
            {
                foreach (var res in result.Contents.Results)
                {
                    yield return res;
                }
            }
        }
        public record Summary(
            [property: JsonPropertyName("workflow_job_template")] NameDescriptionSummary WorkflowJobTemplate,
            ScheduleSummary? Schedule,
            [property: JsonPropertyName("unified_job_template")] UnifiedJobTemplateSummary UnifiedJobTemplate,
            [property: JsonPropertyName("created_by")] UserSummary? CreatedBy,
            [property: JsonPropertyName("modified_by")] UserSummary? ModifiedBy,
            [property: JsonPropertyName("user_capabilities")] Capability UserCapabilities,
            ListSummary<NameSummary> Labels);

        public ulong Id { get; } = id;
        public ResourceType Type { get; } = type;
        public string Url { get; } = url;
        public RelatedDictionary Related { get; } = related;
        public Summary SummaryFields { get; } = summaryFields;
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
        public LaunchedBy LaunchedBy { get; } = launchedBy;
        public string? WorkUnitId { get; } = workUnitId;
        public ulong WorkflowJobTemplate { get; } = workflowJobTemplate;
        public string ExtraVars { get; } = extraVars;
        public bool AllowSimultaneous { get; } = allowSimultaneous;
        public ulong? JobTemplate { get; } = jobTemplate;
        public bool IsSlicedJob { get; } = isSlicedJob;
        public ulong? Inventory { get; } = inventory;
        public string? Limit { get; } = limit;
        public string? ScmBranch { get; } = scmBranch;
        public string WebhookService { get; } = webhookService;
        public ulong? WebhookCredential { get; } = webhookCredential;
        public string WebhookGuid { get; } = webhookGuid;
        public string? SkipTags { get; } = skipTags;
        public string? JobTags { get; } = jobTags;
    }
}
