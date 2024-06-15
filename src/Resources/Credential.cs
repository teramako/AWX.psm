using System.Collections.Specialized;
using System.Text.Json.Serialization;

namespace AWX.Resources
{
    public interface ICredential
    {
        /// <summary>
        /// Name of this credential.
        /// </summary>
        string Name { get; }
        /// <summary>
        /// Optional description of this credential.
        /// </summary>
        string Description { get; }
        ulong? Organization { get; }
        /// <summary>
        /// Specify the type of credential you want to create.
        /// Refer to the documentaion for detail on each type.
        /// </summary>
        [JsonPropertyName("credential_type")]
        ulong CredentialType { get; }
        /// <summary>
        /// Enter inputs using either JSON or YAML syntax.
        /// Refer to the documentaion for example syntax.
        /// </summary>
        OrderedDictionary Inputs { get; }
    }


    public class Credential(ulong id,
                            ResourceType type,
                            string url,
                            RelatedDictionary related,
                            Credential.Summary summaryFields,
                            DateTime created,
                            DateTime? modified,
                            string name,
                            string description,
                            ulong? organization,
                            ulong credentialType,
                            bool managed,
                            OrderedDictionary inputs,
                            string kind,
                            bool cloud,
                            bool kubernetes)
        : ICredential, IResource<Credential.Summary>
    {
        public const string PATH = "/api/v2/credentials/";

        public static async Task<Credential> Get(ulong id)
        {
            var apiResult = await RestAPI.GetAsync<Credential>($"{PATH}{id}/");
            return apiResult.Contents;
        }
        public static async IAsyncEnumerable<Credential> Find(NameValueCollection? query, bool getAll = false)
        {
            await foreach(var result in RestAPI.GetResultSetAsync<Credential>(PATH, query, getAll))
            {
                foreach (var app in result.Contents.Results)
                {
                    yield return app;
                }
            }
        }
        public record Summary(
            NameDescriptionSummary? Organization,
            [property: JsonPropertyName("credential_type")] NameDescriptionSummary CredentialType,
            [property: JsonPropertyName("created_by")] UserSummary CreatedBy,
            [property: JsonPropertyName("modified_by")] UserSummary? ModifiedBy,
            [property: JsonPropertyName("object_roles")] Dictionary<string, NameDescriptionSummary> ObjectRoles,
            [property: JsonPropertyName("user_capabilities")] Capability UserCapabilities,
            OwnerSummary[] Owners);


        public ulong Id { get; } = id;
        public ResourceType Type { get; } = type;
        public string Url { get; } = url;
        public RelatedDictionary Related { get; } = related;
        public Summary SummaryFields { get; } = summaryFields;
        public DateTime Created { get; } = created;
        public DateTime? Modified { get; } = modified;
        public string Name { get; } = name;
        public string Description { get; } = description;
        public ulong? Organization { get; } = organization;
        public ulong CredentialType { get; } = credentialType;
        public bool Managed { get; } = managed;

        public OrderedDictionary Inputs { get; } = inputs;
        public string Kind { get; } = kind;
        public bool Cloud { get; } = cloud;
        public bool Kubernetes { get; } = kubernetes;
    }
}
