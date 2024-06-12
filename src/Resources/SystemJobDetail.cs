using System.Collections.Specialized;

namespace AnsibleTower.Resources
{
    public class SystemJobDetail(ulong id, ResourceType type, string url, RelatedDictionary related,
                                 SystemJob.Summary summaryFields, DateTime created, DateTime? modified, string name,
                                 string description, ulong unifiedJobTemplate, JobLaunchType launchType,
                                 JobStatus status, ulong? executionEnvironment, bool failed, DateTime? started,
                                 DateTime? finished, DateTime? canceledOn, double elapsed, string jobExplanation,
                                 string executionNode, LaunchedBy launchedBy, string? workUnitId,
                                 ulong systemJobTemplate, string jobType, string extraVars, string resultStdout)
        : ISystemJob, IResource<SystemJob.Summary>
    {

        public ulong Id { get; } = id;
        public ResourceType Type { get; } = type;
        public string Url { get; } = url;
        public RelatedDictionary Related { get; } = related;
        public SystemJob.Summary SummaryFields { get; } = summaryFields;

        #region UnifiedJob Properties
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
        public string ExecutionNode { get; } = executionNode;
        public LaunchedBy LaunchedBy { get; } = launchedBy;
        public string? WorkUnitId { get; } = workUnitId;
        #endregion

        public ulong SystemJobTemplate { get; } = systemJobTemplate;
        public string JobType { get; } = jobType;
        public string ExtraVars { get; } = extraVars;
        public string ResultStdout { get; } = resultStdout;
    }
}

