using System.Collections.Specialized;
using System.Text.Json.Serialization;

namespace AnsibleTower.Resources
{
    public interface IJobTemplateJob : IUnifiedJob
    {
        [JsonPropertyName("execution_node")]
        string ExecutionNode { get; }
        [JsonPropertyName("controller_node")]
        string ControllerNode { get; }
        [JsonPropertyName("job_type")]
        JobType JobType { get; }
        ulong Inventory { get; }
        ulong Project { get; }
        string Playbook { get; }
        [JsonPropertyName("scm_branch")]
        string ScmBranch { get; }
        byte Forks { get; }
        string Limit { get; }
        JobVerbosity Verbosity { get; }
        [JsonPropertyName("extra_vars")]
        string ExtraVars { get; }
        [JsonPropertyName("job_tags")]
        string JobTags { get; }
        [JsonPropertyName("force_handlers")]
        bool ForceHandlers { get; }
        [JsonPropertyName("skip_tags")]
        string SkipTags { get; }
        [JsonPropertyName("start_at_task")]
        string StartAtTask { get; }
        ushort Timeout { get; }
        [JsonPropertyName("use_fact_cache")]
        bool UseFactCache { get; }
        ulong Organization { get; }
        [JsonPropertyName("job_template")]
        ulong JobTemplate { get; }
        [JsonPropertyName("passwords_needed_to_start")]
        string[] PasswordsNeededToStart { get; }
        [JsonPropertyName("allow_simultaneous")]
        bool AllowSimultaneous { get; }
        OrderedDictionary Artifacts { get; }
        [JsonPropertyName("scm_revision")]
        string ScmRevision { get; }
        [JsonPropertyName("instance_group")]
        ulong? InstanceGroup { get; }
        [JsonPropertyName("diff_mode")]
        bool DiffMode { get; }
        [JsonPropertyName("job_slice_number")]
        int JobSliceNumber { get; }
        [JsonPropertyName("job_slice_count")]
        int JobSliceCount { get; }
        [JsonPropertyName("webhook_service")]
        string WebhookService { get; }
        [JsonPropertyName("webhook_credential")]
        uint? WebhookCredential { get; }
        [JsonPropertyName("webhook_guid")]
        string WebhookGuid { get; }

    }
    
