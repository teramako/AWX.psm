using System.Collections.Specialized;
using System.Text.Json.Serialization;

namespace AWX.Resources
{
    public interface IInventory
    {
        /// <summary>
        /// Name of this inventory.
        /// </summary>
        string Name { get; }
        /// <summary>
        /// Optional description of this inventry.
        /// </summary>
        string Description { get; }
        /// <summary>
        /// Organization containing this inventory.
        /// </summary>
        ulong Organization { get; }
        /// <summary>
        /// Kind of inventory being represented.
        /// <list type="bullet">
        ///     <item>
        ///         <term><c>""</c></term>
        ///         <description>Hosts have a direct link to this inventory.(default></description>
        ///     </item>
        ///     <item>
        ///         <term><c>"smart"</c></term>
        ///         <description>Hosts for inventory generated using the host_filter property</description>
        ///     </item>
        ///     <item>
        ///         <term><c>"constructed"</c></term>
        ///         <description>Parse list of source inventories with the constructed inventory plugin.</description>
        ///     </item>
        /// </list>
        /// </summary>
        string Kind { get; }
        /// <summary>
        /// Filter that will be applied to the hosts of this inventory.
        /// </summary>
        string HostFilter { get; }
        /// <summary>
        /// Inventory variables in JSON or YAML
        /// </summary>
        string Variables { get; }
        /// <summary>
        /// If enabled, the inventory will prevent adding any organization instance groups
        /// to the list of preferred instances groups to run associated job templates on.
        /// If this setting is enabled and you provided an empty list, the global instance groups
        /// will be applied.
        /// </summary>
        [JsonPropertyName("prevent_instance_group_fallback")]
        bool PreventInstanceGroupFallback { get; }
    }

    public class Inventory(ulong id, ResourceType type, string url, RelatedDictionary related,
                           Inventory.Summary summaryFields, DateTime created, DateTime? modified, string name,
                           string description, ulong organization, string kind, string hostFilter, string variables,
                           bool hasActiveFailures, int totalHosts, int hostsWithActiveFailures, int totalGroups,
                           bool hasInventorySources, int totalInventorySources, int inventorySourcesWithFailures,
                           bool pendingDeletion, bool preventInstanceGroupFallback)
        : IInventory, IResource<Inventory.Summary>
    {
        public const string PATH = "/api/v2/inventories/";

        /// <summary>
        /// Retrieve an Inventory.<br/>
        /// API Path: <c>/api/v2/inventories/<paramref name="id"/>/</c>
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static async Task<Inventory> Get(ulong id)
        {
            var apiResult = await RestAPI.GetAsync<Inventory>($"{PATH}{id}/");
            return apiResult.Contents;
        }
        /// <summary>
        /// List Inventories.<br/>
        /// API Path: <c>/api/v2/inventories/</c>
        /// </summary>
        /// <param name="query"></param>
        /// <param name="getAll"></param>
        /// <returns></returns>
        public static async IAsyncEnumerable<Inventory> Find(NameValueCollection? query, bool getAll = false)
        {
            await foreach(var result in RestAPI.GetResultSetAsync<Inventory>(PATH, query, getAll))
            {
                foreach (var inventory in result.Contents.Results)
                {
                    yield return inventory;
                }
            }
        }
        /// <summary>
        /// List Inventories for an Organization.<br/>
        /// API Path: <c>/api/v2/organizations/<paramref name="organizationId"/>/inventories/</c>
        /// </summary>
        /// <param name="organizationId"></param>
        /// <param name="query"></param>
        /// <param name="getAll"></param>
        /// <returns></returns>
        public static async IAsyncEnumerable<Inventory> FindFromOrganization(ulong organizationId,
                                                                             NameValueCollection? query = null,
                                                                             bool getAll = false)
        {
            var path = $"{Resources.Organization.PATH}{organizationId}/inventories/";
            await foreach(var result in RestAPI.GetResultSetAsync<Inventory>(path, query, getAll))
            {
                foreach(var inventory in result.Contents.Results)
                {
                    yield return inventory;
                }
            }
        }
        /// <summary>
        /// List Inventories for an Inventory.<br/>
        /// API Path: <c>/api/v2/inventories/<paramref name="inventoryId"/>/input_inventories/</c>
        /// </summary>
        /// <param name="inventoryId"></param>
        /// <param name="query"></param>
        /// <param name="getAll"></param>
        /// <returns></returns>
        public static async IAsyncEnumerable<Inventory> FindInputInventoires(ulong inventoryId,
                                                                             NameValueCollection? query = null,
                                                                             bool getAll = false)
        {
            var path = $"{PATH}{inventoryId}/input_inventories/";
            await foreach(var result in RestAPI.GetResultSetAsync<Inventory>(path, query, getAll))
            {
                foreach(var inventory in result.Contents.Results)
                {
                    yield return inventory;
                }
            }
        }

        public record Summary(
            OrganizationSummary Organization,
            [property: JsonPropertyName("created_by")] UserSummary CreatedBy,
            [property: JsonPropertyName("modified_by")] UserSummary? ModifiedBy,
            [property: JsonPropertyName("object_roles")] Dictionary<string, NameDescriptionSummary> ObjectRoles,
            [property: JsonPropertyName("user_capabilities")] Capability UserCapabilities,
            ListSummary<LabelSummary> Labels);

        public ulong Id { get; } = id;
        public ResourceType Type { get; } = type;
        public string Url { get; } = url;
        public RelatedDictionary Related { get; } = related;
        public Summary SummaryFields { get; } = summaryFields;
        public DateTime Created { get; } = created;
        public DateTime? Modified { get; } = modified;
        public string Name { get; } = name;
        public string Description { get; } = description;
        public ulong Organization { get; } = organization;
        public string Kind { get; } = kind;
        public string HostFilter { get; } = hostFilter;
        public string Variables { get; } = variables;
        [JsonPropertyName("has_active_failures")]
        public bool HasActiveFailures { get; } = hasActiveFailures;
        [JsonPropertyName("total_hosts")]
        public int TotalHosts { get; } = totalHosts;
        [JsonPropertyName("hosts_with_active_failures")]
        public int HostsWithActiveFailures { get; } = hostsWithActiveFailures;
        [JsonPropertyName("total_groups")]
        public int TotalGroups { get; } = totalGroups;
        [JsonPropertyName("has_inventory_sources")]
        public bool HasInventorySources { get; } = hasInventorySources;
        [JsonPropertyName("total_inventory_sources")]
        public int TotalInventorySources { get; } = totalInventorySources;
        [JsonPropertyName("inventory_sources_with_failures")]
        public int InventorySourcesWithFailures { get; } = inventorySourcesWithFailures;
        [JsonPropertyName("pending_deletion")]
        public bool PendingDeletion { get; } = pendingDeletion;
        public bool PreventInstanceGroupFallback { get; } = preventInstanceGroupFallback;

        public override string ToString()
        {
            return $"[{Id}] {Name}";
        }
    }
}
