using System.Collections.Specialized;
using System.Text.Json.Serialization;

namespace AWX.Resources
{
    [JsonConverter(typeof(Json.EnumUpperCamelCaseStringConverter<NotificationType>))]
    public enum NotificationType
    {
        Email,
        Grafana,
        IRC,
        Mattemost,
        Pagerduty,
        RoketChat,
        Slack,
        Twillo,
        Webhook
    }

    public interface INotificationTemplate
    {
        /// <summary>
        /// Name of this notification template.
        /// </summary>
        string Name { get; }
        /// <summary>
        /// Optional description of this notification template.
        /// </summary>
        string Description { get; }
        /// <summary>
        /// Organization ID
        /// </summary>
        ulong Organization { get; }
        NotificationType NotificationType { get; }
        Dictionary<string, object?> NotificationConfiguration { get; }
        Messages? Messages { get; }
    }

    public class NotificationTemplate(ulong id,
                                      ResourceType type,
                                      string url,
                                      RelatedDictionary related,
                                      NotificationTemplate.Summary summaryFields,
                                      DateTime created,
                                      DateTime? modified,
                                      string name,
                                      string description,
                                      ulong organization,
                                      NotificationType notificationType,
                                      Dictionary<string, object?> notificationConfiguration,
                                      Messages? messages)
                : INotificationTemplate, IResource<NotificationTemplate.Summary>
    {
        public const string PATH = "/api/v2/notification_templates/";
        /// <summary>
        /// Retrieve a Notification Template.<br/>
        /// API Path: <c>/api/v2/notification_templates/<paramref name="id"/>/</c>
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static async Task<NotificationTemplate> Get(ulong id)
        {
            var apiResult = await RestAPI.GetAsync<NotificationTemplate>($"{PATH}{id}/");
            return apiResult.Contents;
        }
        /// <summary>
        /// List Notification Templates.<br/>
        /// API Path: <c>/api/v2/notification_templates/</c>
        /// </summary>
        /// <param name="query"></param>
        /// <param name="getAll"></param>
        /// <returns></returns>
        public static async IAsyncEnumerable<NotificationTemplate> Find(NameValueCollection? query, bool getAll = false)
        {
            await foreach(var result in RestAPI.GetResultSetAsync<NotificationTemplate>(PATH, query, getAll))
            {
                foreach (var notificationTemplate in result.Contents.Results)
                {
                    yield return notificationTemplate;
                }
            }
        }
        public record Summary(OrganizationSummary Organization,
                              UserSummary CreatedBy,
                              UserSummary ModifiedBy,
                              Capability UserCapabilities,
                              RecentNotificationSummary[] RecentNotifications);


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
        public NotificationType NotificationType { get; } = notificationType;
        public Dictionary<string, object?> NotificationConfiguration { get; } = notificationConfiguration;
        public Messages? Messages { get; } = messages;
    }

    public record Messages(
        NMessage Error,
        NMessage Started,
        NMessage Success,
        ApprovalMessages WorkflowApproval
    );

    public record ApprovalMessages(
        NMessage Denied,
        NMessage Running,
        NMessage Approved,
        NMessage TimedOut
    );

    public record NMessage(
        string Body,
        string? Message
    );
}
