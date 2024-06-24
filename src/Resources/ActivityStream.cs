using System.Collections.Specialized;
using System.Text.Json.Serialization;

namespace AWX.Resources
{
    [JsonConverter(typeof(Json.EnumUpperCamelCaseStringConverter<ActivityStreamOperation>))]
    public enum ActivityStreamOperation
    {
        Create,
        Update,
        Delete,
        Associate,
        Disassociate,
    }

    [ResourceType(ResourceType.ActivityStream)]
    public class ActivityStream(ulong id,
                          ResourceType type,
                          string url,
                          RelatedDictionary related,
                          ActivityStream.Summary summaryFields,
                          DateTime timestamp,
                          ActivityStreamOperation operation,
                          Dictionary<string, object?> changes,
                          string object1,
                          string object2,
                          string objectAssociation,
                          string actionNode,
                          string objectType)
        : IResource<ActivityStream.Summary>
    {
        public const string PATH = "/api/v2/activity_stream/";

        /// <summary>
        /// Retrieve an Activity Stream.<br/>
        /// API Path: <c>/api/v2/activity_stream/<paramref name="id"/>/</c>
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static async Task<ActivityStream> Get(ulong id)
        {
            var apiResult = await RestAPI.GetAsync<ActivityStream>($"{PATH}{id}/");
            return apiResult.Contents;
        }
        /// <summary>
        /// List Activity Sterams.<br/>
        /// API Path: <c>/api/v2/activity_stream/</c>
        /// </summary>
        /// <param name="query"></param>
        /// <param name="getAll"></param>
        /// <returns></returns>
        public static async IAsyncEnumerable<ActivityStream> Find(NameValueCollection? query, bool getAll = false)
        {
            await foreach(var result in RestAPI.GetResultSetAsync<ActivityStream>(PATH, query, getAll))
            {
                foreach (var activity in result.Contents.Results)
                {
                    yield return activity;
                }
            }
        }
        public class Summary(UserSummary? actor)
        {
            public UserSummary? Actor { get; set; } = actor;
            [JsonExtensionData]
            public Dictionary<string, object>? ExtensionData { get; set; }
        }

        public ulong Id { get; } = id;

        public ResourceType Type { get; } = type;
        public string Url { get; } = url;
        public RelatedDictionary Related { get; } = related;
        public Summary SummaryFields { get; } = summaryFields;

        [JsonPropertyOrder(10)]
        public DateTime Timestamp { get; } = timestamp;
        [JsonPropertyOrder(11)]
        public ActivityStreamOperation Operation { get; } = operation;
        [JsonPropertyOrder(12)]
        public Dictionary<string, object?> Changes { get; } = changes;
        [JsonPropertyOrder(13)]
        public string Object1 { get; } = object1;
        [JsonPropertyOrder(14)]
        public string Object2 { get; } = object2;
        [JsonPropertyOrder(15)]
        [JsonPropertyName("object_association")]
        public string ObjectAssociation { get; } = objectAssociation;
        [JsonPropertyOrder(16)]
        [JsonPropertyName("action_node")]
        public string ActionNode { get; } = actionNode;
        [JsonPropertyOrder(17)]
        [JsonPropertyName("object_type")]
        public string ObjectType { get; } = objectType;
    }
}
