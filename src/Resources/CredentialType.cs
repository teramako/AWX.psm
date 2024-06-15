using System.Collections.Specialized;
using System.Text.Json.Serialization;

namespace AWX.Resources
{
    public interface ICredentialType
    {
        /// <summary>
        /// Name of this credential type.
        /// </summary>
        string Name { get; }
        /// <summary>
        /// Optional description of this credential type.
        /// </summary>
        string Description { get; }
        string Kind { get; }
        OrderedDictionary Inputs { get; }
        OrderedDictionary Injectors { get; }
    }

    public class CredentialType(ulong id,
                                ResourceType type,
                                string url,
                                RelatedDictionary related,
                                CredentialType.Summary summaryFields,
                                DateTime created,
                                DateTime? modified,
                                string name,
                                string description,
                                string kind,
                                string nameSpace,
                                bool managed,
                                OrderedDictionary inputs,
                                OrderedDictionary injectors)
        : ICredentialType, IResource<CredentialType.Summary>
    {
        public const string PATH = "/api/v2/credential_types/";

        public static async Task<CredentialType> Get(ulong id)
        {
            var apiResult = await RestAPI.GetAsync<CredentialType>($"{PATH}{id}/");
            return apiResult.Contents;
        }
        public static async IAsyncEnumerable<CredentialType> Find(NameValueCollection? query, bool getAll = false)
        {
            await foreach(var result in RestAPI.GetResultSetAsync<CredentialType>(PATH, query, getAll))
            {
                foreach (var credentialType in result.Contents.Results)
                {
                    yield return credentialType;
                }
            }
        }
    public record Summary(
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
        public string Kind { get; } = kind;
        public string Namespace { get; } = nameSpace;
        public bool Managed { get; } = managed;
        public OrderedDictionary Inputs { get; } = inputs;
        public OrderedDictionary Injectors { get; } = injectors;
    }
}

