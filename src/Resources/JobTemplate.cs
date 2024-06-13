using System.Collections.Specialized;
using System.Text.Json.Serialization;

namespace AnsibleTower.Resources
{
    public interface IJobTemplate
    {
        /// <summary>
        /// Name of this job template.
        /// </summary>
        string Name { get; }
        /// <summary>
        /// Optional description of this job template.
        /// </summary>
        string Description { get; }
        /// <summary>
        /// Job type
        /// <list type="bullet">
        ///     <item><term><c>run</c></term><description>Run (default)</description></item>
        ///     <item><term><c>check</c></term><description>Check mode (dry run)</description></item>
        /// </list>
        /// </summary>
        [JsonPropertyName("job_type")]
        JobType JobType { get; }
        /// <summary>
        /// Inventory ID.
        /// </summary>
        ulong Inventory { get; }
        /// <summary>
        /// Project ID.
        /// </summary>
        ulong Project { get; }
        /// <summary>
        /// Playbook file in the project.
        /// </summary>
        string Playbook { get; }
        /// <summary>
        /// Branch to use in job run.
        /// Project default used if blank. Only allowed if project <c>allow_override</c> field is set to <c>true</c>.
        /// </summary>
        [JsonPropertyName("scm_branch")]
        string ScmBranch { get; }
        /// <summary>
        /// Number of max concurrent fork processes.
        /// </summary>
        int Forks { get; }
        string Limit { get; }
        JobVerbosity Verbosity { get; }
        [JsonPropertyName("extra_vars")]
        string ExtraVars { get; }
        [JsonPropertyName("job_tags")]
        string JobTags { get; }
        [JsonPropertyName("start_at_task")]
        string StartAtTask { get; }
        /// <summary>
        /// The amount of time (in seconds) to run before the task is caceled.
        /// </summary>
        int Timeout { get; }
        /// <summary>
        /// If enabled, the service will act as an Ansible Fact Cache Plugin;
        /// persisting facts at the end of a playbook run to the database and caching facts for use by Ansible.
        /// </summary>
        [JsonPropertyName("use_fact_cache")]
        bool UseFactCache { get; }
        [JsonPropertyName("execution_environment")]
        public ulong? ExecutionEnvironment { get; }
        [JsonPropertyName("host_config_key")]
        string HostConfigKey { get; }
        [JsonPropertyName("ask_scm_branch_on_launch")]
        bool AskScmBranchOnLaunch { get; }
        [JsonPropertyName("ask_diff_mode_on_launch")]
        bool AskDiffModeOnLaunch { get; }
        [JsonPropertyName("ask_variables_on_launch")]
        bool AskVariablesOnLaunch { get; }
        [JsonPropertyName("ask_limit_on_launch")]
        bool AskLimitOnLaunch { get; }
        [JsonPropertyName("ask_tags_on_launch")]
        bool AskTagsOnLaunch { get; }
        [JsonPropertyName("ask_skip_tags_on_launch")]
        bool AskSkipTagsOnLaunch { get; }
        [JsonPropertyName("ask_job_type_on_launch")]
        bool AskJobTypeOnLaunch { get; }
        [JsonPropertyName("ask_verbosity_on_launch")]
        bool AskVerbosityOnLaunch { get; }
        [JsonPropertyName("ask_inventory_on_launch")]
        bool AskInventoryOnLaunch { get; }
        [JsonPropertyName("ask_credential_on_launch")]
        bool AskCredentialOnLaunch { get; }
        [JsonPropertyName("ask_execution_environment_on_launch")]
        bool AskExecutionEnvironmentOnLaunch { get; }
        [JsonPropertyName("ask_labels_on_launch")]
        bool AskLabelsOnLaunch { get; }
        [JsonPropertyName("ask_forks_on_launch")]
        bool AskForksOnLaunch { get; }
        [JsonPropertyName("ask_job_slice_count_on_launch")]
        bool AskJobSliceCountOnLaunch { get; }
        [JsonPropertyName("ask_timeout_on_launch")]
        bool AskTimeoutOnLaunch { get; }
        [JsonPropertyName("ask_instance_groups_on_launch")]
        bool AskInstanceGroupsOnLaunch { get; }
        [JsonPropertyName("survey_enabled")]
        bool SurveyEnabled { get; }
        [JsonPropertyName("become_enabled")]
        bool BecomeEnabled { get; }
        /// <summary>
        /// If enabled, texual changes mode to ny templated files on the host are shown in the standard output.
        /// </summary>
        [JsonPropertyName("diff_mode")]
        bool DiffMode { get; }
        [JsonPropertyName("allow_simultaneous")]
        bool AllowSimultaneous { get; }
        /// <summary>
        /// The number of jobs to slice into at runtime.
        /// Will cause the Job Template to launch a workflow if value is greater than <c>1</c>.
        /// </summary>
        [JsonPropertyName("job_slice_count")]
        int JobSliceCount { get; }
        [JsonPropertyName("webhook_service")]
        string WebhookService { get; }
        [JsonPropertyName("webhook_credential")]
        ulong? WebhookCredential { get; }
        [JsonPropertyName("prevent_instance_group_fallback")]
        bool PreventInstanceGroupFallback { get; }
    }
    

