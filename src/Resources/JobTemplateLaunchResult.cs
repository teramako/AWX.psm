using System.Text.Json.Serialization;

namespace AnsibleTower.Resources
{
    public class JobTemplateLaunchResult(ulong job, Dictionary<string, object?> ignoreFields,
                                         ulong id, ResourceType type, string url, RelatedDictionary related,
                                         JobTemplateJob.Summary summaryFields, DateTime created, DateTime? modified,
                                         string name, string description, ulong unifiedJobTemplate,
                                         JobLaunchType launchType, JobStatus status, ulong? executionEnvironment,
                                         bool failed, DateTime? started, DateTime? finished, DateTime? canceledOn,
                                         double elapsed, string jobExplanation, LaunchedBy launchedBy,
                                         string? workUnitId)
        : UnifiedJob(id, type, url, created, modified, name, description, unifiedJobTemplate, launchType, status,
                     executionEnvironment, failed, started, finished, canceledOn, elapsed, jobExplanation,
                     launchedBy, workUnitId),
          IUnifiedJob, IResource<JobTemplateJob.Summary>
    {
        public ulong Job { get; } = job;
        [JsonPropertyName("ignore_fields")]
        public Dictionary<string, object?> IgnoreFields { get; } = ignoreFields;
        public RelatedDictionary Related { get; } = related;
        public JobTemplateJob.Summary SummaryFields { get; } = summaryFields;
    }
}

