using System.Collections.Specialized;
using System.Text.Json.Serialization;

namespace AWX.Resources
{
    [JsonConverter(typeof(Json.EnumUpperCamelCaseStringConverter<InventorySourceSource>))]
    public enum InventorySourceSource
    {
        /// <summary>
        /// File, Directory or Script
        /// </summary>
        File,
        /// <summary>
        /// Template additional groups and hostvars at runtime.
        /// </summary>
        Constructed,
        /// <summary>
        /// Sourced from a Project
        /// </summary>
        Scm,
        /// <summary>
        /// Amazon EC2
        /// </summary>
        EC2,
        /// <summary>
        /// Google Compute Engine
        /// </summary>
        GCE,
        /// <summary>
        /// Microsoft Azure Resource Manager
        /// </summary>
        AzureRM,
        /// <summary>
        /// VMware vCenter
        /// </summary>
        VMware,
        /// <summary>
        /// Red Hat Satelite 6
        /// </summary>
        Satelite6,
        /// <summary>
        /// OpenStack
        /// </summary>
        OpenStack,
        /// <summary>
        /// Red Hat Virtualization
        /// </summary>
        RHV,
        /// <summary>
        /// Red Hat Ansible Automation Platform
        /// </summary>
        Controller,
        /// <summary>
        /// Red Hat Insights
        /// </summary>
        Insights,
    }

    public interface IInventorySource
    {
        /// <summary>
        /// Name of the inventory source.
        /// </summary>
        string Name { get; }
        /// <summary>
        /// Optional description of the inventory source.
        /// </summary>
        string Description { get; }
        InventorySourceSource Source { get; }
        [JsonPropertyName("source_path")]
        string SourcePath { get; }
        /// <summary>
        /// Inventory source variables in YAML or JSON format.
        /// </summary>
        [JsonPropertyName("source_vars")]
        string SourceVars { get; }
        /// <summary>
        /// Inventory source SCM branch.
        /// Project default used if blank. Only allowed if project <c>allow_override</c> field is set to <c>true</c>.
        /// </summary>
        [JsonPropertyName("scm_branch")]
        string ScmBranch { get; }
        /// <summary>
        /// Cloud credential to use for inventory updates.
        /// </summary>
        ulong? Credential { get; }
        /// <summary>
        /// Retrieve the enabled state from the given dict of host variables.
        /// The enabled variable may be specified as <c>"foo.bar"</c>, in which case the lookup will traverse into nested dicts,
        /// equivalent to: <c>from_dict.get("foo", {}).get("bar", default)</c>
        /// </summary>
        [JsonPropertyName("enabled_var")]
        string EnabledVar { get; }
        /// <summary>
        /// Only used when <c>enabled_var</c> is set.
        /// Value when the host is considered enabled.
        /// For example if
        /// <c>enabled_var</c> = <c>"status.power_state" </c> and
        /// <c>enabled_value</c> =  <c>"powered_on"</c>
        /// with host variables:
        /// <code>
        /// {
        ///     "status": {
        ///         "power_state": "powered_on",
        ///         "created": "2018-02-01T08:00:00.000000Z:00",
        ///         "healthy": true
        ///     },
        ///     "name": "foobar",
        ///     "ip_address": "192.168.2.1"
        /// }
        /// </code>
        /// The host would be marked enabled.
        /// If <c>power_state</c> where any value other then <c>powered_on</c> then the host would be disabled when imprted.
        /// If the key is not found then the host will be enabled.
        /// </summary>
        [JsonPropertyName("enabled_value")]
        string EnabledValue { get; }
        /// <summary>
        /// This field is deprecated and will be removed in a future release.
        /// Regex where only matching hosts will be imported.
        /// </summary>
        [JsonPropertyName("host_filter")]
        string HostFilter { get; }
        /// <summary>
        /// Overwrite local groups and hosts from remote inventory source.
        /// </summary>
        bool Overwrite { get; }
        /// <summary>
        /// Overwrite local variables from remote inventory source.
        /// </summary>
        [JsonPropertyName("overwrite_vars")]
        bool OverwriteVars { get; }
        /// <summary>
        /// The amount of time (in seconds) to run before the task is canceled.
        /// </summary>
        int Timeout { get; }
        /// <summary>
        /// <list type="bullet">
        /// <item><term>0</term><description>WARNING</description></item>
        /// <item><term>1</term><description>INFO (default)</description></item>
        /// <item><term>2</term><description>DEBUG</description></item>
        /// </list>
        /// </summary>
        int Verbosity { get; }
        /// <summary>
        /// Enter host, group or pettern match.
        /// </summary>
        string Limit { get; }
        /// <summary>
        /// The container image to be used for execution.
        /// </summary>
        [JsonPropertyName("execution_environment")]
        ulong? ExecutionEnvironment { get; }
        ulong Inventory { get; }
        [JsonPropertyName("update_on_launch")]
        bool UpdateOnLaunch { get; }
        int UpdateCacheTimeout { get; }
        /// <summary>
        /// Project containing inventory file used as source.
        /// </summary>
        [JsonPropertyName("source_project")]
        ulong? SourceProject { get; }
    }

