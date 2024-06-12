using System.Collections.Specialized;
using System.Text.Json.Serialization;

namespace AnsibleTower.Resources
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
        [JsonPropertyName("notification_type")]
        NotificationType NotificationType { get; }
        [JsonPropertyName("notification_configuration")]
        OrderedDictionary NotificationConfiguration { get; }
        NotificationMessages? Messages { get; }
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
                                      OrderedDictionary notificationConfiguration,
                                      NotificationMessages? messages)
                : INotificationTemplate, IResource<NotificationTemplate.Summary>
    {
        public const string PATH = "/api/v2/notification_templates/";
        public static async Task<NotificationTemplate> Get(ulong id)
        {
            var apiResult = await RestAPI.GetAsync<NotificationTemplate>($"{PATH}{id}/");
            return apiResult.Contents;
        }
        public static async IAsyncEnumerable<NotificationTemplate> Find(NameValueCollection? query, bool getAll = false)
        {
            await foreach(var result in RestAPI.GetResultSetAsync<NotificationTemplate>(PATH, query, getAll))
            {
                foreach (var app in result.Contents.Results)
                {
                    yield return app;
                }
            }
        }
        public record Summary(
            NameDescriptionSummary Organization,
            [property: JsonPropertyName("created_by")] UserSummary CreatedBy,
            [property: JsonPropertyName("modified_by")] UserSummary ModifiedBy,
            [property: JsonPropertyName("user_capabilities")] Capability UserCapabilities,
            [property: JsonPropertyName("recent_notifications")] RecentNotificationSummary[] RecentNotification);


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
        public OrderedDictionary NotificationConfiguration { get; } = notificationConfiguration;
        public NotificationMessages? Messages { get; } = messages;
    }

    public class NotificationMessages(NotificationMessage error,
                                      NotificationMessage started,
                                      NotificationMessage success,
                                      NotificationWorkflowApprovalMessages workflowApproval)
    {
        public NotificationMessage Error { get; } = error;
        public NotificationMessage Started { get; } = started;
        public NotificationMessage Success { get; } = success;
        [JsonPropertyName("workflow_approval")]
        public NotificationWorkflowApprovalMessages WorkflowApproval { get; } = workflowApproval;
    }

    public class NotificationWorkflowApprovalMessages(NotificationMessage denied,
                                                      NotificationMessage running,
                                                      NotificationMessage approved,
                                                      NotificationMessage timedOut)
    {
        public NotificationMessage Denied { get; } = denied;
        public NotificationMessage Running { get; } = running;
        public NotificationMessage Approved { get; } = approved;
        [JsonPropertyName("timed_out")]
        public NotificationMessage TimedOut { get; } = timedOut;
    }

    public class NotificationMessage(string body, string? message)
    {
        public string Body { get; } = body;
        public string? Message { get; } = message;
    }
}
