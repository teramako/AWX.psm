using System.Collections.Specialized;
using System.Text.Json.Serialization;

namespace AnsibleTower.Resources
{
    public class Role(ulong id,
                      ResourceType type,
                      string url,
                      RelatedDictionary related,
                      Role.Summary summaryFields,
                      string name,
                      string description)
                : IResource<Role.Summary>
    {
        public const string PATH = "/api/v2/roles/";
        public static async Task<Role> Get(ulong id)
        {
            var apiResult = await RestAPI.GetAsync<Role>($"{PATH}{id}/");
            return apiResult.Contents;
        }
        public static async IAsyncEnumerable<Role> Find(NameValueCollection? query, bool getAll = false)
        {
            await foreach(var result in RestAPI.GetResultSetAsync<Role>(PATH, query, getAll))
            {
                foreach (var app in result.Contents.Results)
                {
                    yield return app;
                }
            }
        }
        public record Summary(
            [property: JsonPropertyName("resource_name")] string ResourceName,
            [property: JsonPropertyName("resource_type")] string ResourceType,
            [property: JsonPropertyName("resource_type_display_name")] string ResourceTypeDisplayName,
            [property: JsonPropertyName("resource_id")] ulong ResourceId);


        public ulong Id { get; } = id;
        public ResourceType Type { get; } = type;
        public string Url { get; } = url;
        public RelatedDictionary Related { get; } = related;
        public Summary SummaryFields { get; } = summaryFields;

        public string Name { get; } = name;
        public string Description { get; } = description;
    }
}
