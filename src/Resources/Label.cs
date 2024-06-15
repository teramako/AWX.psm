using System.Collections.Specialized;
using System.Text.Json.Serialization;

namespace AWX.Resources
{
    public interface ILabel
    {
        /// <summary>
        /// Name of the label.
        /// </summary>
        string Name { get; }
        /// <summary>
        /// Organization this label belongs to.
        /// </summary>
        ulong Organization { get; }
    }

    public class Label(ulong id,
                       ResourceType type,
                       string url,
                       RelatedDictionary related,
                       Label.Summary summaryFields,
                       DateTime created,
                       DateTime? modified,
                       string name,
                       ulong organization)
        : ILabel, IResource<Label.Summary>
    {
        public const string PATH = "/api/v2/labels/";
        public static async Task<Label> Get(ulong id)
        {
            var apiResult = await RestAPI.GetAsync<Label>($"{PATH}{id}/");
            return apiResult.Contents;
        }
        public static async IAsyncEnumerable<Label> Find(NameValueCollection? query, bool getAll = false)
        {
            await foreach(var result in RestAPI.GetResultSetAsync<Label>(PATH, query, getAll))
            {
                foreach (var app in result.Contents.Results)
                {
                    yield return app;
                }
            }
        }
        public record Summary(
            NameDescriptionSummary Organization,
            [property: JsonPropertyName("created_by")] UserSummary? CreatedBy,
            [property: JsonPropertyName("modified_by")] UserSummary? ModifiedBy);


        public ulong Id { get; } = id;
        public ResourceType Type { get; } = type;
        public string Url { get; } = url;
        public RelatedDictionary Related { get; } = related;
        public Summary SummaryFields { get; } = summaryFields;

        public DateTime Created { get; } = created;
        public DateTime? Modified { get; } = modified;
        public string Name { get; } = name;
        public ulong Organization { get; } = organization;
    }
}
