using System.Collections.Specialized;
using System.Text.Json.Serialization;

namespace AnsibleTower.Resources
{
    public interface IUser
    {
        string Username { get; }
        [JsonPropertyName("first_name")]
        string FirstName { get; }
        [JsonPropertyName("last_name")]
        string LastName { get; }
        string Email { get; }
        [JsonPropertyName("is_superuser")]
        bool IsSuperuser { get; }
        [JsonPropertyName("is_system_auditor")]
        bool IsSystemAuditor { get; }
        string Password { get; }
    }

    [ResourceType(ResourceType.User)]
    public class User(ulong id,
                      ResourceType type,
                      string url,
                      RelatedDictionary related,
                      User.Summary summaryFields,
                      DateTime created,
                      DateTime? modified,
                      string username,
                      string firstName,
                      string lastName,
                      string email,
                      bool isSuperuser,
                      bool isSystemAuditor,
                      string password,
                      string ldapDn,
                      DateTime? lastLogin,
                      string externalAccount,
                      string[] auth)
        : IUser, IResource<User.Summary>
    {
        public const string PATH = "/api/v2/users/";
        /// <summary>
        /// Retrieve information about the current User.
        /// </summary>
        /// <returns></returns>
        public static async Task<User> GetMe()
        {
            var apiResult = await RestAPI.GetAsync<ResultSet<User>>("/api/v2/me/");
            return apiResult.Contents.Results.Single();
        }
        /// <summary>
        /// Retrieve a User for the <paramref name="id"/>
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static async Task<User> Get(ulong id)
        {
            var apiResult = await RestAPI.GetAsync<User>($"{PATH}{id}/");
            return apiResult.Contents;
        }
        public static async IAsyncEnumerable<User> Find(NameValueCollection? query, bool getAll = false)
        {
            await foreach (var result in RestAPI.GetResultSetAsync<User>(PATH, query, getAll))
            {
                foreach (var app in result.Contents.Results)
                {
                    yield return app;
                }
            }
        }
        public record Summary([property: JsonPropertyName("user_capabilities")] Capability UserCapabilities);

        public ulong Id { get; } = id;
        public ResourceType Type { get; } = type;
        public string Url { get; } = url;
        public RelatedDictionary Related { get; } = related;
        public Summary SummaryFields { get; } = summaryFields;

        public DateTime Created { get; } = created;
        public DateTime? Modified { get; } = modified;
        public string Username { get; } = username;
        public string FirstName { get; } = firstName;
        public string LastName { get; } = lastName;
        public string Email { get; } = email;
        public bool IsSuperuser { get; } = isSuperuser;
        public bool IsSystemAuditor { get; } = isSystemAuditor;
        public string Password { get; } = password;
        [JsonPropertyName("ldap_dn")]
        public string LdapDn { get; } = ldapDn;
        [JsonPropertyName("last_login")]
        public DateTime? LastLogin { get; } = lastLogin;
        [JsonPropertyName("external_account")]
        public string? ExternalAccount { get; } = externalAccount;
        [JsonPropertyOrder(22)]
        public string[] Auth { get; } = auth;

        public UserData ToData()
        {
            return new UserData()
            {
                Username = Username,
                FirstName = FirstName,
                LastName = LastName,
                Email = Email,
                IsSuperuser = IsSuperuser,
                IsSystemAuditor = IsSystemAuditor,
                Password = Password,
            };
        }
    }

    public struct UserData
    {
        public string? Username { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Email { get; set;  }
        public bool? IsSuperuser { get; set; }
        public bool? IsSystemAuditor { get; set; }
        public string?  Password { get; set; }
    }
}
