using System.Collections.Specialized;
using System.Text.Json.Serialization;

namespace AWX.Resources
{
    public interface ITeam
    {
        /// <summary>
        /// Name of this team.
        /// </summary>
        string Name { get; }
        /// <summary>
        /// Optional description of this team.
        /// </summary>
        string Description { get; }
        /// <summary>
        /// The organization ID to which this team belongs.
        /// </summary>
        ulong Organization { get; }
    }

    public class Team(ulong id,
                      ResourceType type,
                      string url,
                      RelatedDictionary related,
                      Team.Summary summaryFields,
                      DateTime created,
                      DateTime? modified,
                      string name,
                      string description,
                      ulong organization)
        : ITeam, IResource<Team.Summary>
    {
        public const string PATH = "/api/v2/teams/";

        public static async Task<Team> Get(ulong id)
        {
            var apiResult = await RestAPI.GetAsync<Team>($"{PATH}{id}/");
            return apiResult.Contents;
        }
        public static async IAsyncEnumerable<Team> Find(NameValueCollection? query, bool getAll = false)
        {
            await foreach(var result in RestAPI.GetResultSetAsync<Team>(PATH, query, getAll))
            {
                foreach (var team in result.Contents.Results)
                {
                    yield return team;
                }
            }
        }
        public record Summary(
            NameDescriptionSummary Organization,
            [property: JsonPropertyName("created_by")] UserSummary CreatedBy,
            [property: JsonPropertyName("modified_by")] UserSummary? ModifiedBy,
            [property: JsonPropertyName("object_roles")] Dictionary<string, NameDescriptionSummary> ObjectRoles,
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
        public ulong Organization { get; } = organization;
    }
}

