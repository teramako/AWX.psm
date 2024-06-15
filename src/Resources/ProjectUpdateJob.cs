using System.Collections.Specialized;
using System.Text.Json.Serialization;

namespace AWX.Resources
{
    public interface IProjectUpdateJob : IUnifiedJob
    {
        [JsonPropertyName("execution_node")]
        string ExecutionNode { get; }

        [JsonPropertyName("local_path")]
        string LocalPath { get; }
        [JsonPropertyName("scm_type")]
        string ScmType { get; }
        [JsonPropertyName("scm_url")]
        string ScmUrl { get; }
        [JsonPropertyName("scm_branch")]
        string ScmBranch { get; }
        [JsonPropertyName("scm_refspec")]
        string ScmRefspec { get; }
        [JsonPropertyName("scm_clean")]
        bool ScmClean { get; }
        [JsonPropertyName("scm_track_submodules")]
        bool ScmTrackSubmodules { get; }
        [JsonPropertyName("scm_delete_on_update")]
        bool ScmDeleteOnUpdate { get; }
        ulong? Credential { get; }
        int Timeout { get; }
        [JsonPropertyName("scm_revision")]
        string ScmRevision { get; }
        ulong Project { get; }
        [JsonPropertyName("job_type")]
        JobType JobType { get; }
        [JsonPropertyName("job_tags")]
        string JobTags { get; }
    }

    public class ProjectUpdateJob(ulong id, ResourceType type, string url, RelatedDictionary related,
                                  ProjectUpdateJob.Summary summaryFields, DateTime created, DateTime? modified,
                                  string name, string description, ulong unifiedJobTemplate, JobLaunchType launchType,
                                  JobStatus status, ulong? executionEnvironment, bool failed, DateTime? started,
                                  DateTime? finished, DateTime? canceledOn, double elapsed, string jobExplanation,
                                  string executionNode, LaunchedBy launchedBy, string workUnitId, string localPath,
                                  string scmType, string scmUrl, string scmBranch, string scmRefspec, bool scmClean,
                                  bool scmTrackSubmodules, bool scmDeleteOnUpdate, ulong? credential, int timeout,
                                  string scmRevision, ulong project, JobType jobType, string jobTags)
        : UnifiedJob(id, type, url, created, modified, name, description, unifiedJobTemplate, launchType, status,
                     executionEnvironment, failed, started, finished, canceledOn, elapsed, jobExplanation,
                     launchedBy, workUnitId),
          IProjectUpdateJob, IResource<ProjectUpdateJob.Summary>
    {
        public new const string PATH = "/api/v2/project_updates/";

        public static async Task<Detail> Get(ulong id)
        {
            var apiResult = await RestAPI.GetAsync<Detail>($"{PATH}{id}/");
            return apiResult.Contents;
        }
        public static new async IAsyncEnumerable<ProjectUpdateJob> Find(NameValueCollection? query, bool getAll = false)
        {
            await foreach(var result in RestAPI.GetResultSetAsync<ProjectUpdateJob>(PATH, query, getAll))
            {
                foreach (var app in result.Contents.Results)
                {
                    yield return app;
                }
            }
        }
        public record Summary(
            NameDescriptionSummary Organization,
            [property: JsonPropertyName("default_environment")] EnvironmentSummary? DefaultEnvironment,
            ProjectSummary Project,
            CredentialSummary? Credential,
            [property: JsonPropertyName("unified_job_template")] UnifiedJobTemplateSummary UnifiedJobTemplate,
            [property: JsonPropertyName("instance_group")] InstanceGroupSummary InstanceGroup,
            [property: JsonPropertyName("user_capabilities")] Capability UserCapabilities);


        public class Detail(ulong id, ResourceType type, string url, RelatedDictionary related,
                            Summary summaryFields, DateTime created, DateTime? modified, string name,
                            string description, string localPath, string scmType, string scmUrl, string scmBranch,
                            string scmRefspec, bool scmClean, bool scmTrackSubmodules, bool scmDeleteOnUpdate,
                            ulong? credential, int timeout, string scmRevision, ulong unifiedJobTemplate,
                            JobLaunchType launchType, JobStatus status, ulong? executionEnvironment, bool failed,
                            DateTime? started, DateTime? finished, DateTime? canceledOn, double elapsed, string jobArgs,
                            string jobCwd, Dictionary<string, string> jobEnv, string jobExplanation,
                            string executionNode, string resultTraceback, bool eventProcessingFinished,
                            LaunchedBy launchedBy, string workUnitId, ulong project, JobType jobType, string jobTags,
                            Dictionary<string, int> hostStatusCounts, Dictionary<string, int> playbookCounts)
            : ProjectUpdateJob(id, type, url, related, summaryFields, created, modified, name, description,
                               unifiedJobTemplate, launchType, status, executionEnvironment, failed, started,
                               finished, canceledOn, elapsed, jobExplanation, executionNode, launchedBy, workUnitId,
                               localPath, scmType, scmUrl, scmBranch, scmRefspec, scmClean, scmTrackSubmodules,
                               scmDeleteOnUpdate, credential, timeout, scmRevision, project, jobType, jobTags),
              IProjectUpdateJob, IJobDetail
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
        }


        public RelatedDictionary Related { get; } = related;
        public Summary SummaryFields { get; } = summaryFields;

        public string ExecutionNode { get; } = executionNode;

        public string LocalPath { get; } = localPath;
        public string ScmType { get; } = scmType;
        public string ScmUrl { get; } = scmUrl;
        public string ScmBranch { get; } = scmBranch;
        public string ScmRefspec { get; } = scmRefspec;
        public bool ScmClean { get; } = scmClean;
        public bool ScmTrackSubmodules { get; } = scmTrackSubmodules;
        public bool ScmDeleteOnUpdate { get; } = scmDeleteOnUpdate;
        public ulong? Credential { get; } = credential;
        public int Timeout { get; } = timeout;
        public string ScmRevision { get; } = scmRevision;
        public ulong Project { get; } = project;
        public JobType JobType { get; } = jobType;
        public string JobTags { get; } = jobTags;
    }
}
