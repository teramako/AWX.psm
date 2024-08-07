using System.Collections.Specialized;
using System.Text.Json.Serialization;

namespace AWX.Resources
{
    [Flags]
    public enum ProjectOptions
    {
        None = 0,
        ScmClean = 1 << 0,
        ScmDeleteOnUpdate = 1 << 1,
        ScmTrackSubmodules = 1 << 2,
        ScmUpdateOnLaunch = 1 << 3,
        AllowOverride = 1 << 4
    }

    public interface IProject
    {
        /// <summary>
        /// Name of this project.
        /// </summary>
        string Name { get; }
        /// <summary>
        /// Optional description of this project.
        /// </summary>
        string Description { get; }
        /// <summary>
        /// Local path (relative <c>PROJECT_ROOT</c>) containing playbooks and related files for this project.
        /// </summary>
        [JsonPropertyName("local_path")]
        string LocalPath { get; }
        /// <summary>
        /// Specifies the source control system used to store the project.
        /// </summary>
        [JsonPropertyName("scm_type")]
        string ScmType { get; }
        /// <summary>
        /// The location where the project is stored.
        /// </summary>
        [JsonPropertyName("scm_url")]
        string ScmUrl { get; }
        /// <summary>
        /// Specific branch, tag or commit to checkout.
        /// </summary>
        [JsonPropertyName("scm_branch")]
        string ScmBranch { get; }
        /// <summary>
        /// For git projects, an additional refspec to fetch.
        /// </summary>
        [JsonPropertyName("scm_refspec")]
        string ScmRefspec { get; }
        /// <summary>
        /// Discard any local changes before syncing the project.
        /// </summary>
        [JsonPropertyName("scm_clean")]
        bool ScmClean { get; }
        /// <summary>
        /// Track submodules latest commits on defined branch.
        /// </summary>
        [JsonPropertyName("scm_track_submodules")]
        bool ScmTrackSubmodules { get; }
        /// <summary>
        /// Delete the project before syncing.
        /// </summary>
        [JsonPropertyName("scm_delete_on_update")]
        bool ScmDeleteOnUpdate { get; }
        ulong? Credential { get; }
        /// <summary>
        /// The amount of time (in seconds) to run before the task is canceled.
        /// </summary>
        int Timeout { get; }
        /// <summary>
        /// The organization used to determine access to this template.
        /// </summary>
        ulong Organization { get; }
        /// <summary>
        /// Update the project when a job is launched that used the project.
        /// </summary>
        [JsonPropertyName("scm_update_on_launch")]
        bool ScmUpdateOnLaunch { get; }
        /// <summary>
        /// The number of seconds after the last project update ran that
        /// a new project update will be launched as a job dependency.
        /// </summary>
        [JsonPropertyName("scm_update_cache_timeout")]
        int ScmUpdateCacheTimeout { get; }
        /// <summary>
        /// Allow changing the SCM branch or revision in a job template that uses this project.
        /// </summary>
        [JsonPropertyName("allow_override")]
        bool AllowOverride { get; }
        /// <summary>
        /// The default execution environment for jobs run using this project.
        /// </summary>
        [JsonPropertyName("default_environment")]
        ulong? DefaultEnvironment { get; }
        /// <summary>
        /// An optional credential used for validating files in the project against unexpected changes.
        /// </summary>
        [JsonPropertyName("signature_validation_credential")]
        ulong? SignatureValidationCredential { get; }
    }

