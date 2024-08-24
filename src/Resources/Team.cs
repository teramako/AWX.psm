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

        /// <summary>
        /// Retrieve a Team.<br/>
        /// API Path: <c>/api/v2/teams/<paramref name="id"/>/</c>
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static async Task<Team> Get(ulong id)
        {
            var apiResult = await RestAPI.GetAsync<Team>($"{PATH}{id}/");
            return apiResult.Contents;
        }
        /// <summary>
        /// List Teams.<br/>
        /// API Path: <c>/api/v2/teams/</c>
        /// </summary>
        /// <param name="query"></param>
        /// <param name="getAll"></param>
        /// <returns></returns>
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
        /// <summary>
        /// List Teams for an Organization.<br/>
        /// API Path: <c>/api/v2/organizations/<paramref name="organizationId"/>/teams/</c>
        /// </summary>
        /// <param name="organizationId"></param>
        /// <param name="query"></param>
        /// <param name="getAll"></param>
        /// <returns></returns>
        public static async IAsyncEnumerable<Team> FindFromOrganization(ulong organizationId,
                                                                        NameValueCollection? query = null,
                                                                        bool getAll = false)
        {
            var path = $"{Resources.Organization.PATH}{organizationId}/teams/";
            await foreach(var result in RestAPI.GetResultSetAsync<Team>(path, query, getAll))
            {
                foreach (var team in result.Contents.Results)
                {
                    yield return team;
                }
            }
        }
        /// <summary>
        /// List Teams for a User.<br/>
        /// API Path: <c>/api/v2/users/<paramref name="userId"/>/teams/</c>
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="query"></param>
        /// <param name="getAll"></param>
        /// <returns></returns>
        public static async IAsyncEnumerable<Team> FindFromUser(ulong userId,
                                                                NameValueCollection? query = null,
                                                                bool getAll = false)
        {
            var path = $"{User.PATH}{userId}/teams/";
            await foreach(var result in RestAPI.GetResultSetAsync<Team>(path, query, getAll))
            {
                foreach (var team in result.Contents.Results)
                {
                    yield return team;
                }
            }
        }
        /// <summary>
        /// List Teams for a Project.<br/>
        /// API Path: <c>/api/v2/projects/<paramref name="projectId"/>/teams/</c>
        /// </summary>
        /// <param name="projectId"></param>
        /// <param name="query"></param>
        /// <param name="getAll"></param>
        /// <returns></returns>
        public static async IAsyncEnumerable<Team> FindFromProject(ulong projectId,
                                                                   NameValueCollection? query = null,
                                                                   bool getAll = false)
        {
            var path = $"{Project.PATH}{projectId}/teams/";
            await foreach(var result in RestAPI.GetResultSetAsync<Team>(path, query, getAll))
            {
                foreach (var team in result.Contents.Results)
                {
                    yield return team;
                }
            }
        }
        /// <summary>
        /// List owner Teams for a Credential.<br/>
        /// API Path: <c>/api/v2/credentials/<paramref name="credentialId"/>/owner_teams/</c>
        /// </summary>
        /// <param name="credentialId"></param>
        /// <param name="query"></param>
        /// <param name="getAll"></param>
        /// <returns></returns>
        public static async IAsyncEnumerable<Team> FindOwnerFromCredential(ulong credentialId,
                                                                           NameValueCollection? query = null,
                                                                           bool getAll = false)
        {
            var path = $"{Credential.PATH}{credentialId}/owner_teams/";
            await foreach(var result in RestAPI.GetResultSetAsync<Team>(path, query, getAll))
            {
                foreach (var team in result.Contents.Results)
                {
                    yield return team;
                }
            }
        }
        /// <summary>
        /// List Teams for a Role.<br/>
        /// API Path: <c>/api/v2/roles/<paramref name="credentialId"/>/teams/</c>
        /// </summary>
        /// <param name="credentialId"></param>
        /// <param name="query"></param>
        /// <param name="getAll"></param>
        /// <returns></returns>
        public static async IAsyncEnumerable<Team> FindFromRole(ulong credentialId,
                                                                NameValueCollection? query = null,
                                                                bool getAll = false)
        {
            var path = $"{Role.PATH}{credentialId}/teams/";
            await foreach(var result in RestAPI.GetResultSetAsync<Team>(path, query, getAll))
            {
                foreach (var team in result.Contents.Results)
                {
                    yield return team;
                }
            }
        }

        public record Summary(
            OrganizationSummary Organization,
            [property: JsonPropertyName("created_by")] UserSummary CreatedBy,
            [property: JsonPropertyName("modified_by")] UserSummary? ModifiedBy,
            [property: JsonPropertyName("object_roles")] Dictionary<string, ObjectRoleSummary> ObjectRoles,
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

