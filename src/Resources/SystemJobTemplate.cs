using System.Collections.Specialized;
using System.Text.Json.Serialization;

namespace AnsibleTower.Resources
{
    public class SystemJobTemplate(ulong id,
                                   ResourceType type,
                                   string url,
                                   RelatedDictionary related,
                                   SystemJobTemplate.Summary summaryFields,
                                   DateTime created,
                                   DateTime? modified,
                                   string name,
                                   string description,
                                   DateTime? lastJobRun,
                                   bool lastJobFailed,
                                   DateTime? nextJobRun,
                                   JobTemplateStatus status,
                                   ulong? executionEnvironment,
                                   string jobType)
        : IUnifiedJobTemplate, IResource<SystemJobTemplate.Summary>
    {
        public const string PATH = "/api/v2/system_job_templates/";

        public static async Task<SystemJobTemplate> Get(ulong id)
        {
            var apiResult = await RestAPI.GetAsync<SystemJobTemplate>($"{PATH}{id}/");
            return apiResult.Contents;
        }
        public static async IAsyncEnumerable<SystemJobTemplate> Find(NameValueCollection? query, bool getAll = false)
        {
            await foreach(var result in RestAPI.GetResultSetAsync<SystemJobTemplate>(PATH, query, getAll))
            {
                foreach (var app in result.Contents.Results)
                {
                    yield return app;
                }
            }
        }
        public record Summary(
            [property: JsonPropertyName("last_job")] LastJobSummary? LastJob,
            [property: JsonPropertyName("last_update")] LastUpdateSummary? LastUpdate,
            [property: JsonPropertyName("resolved_environment")] EnvironmentSummary? ResolvedEnvironment);


        public ulong Id { get; } = id;
        public ResourceType Type { get; } = type;
        public string Url { get; } = url;
        public RelatedDictionary Related { get; } = related;
        public Summary SummaryFields { get; } = summaryFields;
        public DateTime Created { get; } = created;
        public DateTime? Modified { get; } = modified;

        public string Name { get; } = name;
        public string Description { get; } = description;
        public DateTime? LastJobRun { get; } = lastJobRun;
        public bool LastJobFailed { get; } = lastJobFailed;
        public DateTime? NextJobRun { get; } = nextJobRun;
        public JobTemplateStatus Status { get; } = status;
        [JsonPropertyName("execution_environment")]
        public ulong? ExecutionEnvironment { get; } = executionEnvironment;
        [JsonPropertyName("job_type")]
        public string JobType { get; } = jobType;
    }
}