    public class Project(ulong id, ResourceType type, string url, RelatedDictionary related,
                         Project.Summary summaryFields, DateTime created, DateTime? modified, string name,
                         string description, string localPath, string scmType, string scmUrl, string scmBranch,
                         string scmRefspec, bool scmClean, bool scmTrackSubmodules, bool scmDeleteOnUpdate,
                         ulong? credential, int timeout, string scmRevision, DateTime? lastJobRun, bool lastJobFailed,
                         DateTime? nextJobRun, JobTemplateStatus status, ulong organization, bool scmUpdateOnLaunch,
                         int scmUpdateCacheTimeout, bool allowOverride, string? customVirtualenv,
                         ulong? defaultEnvironment, ulong? signatureValidationCredential, bool lastUpdateFailed,
                         DateTime? lastUpdated)
        : UnifiedJobTemplate(id, type, url, created, modified, name, description, lastJobRun,
                             lastJobFailed, nextJobRun, status),
          IProject, IUnifiedJobTemplate, IResource<Project.Summary>
    {
        public new const string PATH = "/api/v2/projects/";


        /// <summary>
        /// Retrieve a Project.<br/>
        /// API Path: <c>/api/v2/projects/<paramref name="id"/>/</c>
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static async Task<Project> Get(ulong id)
        {
            var apiResult = await RestAPI.GetAsync<Project>($"{PATH}{id}/");
            return apiResult.Contents;
        }
        /// <summary>
        /// List Projects.<br/>
        /// API Path: <c>/api/v2/projects/</c>
        /// </summary>
        /// <param name="query"></param>
        /// <param name="getAll"></param>
        /// <returns></returns>
        public static new async IAsyncEnumerable<Project> Find(NameValueCollection? query, bool getAll = false)
        {
            await foreach(var result in RestAPI.GetResultSetAsync<Project>(PATH, query, getAll))
            {
                foreach (var project in result.Contents.Results)
                {
                    yield return project;
                }
            }
        }
        /// <summary>
        /// List Projects for an Organization.<br/>
        /// API Path: <c>/api/v2/organizations/<paramref name="organizationId"/>/projects/</c>
        /// </summary>
        /// <param name="organizationId"></param>
        /// <param name="query"></param>
        /// <param name="getAll"></param>
        /// <returns></returns>
        public static async IAsyncEnumerable<Project> FindFromOrganization(ulong organizationId,
                                                                           NameValueCollection? query = null,
                                                                           bool getAll = false)
        {
            var path = $"{Resources.Organization.PATH}{organizationId}/projects/";
            await foreach (var result in RestAPI.GetResultSetAsync<Project>(path, query, getAll))
            {
                foreach (var project in result.Contents.Results)
                {
                    yield return project;
                }
            }
        }
        /// <summary>
        /// List Projects for a User.<br/>
        /// API Path: <c>/api/v2/users/<paramref name="userId"/>/projects/</c>
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="query"></param>
        /// <param name="getAll"></param>
        /// <returns></returns>
        public static async IAsyncEnumerable<Project> FindFromUser(ulong userId,
                                                                   NameValueCollection? query = null,
                                                                   bool getAll = false)
        {
            var path = $"{User.PATH}{userId}/projects/";
            await foreach (var result in RestAPI.GetResultSetAsync<Project>(path, query, getAll))
            {
                foreach (var project in result.Contents.Results)
                {
                    yield return project;
                }
            }
        }
        /// <summary>
        /// List Projects for a Team.<br/>
        /// API Path: <c>/api/v2/teams/<paramref name="teamId"/>/projects/</c>
        /// </summary>
        /// <param name="teamId"></param>
        /// <param name="query"></param>
        /// <param name="getAll"></param>
        /// <returns></returns>
        public static async IAsyncEnumerable<Project> FindFromTeam(ulong teamId,
                                                                   NameValueCollection? query = null,
                                                                   bool getAll = false)
        {
            var path = $"{Team.PATH}{teamId}/projects/";
            await foreach (var result in RestAPI.GetResultSetAsync<Project>(path, query, getAll))
            {
                foreach (var project in result.Contents.Results)
                {
                    yield return project;
                }
            }
        }

        /// <summary>
        /// Get inventory files and directories.
        /// </summary>
        /// <returns>Array of inventory files and directories available with in the project, not comprehensive</returns>
        public static async Task<string[]> GetInventoryFiles(ulong projectId)
        {
            var apiResult = await RestAPI.GetAsync<string[]>($"{PATH}{projectId}/inventories/");
            return apiResult.Contents;
        }

        public record Summary(
            NameDescriptionSummary Organization,
            [property: JsonPropertyName("default_environment")] EnvironmentSummary? DefaultEnvironment,
            CredentialSummary? Credential,
            [property: JsonPropertyName("last_job")] LastJobSummary? LastJob,
            [property: JsonPropertyName("last_update")] LastUpdateSummary? LastUpdate,
            [property: JsonPropertyName("created_by")] UserSummary CreatedBy,
            [property: JsonPropertyName("modified_by")] UserSummary ModifiedBy,
            [property: JsonPropertyName("object_roles")] Dictionary<string, NameDescriptionSummary> ObjectRoles,
            [property: JsonPropertyName("user_capabilities")] Capability UserCapabilities);


        public RelatedDictionary Related { get; } = related;
        public Summary SummaryFields { get; } = summaryFields;
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
        [JsonPropertyName("scm_revision")]
        public string ScmRevision { get; } = scmRevision;
        public ulong Organization { get; } = organization;
        public bool ScmUpdateOnLaunch { get; } = scmUpdateOnLaunch;
        public int ScmUpdateCacheTimeout { get; } = scmUpdateCacheTimeout;
        public bool AllowOverride { get; } = allowOverride;
        [JsonPropertyName("custom_virtualenv")]
        public string? CustomVirtualenv { get; } = customVirtualenv;
        public ulong? DefaultEnvironment { get; } = defaultEnvironment;
        public ulong? SignatureValidationCredential { get; } = signatureValidationCredential;
        [JsonPropertyName("last_update_failed")]
        public bool LastUpdateFailed { get; } = lastUpdateFailed;
        [JsonPropertyName("last_updated")]
        public DateTime? LastUpdated { get; } = lastUpdated;

        [JsonIgnore(Condition = JsonIgnoreCondition.Always)]
        public ProjectOptions Options
        {
            get
            {
                return (ScmClean ? ProjectOptions.ScmClean : 0) |
                       (ScmDeleteOnUpdate ? ProjectOptions.ScmDeleteOnUpdate : 0) |
                       (ScmTrackSubmodules ? ProjectOptions.ScmTrackSubmodules : 0) |
                       (ScmUpdateOnLaunch ? ProjectOptions.ScmUpdateOnLaunch : 0) |
                       (AllowOverride ? ProjectOptions.AllowOverride : 0);
            }
        }

        /// <summary>
        /// Get inventory files and directories.
        /// </summary>
        /// <returns>Array of inventory files and directories available with in this project, not comprehensive</returns>
        public async Task<string[]> GetInventoryFiles()
        {
            return await GetInventoryFiles(Id);
        }
    }
}
