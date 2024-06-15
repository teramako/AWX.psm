using System.Collections.Specialized;
using System.Text.Json.Serialization;

namespace AWX.Resources
{
    public interface IGroup
    {
        /// <summary>
        /// Name of this group.
        /// </summary>
        string Name { get; }
        /// <summary>
        /// Optional description of this group.
        /// </summary>
        string Description { get; }
        /// <summary>
        /// Inventory ID
        /// </summary>
        ulong Inventory { get; }
        /// <summary>
        /// Group variables in JSON or YAML format.
        /// </summary>
        string Variables { get; }
    }
    public class Group(ulong id,
                       ResourceType type,
                       string url,
                       RelatedDictionary related,
                       Group.Summary summaryFields,
                       DateTime created,
                       DateTime? modified,
                       string name,
                       string description,
                       ulong inventory,
                       string variables)
        : IGroup, IResource<Group.Summary>
    {
        public const string PATH = "/api/v2/groups/";

        public static async Task<Group> Get(ulong id)
        {
            var apiResult = await RestAPI.GetAsync<Group>($"{PATH}{id}/");
            return apiResult.Contents;
        }
        public static async IAsyncEnumerable<Group> Find(NameValueCollection? query, bool getAll = false)
        {
            await foreach(var result in RestAPI.GetResultSetAsync<Group>(PATH, query, getAll))
            {
                foreach (var group in result.Contents.Results)
                {
                    yield return group;
                }
            }
        }
        public record Summary(
            InventorySummary Inventory,
            [property: JsonPropertyName("created_by")] UserSummary? CreatedBy,
            [property: JsonPropertyName("modified_by")] UserSummary? ModifiedBy,
            [property: JsonPropertyName("user_capabilities")] Capability UserCapabilities);


        public ulong Id { get; } = id;
        public ResourceType Type { get; } = type;
        public string Url { get; } = url;
        public RelatedDictionary Related { get; } = related;
        public Summary SummaryFields { get; } = summaryFields;
        public DateTime Created { get; } = created;
        public DateTime? Modified { get; } = modified;
        public string Name { get; } = name;
        public string Description { get; } = description;
        public ulong Inventory { get; } = inventory;
        public string Variables { get; } = variables;
    }
}