    [ResourceType(ResourceType.Job,
        Description = "JobTemplate's Job",
        CanAggregate = false)]
    public class JobTemplateJob(ulong id, ResourceType type, string url, RelatedDictionary related,
                                JobTemplateJob.Summary summaryFields, DateTime created, DateTime? modified, string name,
                                string description, ulong unifiedJobTemplate, JobLaunchType launchType, JobStatus status,
                                ulong? executionEnvironment, bool failed, DateTime? started, DateTime? finished,
                                DateTime? canceledOn, double elapsed, string jobExplanation, string executionNode,
                                string controllerNode, LaunchedBy launchedBy, string workUnitId, JobType jobType,
                                ulong inventory, ulong project, string playbook, string scmBranch, byte forks,
                                string limit, JobVerbosity verbosity, string extraVars, string jobTags,
                                bool forceHandlers, string skipTags, string startAtTask, ushort timeout,
                                bool useFactCache, ulong organization, ulong jobTemplate,
                                string[] passwordsNeededToStart, bool allowSimultaneous, OrderedDictionary artifacts,
                                string scmRevision, ulong? instanceGroup, bool diffMode, int jobSliceNumber,
                                int jobSliceCount, string webhookService, uint? webhookCredential, string webhookGuid)
        : UnifiedJob(id, type, url, created, modified, name, description, unifiedJobTemplate, launchType, status,
                     executionEnvironment, failed, started, finished, canceledOn, elapsed, jobExplanation,
                     launchedBy, workUnitId),
          IJobTemplateJob, IResource<JobTemplateJob.Summary>
    {
        public new const string PATH = "/api/v2/jobs/";
        public static async Task<Detail> Get(ulong id)
        {
            var apiResult = await RestAPI.GetAsync<Detail>($"{PATH}{id}/");
            return apiResult.Contents;
        }
        public static new async IAsyncEnumerable<JobTemplateJob> Find(NameValueCollection? query, bool getAll = false)
        {
            await foreach(var result in RestAPI.GetResultSetAsync<JobTemplateJob>(PATH, query, getAll))
            {
                foreach (var job in result.Contents.Results)
                {
                    yield return job;
                }
            }
        }
        public record Summary(
            NameDescriptionSummary Organization,
            InventorySummary Inventory,
            [property: JsonPropertyName("execution_environment")] EnvironmentSummary? ExecutionEnvironment,
            ProjectSummary Project,
            [property: JsonPropertyName("job_template")] NameDescriptionSummary JobTemplate,
            ScheduleSummary? Schedule,
            [property: JsonPropertyName("unified_job_template")] UnifiedJobTemplateSummary UnifiedJobTemplate,
            [property: JsonPropertyName("instance_group")] InstanceGroupSummary InstanceGroup,
            [property: JsonPropertyName("created_by")] UserSummary CreatedBy,
            [property: JsonPropertyName("user_capabilities")] Capability UserCapabilities,
            ListSummary<NameSummary> Labels,
            JobTemplateCredentialSummary[] Credentials);


        public RelatedDictionary Related { get; } = related;
        public Summary SummaryFields { get; } = summaryFields;

        public string ExecutionNode { get; } = executionNode;
        public string ControllerNode { get; } = controllerNode;

        #region JobTemplateJob Properties
        public JobType JobType { get; } = jobType;
        public ulong Inventory { get; } = inventory;
        public ulong Project { get; } = project;
        public string Playbook { get; } = playbook;
        public string ScmBranch { get; } = scmBranch;
        public byte Forks { get; } = forks;
        public string Limit { get; } = limit;
        public JobVerbosity Verbosity { get; } = verbosity;
        public string ExtraVars { get; } = extraVars;
        public string JobTags { get; } = jobTags;
        public bool ForceHandlers { get; } = forceHandlers;
        public string SkipTags { get; } = skipTags;
        public string StartAtTask { get; } = startAtTask;
        public ushort Timeout { get; } = timeout;
        public bool UseFactCache { get; } = useFactCache;
        public ulong Organization { get; } = organization;
        public ulong JobTemplate { get; } = jobTemplate;
        public string[] PasswordsNeededToStart { get; } = passwordsNeededToStart;
        public bool AllowSimultaneous { get; } = allowSimultaneous;
        public OrderedDictionary Artifacts { get; } = artifacts;
        public string ScmRevision { get; } = scmRevision;
        public ulong? InstanceGroup { get; } = instanceGroup;
        public bool DiffMode { get; } = diffMode;
        public int JobSliceNumber { get; } = jobSliceNumber;
        public int JobSliceCount { get; } = jobSliceCount;
        public string WebhookService { get; } = webhookService;
        public uint? WebhookCredential { get; } = webhookCredential;
        public string WebhookGuid { get; } = webhookGuid;
        #endregion

        public class Detail(ulong id, ResourceType type, string url, RelatedDictionary related,
                            Summary summaryFields, DateTime created, DateTime? modified, string name,
                            string description, JobType jobType, ulong inventory, ulong project, string playbook,
                            string scmBranch, byte forks, string limit, JobVerbosity verbosity, string extraVars,
                            string jobTags, bool forceHandlers, string skipTags, string startAtTask, ushort timeout,
                            bool useFactCache, ulong organization, ulong unifiedJobTemplate, JobLaunchType launchType,
                            JobStatus status, ulong? executionEnvironment, bool failed, DateTime? started,
                            DateTime? finished, DateTime? canceledOn, double elapsed, string jobArgs, string jobCwd,
                            Dictionary<string, string> jobEnv, string jobExplanation, string executionNode,
                            string controllerNode, string resultTraceback, bool eventProcessingFinished,
                            LaunchedBy launchedBy, string workUnitId, ulong jobTemplate, string[] passwordsNeededToStart,
                            bool allowSimultaneous, OrderedDictionary artifacts, string scmRevision,
                            ulong? instanceGroup, bool diffMode, int jobSliceNumber, int jobSliceCount,
                            string webhookService, uint? webhookCredential, string webhookGuid,
                            Dictionary<string, int> hostStatusCounts, Dictionary<string, int> playbookCounts,
                            string customVirtualenv)
            : JobTemplateJob(id, type, url, related, summaryFields, created, modified, name, description,
                             unifiedJobTemplate, launchType, status, executionEnvironment, failed, started,
                             finished, canceledOn, elapsed, jobExplanation, executionNode, controllerNode,
                             launchedBy, workUnitId, jobType, inventory, project, playbook, scmBranch, forks,
                             limit, verbosity, extraVars, jobTags, forceHandlers, skipTags, startAtTask, timeout,
                             useFactCache, organization, jobTemplate, passwordsNeededToStart, allowSimultaneous,
                             artifacts, scmRevision, instanceGroup, diffMode, jobSliceNumber, jobSliceCount,
                             webhookService, webhookCredential, webhookGuid),
               IJobTemplateJob, IJobDetail, IResource<Summary>
        {

            public string JobArgs { get; } = jobArgs;
            public string JobCwd { get; } = jobCwd;
            public Dictionary<string, string> JobEnv { get; } = jobEnv;
            public string ResultTraceback { get; } = resultTraceback;
            public bool EventProcessingFinished { get; } = eventProcessingFinished;


            [JsonPropertyName("host_status_counts")]
            public Dictionary<string, int> HostStatusCounts { get; } = hostStatusCounts;

            [JsonPropertyName("playbook_counts")]
            public Dictionary<string, int> PlaybookCounts { get; } = playbookCounts;

            [JsonPropertyName("custom_virtualenv")]
            public string? CustomVirtualenv { get; } = customVirtualenv;
        }
    }
}