    [ResourceType(ResourceType.JobTemplate)]
    public class JobTemplate(ulong id, ResourceType type, string url, RelatedDictionary related,
                             JobTemplate.Summary summaryFields, DateTime created, DateTime? modified, string name,
                             string description, JobType jobType, ulong inventory, ulong project, string playbook,
                             string scmBranch, int forks, string limit, JobVerbosity verbosity, string extraVars,
                             string jobTags, bool forceHandlers, string startAtTask, int timeout, bool useFactCache,
                             ulong organization, DateTime? lastJobRun, bool lastJobFailed, DateTime? nextJobRun,
                             JobTemplateStatus status, ulong? executionEnvironment, string hostConfigKey,
                             bool askScmBranchOnLaunch, bool askDiffModeOnLaunch, bool askVariablesOnLaunch,
                             bool askLimitOnLaunch, bool askTagsOnLaunch, bool askSkipTagsOnLaunch,
                             bool askJobTypeOnLaunch, bool askVerbosityOnLaunch, bool askInventoryOnLaunch,
                             bool askCredentialOnLaunch, bool askExecutionEnvironmentOnLaunch, bool askLabelsOnLaunch,
                             bool askForksOnLaunch, bool askJobSliceCountOnLaunch, bool askTimeoutOnLaunch,
                             bool askInstanceGroupsOnLaunch, bool surveyEnabled, bool becomeEnabled, bool diffMode,
                             bool allowSimultaneous, string? customVirtualenv, int jobSliceCount, string webhookService,
                             ulong? webhookCredential, bool preventInstanceGroupFallback)
        : UnifiedJobTemplate(id, type, url, created, modified, name, description, lastJobRun,
                             lastJobFailed, nextJobRun, status),
          IJobTemplate, IUnifiedJobTemplate, IResource<JobTemplate.Summary>
    {
        public new const string PATH = "/api/v2/job_templates/";

        public static async Task<JobTemplate> Get(ulong id)
        {
            var apiResult = await RestAPI.GetAsync<JobTemplate>($"{PATH}{id}/");
            return apiResult.Contents;
        }
        public static new async IAsyncEnumerable<JobTemplate> Find(NameValueCollection? query, bool getAll = false)
        {
            await foreach(var result in RestAPI.GetResultSetAsync<JobTemplate>(PATH, query, getAll))
            {
                foreach (var jobTemplate in result.Contents.Results)
                {
                    yield return jobTemplate;
                }
            }
        }
    public record Summary(
        NameDescriptionSummary Organization,
        InventorySummary Inventory,
        ProjectSummary Project,
        [property: JsonPropertyName("last_job")] LastJobSummary? LastJob,
        [property: JsonPropertyName("last_update")] LastUpdateSummary? LastUpdate,
        [property: JsonPropertyName("created_by")] UserSummary CreatedBy,
        [property: JsonPropertyName("modified_by")] UserSummary? ModifiedBy,
        [property: JsonPropertyName("object_roles")] Dictionary<string, NameDescriptionSummary> ObjectRoles,
        [property: JsonPropertyName("user_capabilities")] Capability UserCapabilities,
        ListSummary<NameSummary> Labels,
        [property: JsonPropertyName("resolved_environment")] EnvironmentSummary? ResolvedEnvironment,
        [property: JsonPropertyName("recent_jobs")] JobTemplateRecentJobSummary[] RecentJobs,
        JobTemplateCredentialSummary[] Credentials);


        public RelatedDictionary Related { get; } = related;
        public Summary SummaryFields { get; } = summaryFields;
        public JobType JobType { get; } = jobType;
        public ulong Inventory { get; } = inventory;
        public ulong Project { get; } = project;
        public string Playbook { get; } = playbook;
        public string ScmBranch { get; } = scmBranch;
        public int Forks { get; } = forks;
        public string Limit { get; } = limit;
        public JobVerbosity Verbosity { get; } = verbosity;
        public string ExtraVars { get; } = extraVars;
        public string JobTags { get; } = jobTags;
        [JsonPropertyName("force_handlers")]
        public bool ForceHandlers { get; } = forceHandlers;
        public string StartAtTask { get; } = startAtTask;
        public int Timeout { get; } = timeout;
        public bool UseFactCache { get; } = useFactCache;
        public ulong Organization { get; } = organization;
        public ulong? ExecutionEnvironment { get; } = executionEnvironment;
        public string HostConfigKey { get; } = hostConfigKey;
        public bool AskScmBranchOnLaunch { get; } = askScmBranchOnLaunch;
        public bool AskDiffModeOnLaunch { get; } = askDiffModeOnLaunch;
        public bool AskVariablesOnLaunch { get; } = askVariablesOnLaunch;
        public bool AskLimitOnLaunch { get; } = askLimitOnLaunch;
        public bool AskTagsOnLaunch { get; } = askTagsOnLaunch;
        public bool AskSkipTagsOnLaunch { get; } = askSkipTagsOnLaunch;
        public bool AskJobTypeOnLaunch { get; } = askJobTypeOnLaunch;
        public bool AskVerbosityOnLaunch { get; } = askVerbosityOnLaunch;
        public bool AskInventoryOnLaunch { get; } = askInventoryOnLaunch;
        public bool AskCredentialOnLaunch { get; } = askCredentialOnLaunch;
        public bool AskExecutionEnvironmentOnLaunch { get; } = askExecutionEnvironmentOnLaunch;
        public bool AskLabelsOnLaunch { get; } = askLabelsOnLaunch;
        public bool AskForksOnLaunch { get; } = askForksOnLaunch;
        public bool AskJobSliceCountOnLaunch { get; } = askJobSliceCountOnLaunch;
        public bool AskTimeoutOnLaunch { get; } = askTimeoutOnLaunch;
        public bool AskInstanceGroupsOnLaunch { get; } = askInstanceGroupsOnLaunch;
        public bool SurveyEnabled { get; } = surveyEnabled;
        public bool BecomeEnabled { get; } = becomeEnabled;
        public bool DiffMode { get; } = diffMode;
        public bool AllowSimultaneous { get; } = allowSimultaneous;
        [JsonPropertyName("custom_virtualenv")]
        public string? CustomVirtualenv { get; } = customVirtualenv;
        public int JobSliceCount { get; } = jobSliceCount;
        public string WebhookService { get; } = webhookService;
        public ulong? WebhookCredential { get; } = webhookCredential;
        public bool PreventInstanceGroupFallback { get; } = preventInstanceGroupFallback;
    }
}
