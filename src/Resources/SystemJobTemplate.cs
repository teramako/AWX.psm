using System.Collections.Specialized;
using System.Text.Json.Serialization;

namespace AWX.Resources
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
        : UnifiedJobTemplate(id, type, url, created, modified, name, description, lastJobRun,
                             lastJobFailed, nextJobRun, status),
          IUnifiedJobTemplate, IResource<SystemJobTemplate.Summary>
    {
        public new const string PATH = "/api/v2/system_job_templates/";

        public static async Task<SystemJobTemplate> Get(ulong id)
        {
            var apiResult = await RestAPI.GetAsync<SystemJobTemplate>($"{PATH}{id}/");
            return apiResult.Contents;
        }
        public static new async IAsyncEnumerable<SystemJobTemplate> Find(NameValueCollection? query, bool getAll = false)
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


        public RelatedDictionary Related { get; } = related;
        public Summary SummaryFields { get; } = summaryFields;

        [JsonPropertyName("execution_environment")]
        public ulong? ExecutionEnvironment { get; } = executionEnvironment;
        [JsonPropertyName("job_type")]
        public string JobType { get; } = jobType;
    }
}
