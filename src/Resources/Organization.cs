using System.Collections.Specialized;
using System.Text.Json.Serialization;

namespace AWX.Resources
{
    public interface IOrganization
    {
        string Name { get; }
        string Description { get; }
        [JsonPropertyName("max_hosts")]
        int MaxHosts { get; }
        [JsonPropertyName("default_environment")]
        int? DefaultEnvironment { get; }
    }

    [ResourceType(ResourceType.Organization)]
    public class Organization(ulong id,
                              ResourceType type,
                              string url,
                              RelatedDictionary related,
                              Organization.Summary summaryFields,
                              DateTime created,
                              DateTime? modified,
                              string name,
                              string description,
                              int maxHosts,
                              string? customVirtualenv,
                              int? defaultEnvironment)
        : IOrganization, IResource<Organization.Summary>
    {
        public const string PATH = "/api/v2/organizations/";
        public static async Task<Organization> Get(ulong id)
        {
            var apiResult = await RestAPI.GetAsync<Organization>($"{PATH}{id}/");
            return apiResult.Contents;
        }
        public static async IAsyncEnumerable<Organization> Find(NameValueCollection? query, bool getAll = false)
        {
            await foreach (var result in RestAPI.GetResultSetAsync<Organization>(PATH, query, getAll))
            {
                foreach (var app in result.Contents.Results)
                {
                    yield return app;
                }
            }
        }
        public record Summary(
            [property: JsonPropertyName("default_environment")] EnvironmentSummary? DefaultEnvironment,
            [property: JsonPropertyName("created_by")] UserSummary CreatedBy,
            [property: JsonPropertyName("modified_by")] UserSummary ModifiedBy,
            [property: JsonPropertyName("object_roles")] Dictionary<string, RoleSummary> ObjectRoles,
            [property: JsonPropertyName("user_capabilities")] Capability UserCapabilities,
            [property: JsonPropertyName("related_field_counts")] RelatedFieldCountsSummary RelatedFieldCounts);


        public ulong Id { get; } = id;
        public ResourceType Type { get; } = type;
        public string Url { get; } = url;
        public RelatedDictionary Related { get; } = related;
        public Summary SummaryFields { get; } = summaryFields;
        public DateTime Created { get; } = created;
        public DateTime? Modified { get; } = modified;
        public string Name { get; } = name;
        public string Description { get; } = description;
        public int MaxHosts { get; } = maxHosts;
        [JsonPropertyName("custom_virtualenv")]
        public string? CustomVirtualenv { get; } = customVirtualenv;
        public int? DefaultEnvironment { get; } = defaultEnvironment;
    }
}