    public class InventorySource(ulong id, ResourceType type, string url, RelatedDictionary related,
                                 InventorySource.Summary summaryFields, DateTime created, DateTime? modified,
                                 string name, string description, InventorySourceSource source, string sourcePath,
                                 string sourceVars, string scmBranch, ulong? credential, string enabledVar,
                                 string enabledValue, string hostFilter, bool overwrite, bool overwriteVars,
                                 string? customVirtualenv, int timeout, int verbosity, string limit,
                                 DateTime? lastJobRun, bool lastJobFailed, DateTime? nextJobRun,
                                 JobTemplateStatus status, ulong? executionEnvironment, ulong inventory,
                                 bool updateOnLaunch, int updateCacheTimeout, ulong? sourceProject,
                                 bool lastUpdateFailed, DateTime? lastUpdated)
        : UnifiedJobTemplate(id, type, url, created, modified, name, description, lastJobRun,
                             lastJobFailed, nextJobRun, status),
          IInventorySource, IUnifiedJobTemplate, IResource<InventorySource.Summary>
    {
        public new const string PATH = "/api/v2/inventory_sources/";

        public static async Task<InventorySource> Get(ulong id)
        {
            var apiResult = await RestAPI.GetAsync<InventorySource>($"{PATH}{id}/");
            return apiResult.Contents;
        }
        public static new async IAsyncEnumerable<InventorySource> Find(NameValueCollection? query, bool getAll = false)
        {
            await foreach(var result in RestAPI.GetResultSetAsync<InventorySource>(PATH, query, getAll))
            {
                foreach (var inventorySource in result.Contents.Results)
                {
                    yield return inventorySource;
                }
            }
        }
        public record Summary(
            NameDescriptionSummary Organization,
            InventorySummary Inventory,
            [property: JsonPropertyName("execution_environment")] EnvironmentSummary? ExecutionEnvironment,
            [property: JsonPropertyName("source_project")] ProjectSummary? SourceProject,
            [property: JsonPropertyName("last_job")] LastJobSummary? LastJob,
            [property: JsonPropertyName("last_update")] LastUpdateSummary? LastUpdate,
            [property: JsonPropertyName("created_by")] UserSummary CreatedBy,
            [property: JsonPropertyName("modified_by")] UserSummary? ModifiedBy,
            [property: JsonPropertyName("user_capabilities")] Capability UserCapabilities,
            CredentialSummary[] Credentials);


        public RelatedDictionary Related { get; } = related;
        public Summary SummaryFields { get; } = summaryFields;

        public InventorySourceSource Source { get; } = source;
        public string SourcePath { get; } = sourcePath;
        public string SourceVars { get; } = sourceVars;
        public string ScmBranch { get; } = scmBranch;
        public ulong? Credential { get; } = credential;
        public string EnabledVar { get; } = enabledVar;
        public string EnabledValue { get; } = enabledValue;
        public string HostFilter { get; } = hostFilter;
        public bool Overwrite { get; } = overwrite;
        public bool OverwriteVars { get; } = overwriteVars;
        [JsonPropertyName("custom_virtualenv")]
        public string? CustomVirtualenv { get; } = customVirtualenv;
        public int Timeout { get; } = timeout;
        public int Verbosity { get; } = verbosity;
        public string Limit { get; } = limit;
        public ulong? ExecutionEnvironment { get; } = executionEnvironment;
        public ulong Inventory { get; } = inventory;
        public bool UpdateOnLaunch { get; } = updateOnLaunch;
        public int UpdateCacheTimeout { get; } = updateCacheTimeout;
        public ulong? SourceProject { get; } = sourceProject;
        [JsonPropertyName("last_update_failed")]
        public bool LastUpdateFailed { get; } = lastUpdateFailed;
        [JsonPropertyName("last_updated")]
        public DateTime? LastUpdated { get; } = lastUpdated;
    }
}
