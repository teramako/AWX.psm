using System.Collections.Specialized;
using System.Text.Json.Serialization;

namespace AWX.Resources
{
    public interface ISchedule
    {
        string Name { get; }
        string Description { get; }
        string Rrule { get; }
        [JsonPropertyName("extra_data")]
        OrderedDictionary ExtraData { get; }
        ulong? Inventory { get; }
        [JsonPropertyName("scm_branch")]
        string? ScmBranch { get; }
        [JsonPropertyName("job_type")]
        string? JobType { get; }
        [JsonPropertyName("job_tags")]
        string? JobTags { get; }
        [JsonPropertyName("skip_tags")]
        string? SkipTags { get; }
        string? Limit { get; }
        [JsonPropertyName("diff_mode")]
        bool? DiffMode { get; }
        JobVerbosity? Verbosity { get; }
        [JsonPropertyName("execution_environment")]
        ulong? ExecutionEnvironment { get; }
        int? Forks { get; }
        [JsonPropertyName("job_slice_count")]
        int? JobSliceCount { get; }
        int? Timeout { get; }
        [JsonPropertyName("unified_job_template")]
        ulong UnifiedJobTemplate { get; }
        bool Enabled { get; }

    }

    public class Schedule(string rrule, ulong id, ResourceType type, string url, RelatedDictionary related,
                          Schedule.Summary summaryFields, DateTime created, DateTime? modified, string name,
                          string description, OrderedDictionary extraData, ulong? inventory, string? scmBranch,
                          string? jobType, string? jobTags, string? skipTags, string? limit, bool? diffMode,
                          JobVerbosity? verbosity, ulong? executionEnvironment, int? forks, int? jobSliceCount,
                          int? timeout, ulong unifiedJobTemplate, bool enabled, DateTime? dtStart, DateTime? dtEnd,
                          DateTime? nextRun, string timezone, string until)
                : ISchedule, IResource<Schedule.Summary>
    {
        public const string PATH = "/api/v2/schedules/";
        public static async Task<Schedule> Get(ulong id)
        {
            var apiResult = await RestAPI.GetAsync<Schedule>($"{PATH}{id}/");
            return apiResult.Contents;
        }
        public static async IAsyncEnumerable<Schedule> Find(NameValueCollection? query, bool getAll = false)
        {
            await foreach(var result in RestAPI.GetResultSetAsync<Schedule>(PATH, query, getAll))
            {
                foreach (var app in result.Contents.Results)
                {
                    yield return app;
                }
            }
        }
        public record Summary(
            [property: JsonPropertyName("unified_job_template")] UnifiedJobTemplateSummary UnifiedJobTemplate,
            [property: JsonPropertyName("created_by")] UserSummary? CreatedBy,
            [property: JsonPropertyName("modified_by")] UserSummary? ModifiedBy,
            [property: JsonPropertyName("user_capabilities")] Capability UserCapabilities,
            InventorySummary? Inventory);


        public string Rrule { get; } = rrule;

        public ulong Id { get; } = id;
        public ResourceType Type { get; } = type;
        public string Url { get; } = url;
        public RelatedDictionary Related { get; } = related;
        public Summary SummaryFields { get; } = summaryFields;
        public DateTime Created { get; } = created;
        public DateTime? Modified { get; } = modified;

        public string Name { get; } = name;
        public string Description { get; } = description;
        public OrderedDictionary ExtraData { get; } = extraData;
        public ulong? Inventory { get; } = inventory;
        public string? ScmBranch { get; } = scmBranch;
        public string? JobType { get; } = jobType;
        public string? JobTags { get; } = jobTags;
        public string? SkipTags { get; } = skipTags;
        public string? Limit { get; } = limit;
        public bool? DiffMode { get; } = diffMode;
        public JobVerbosity? Verbosity { get; } = verbosity;
        public ulong? ExecutionEnvironment { get; } = executionEnvironment;
        public int? Forks { get; } = forks;
        public int? JobSliceCount { get; } = jobSliceCount;
        public int? Timeout { get; } = timeout;
        public ulong UnifiedJobTemplate { get; } = unifiedJobTemplate;
        public bool Enabled { get; } = enabled;
        public DateTime? DTStart { get; } = dtStart;
        public DateTime? DTEnd { get; } = dtEnd;
        [JsonPropertyName("next_run")]
        public DateTime? NextRun { get; } = nextRun;
        public string TimeZone { get; } = timezone;
        public string Until { get; } = until;

    }
}
