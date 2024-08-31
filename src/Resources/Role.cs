using System.Collections.Specialized;

namespace AWX.Resources
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
        /// <summary>
        /// Retrieve a Role.<br/>
        /// API Path: <c>/api/v2/roles/<paramref name="id"/>/</c>
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static async Task<Role> Get(ulong id)
        {
            var apiResult = await RestAPI.GetAsync<Role>($"{PATH}{id}/");
            return apiResult.Contents;
        }
        /// <summary>
        /// List Roles.<br/>
        /// API Path: <c>/api/v2/roles/</c>
        /// </summary>
        /// <param name="query"></param>
        /// <param name="getAll"></param>
        /// <returns></returns>
        public static async IAsyncEnumerable<Role> Find(NameValueCollection? query, bool getAll = false)
        {
            await foreach(var result in RestAPI.GetResultSetAsync<Role>(PATH, query, getAll))
            {
                foreach (var role in result.Contents.Results)
                {
                    yield return role;
                }
            }
        }
        public record Summary(string? ResourceName,
                              ResourceType? ResourceType,
                              string? ResourceTypeDisplayName,
                              ulong? ResourceId);


        public ulong Id { get; } = id;
        public ResourceType Type { get; } = type;
        public string Url { get; } = url;
        public RelatedDictionary Related { get; } = related;
        public Summary SummaryFields { get; } = summaryFields;

        public string Name { get; } = name;
        public string Description { get; } = description;
    }
}
