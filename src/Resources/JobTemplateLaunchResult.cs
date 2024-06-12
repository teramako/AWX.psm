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
            : IUnifiedJob, IResource<JobTemplateJob.Summary>
    {
        public ulong Job { get; } = job;
        [JsonPropertyName("ignore_fields")]
        public Dictionary<string, object?> IgnoreFields { get; } = ignoreFields;
        public ulong Id { get; } = id;
        public ResourceType Type { get; } = type;
        public string Url { get; } = url;
        public RelatedDictionary Related { get; } = related;
        public JobTemplateJob.Summary SummaryFields { get; } = summaryFields;
        public DateTime Created { get; } = created;
        public DateTime? Modified { get; } = modified;
        public string Name { get; } = name;
        public string Description { get; } = description;
        public ulong UnifiedJobTemplate { get; } = unifiedJobTemplate;
        public JobLaunchType LaunchType { get; } = launchType;
        public JobStatus Status { get; } = status;
        public ulong? ExecutionEnvironment { get; } = executionEnvironment;
        public bool Failed { get; } = failed;
        public DateTime? Started { get; } = started;
        public DateTime? Finished { get; } = finished;
        public DateTime? CanceledOn { get; } = canceledOn;
        public double Elapsed { get; } = elapsed;
        public string JobExplanation { get; } = jobExplanation;
        public LaunchedBy LaunchedBy { get; } = launchedBy;
        public string? WorkUnitId { get; } = workUnitId;
    }
}

