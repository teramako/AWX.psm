using System.Collections.Specialized;

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
        /// <summary>
        /// Retrieve a Label.<br/>
        /// API Path: <c>/api/v2/labels/<paramref name="id"/>/</c>
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static async Task<Label> Get(ulong id)
        {
            var apiResult = await RestAPI.GetAsync<Label>($"{PATH}{id}/");
            return apiResult.Contents;
        }
        /// <summary>
        /// List Labels.<br/>
        /// API Path: <c>/api/v2/labels/</c>
        /// </summary>
        /// <param name="query"></param>
        /// <param name="getAll"></param>
        /// <returns></returns>
        public static async IAsyncEnumerable<Label> Find(NameValueCollection? query, bool getAll = false)
        {
            await foreach(var result in RestAPI.GetResultSetAsync<Label>(PATH, query, getAll))
            {
                foreach (var label in result.Contents.Results)
                {
                    yield return label;
                }
            }
        }
        public record Summary(OrganizationSummary Organization,
                              UserSummary? CreatedBy,
                              UserSummary? ModifiedBy);


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
