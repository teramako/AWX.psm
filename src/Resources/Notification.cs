using System.Collections.Specialized;
using System.Text.Json.Serialization;

namespace AnsibleTower.Resources
{
    public interface INotification
    {
        DateTime Created { get; }
        DateTime? Modified { get; }
        [JsonPropertyName("notification_template")]
        ulong NotificationTemplate { get; }
        string Error { get; }
        JobStatus Status { get; }
        [JsonPropertyName("notifications_sent")]
        int NotificationsSent { get; }
        [JsonPropertyName("notification_type")]
        NotificationType NotificationType { get; }
        string Recipients { get; }
        string Subject { get; }
        string? Body { get; }

    }


    public class Notification(ulong id, ResourceType type, string url, RelatedDictionary related,
                              Notification.Summary summaryFields, DateTime created, DateTime? modified,
                              ulong notificationTemplate, string error, JobStatus status, int notificationsSent,
                              NotificationType notificationType, string recipients, string subject, string? body)
                : INotification, IResource<Notification.Summary>
    {
        public const string PATH = "/api/v2/notifications/";
        public static async Task<Notification> Get(ulong id)
        {
            var apiResult = await RestAPI.GetAsync<Notification>($"{PATH}{id}/");
            return apiResult.Contents;
        }
        public static async IAsyncEnumerable<Notification> Find(NameValueCollection? query, bool getAll = false)
        {
            await foreach(var result in RestAPI.GetResultSetAsync<Notification>(PATH, query, getAll))
            {
                foreach (var app in result.Contents.Results)
                {
                    yield return app;
                }
            }
        }
        public record Summary(
            [property: JsonPropertyName("notification_template")] NameDescriptionSummary NotificationTemplate);


        public ulong Id { get; } = id;
        public ResourceType Type { get; } = type;
        public string Url { get; } = url;
        public RelatedDictionary Related { get; } = related;
        public Summary SummaryFields { get; } = summaryFields;

        public DateTime Created { get; } = created;
        public DateTime? Modified { get; } = modified;
        public ulong NotificationTemplate { get; } = notificationTemplate;
        public string Error { get; } = error;
        public JobStatus Status { get; } = status;
        public int NotificationsSent { get; } = notificationsSent;
        public NotificationType NotificationType { get; } = notificationType;
        public string Recipients { get; } = recipients;
        public string Subject { get; } = subject;
        public string? Body { get; } = body;
    }
}
