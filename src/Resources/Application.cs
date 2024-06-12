using System.Collections.Specialized;
using System.Text.Json.Serialization;

namespace AnsibleTower.Resources
{
    public enum ApplicationClientType
    {
        Confidential,
        Public
    }

    public interface IApplication
    {
        /// <summary>
        /// Name of this application.
        /// </summary>
        string Name { get; }
        /// <summary>
        /// Optional description of this application.
        /// </summary>
        string Description { get; }
        /// <summary>
        /// Set to <c>Public</c> or <c>Confidential</c> depending on how secure the client device is.
        /// </summary>
        [JsonPropertyName("client_type")]
        ApplicationClientType ClientType { get; }
        /// <summary>
        /// Allowed URIs list, space spareted.
        /// </summary>
        [JsonPropertyName("redirect_uris")]
        string RedirectUris { get; }
        /// <summary>
        /// The Grant type the user must use for acquire tokens for this application.
        /// </summary>
        [JsonPropertyName("authorization_grant_type")]
        string AuthorizationGrantType { get; }
        /// <summary>
        /// Set <c>True</c> to skip authorization step for completely trusted applications.
        /// </summary>
        [JsonPropertyName("skip_authorization")]
        bool SkipAuthorization { get; }
        /// <summary>
        /// Organization containing thie application.
        /// </summary>
        ulong Organization { get; }
    }

    [ResourceType(ResourceType.OAuth2Application)]
    public class Application(ulong id,
                             ResourceType type,
                             string url,
                             RelatedDictionary related,
                             Application.Summary summaryFields,
                             DateTime created,
                             DateTime? modified,
                             string name,
                             string description,
                             string clientId,
                             ApplicationClientType clientType,
                             string? clientSecret,
                             string redirectUris,
                             string authorizationGrantType,
                             bool skipAuthorization,
                             ulong organization)
        : IApplication, IResource<Application.Summary>
    {
        public const string PATH = "/api/v2/applications/";
        public static async Task<Application> Get(ulong id)
        {
            var apiResult = await RestAPI.GetAsync<Application>($"{PATH}{id}/");
            return apiResult.Contents;
        }
        public static async IAsyncEnumerable<Application> Find(NameValueCollection? query, bool getAll = false)
        {
            await foreach (var result in RestAPI.GetResultSetAsync<Application>(PATH, query, getAll))
            {
                foreach (var app in result.Contents.Results)
                {
                    yield return app;
                }
            }
        }
        public record Summary(NameDescriptionSummary Organization,
                                               [property: JsonPropertyName("user_capabilities")] Capability UserCapabilities,
                                               ListSummary<TokenSummary> Tokens);

        public ulong Id { get; } = id;
        public ResourceType Type { get; } = type;
        public string Url { get; } = url;
        public RelatedDictionary Related { get; } = related;
        public Summary SummaryFields { get; } = summaryFields;
        public DateTime Created { get; } = created;
        public DateTime? Modified { get; } = modified;
        public string Name { get; } = name;
        public string Description { get; } = description;
        [JsonPropertyName("client_id")]
        public string ClientId { get; } = clientId;
        [JsonConverter(typeof(Json.EnumUpperCamelCaseStringConverter<ApplicationClientType>))]
        public ApplicationClientType ClientType { get; } = clientType;
        [JsonPropertyName("client_secret")]
        public string? ClientSecret { get; } = clientSecret;
        public string RedirectUris { get; } = redirectUris;
        public string AuthorizationGrantType { get; } = authorizationGrantType;
        public bool SkipAuthorization { get; } = skipAuthorization;
        public ulong Organization { get; } = organization;
    }
}
