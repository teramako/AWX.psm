using System.Collections.Specialized;

namespace AWX.Resources
{
    public class WorkflowApproval(ulong id, ResourceType type, string url, RelatedDictionary related,
                                  WorkflowApproval.Summary summaryFields, DateTime created, DateTime? modified,
                                  string name, string description, ulong? unifiedJobTemplate, JobLaunchType launchType,
                                  JobStatus status, ulong? executionEnvironment, bool failed, DateTime? started,
                                  DateTime? finished, DateTime? canceledOn, double elapsed, string jobExplanation,
                                  LaunchedBy launchedBy, string? workUnitId, bool canApproveOrDeny,
                                  DateTime? approvalExpiration, bool timedOut)
        : UnifiedJob(id, type, url, created, modified, name, launchType, status, executionEnvironment, failed, started,
                     finished, canceledOn, elapsed, jobExplanation, launchedBy, workUnitId),
          IResource<WorkflowApproval.Summary>
    {
        public new const string PATH = "/api/v2/workflow_approvals/";

        /// <summary>
        /// Retrieve a Workflow Approval.<br/>
        /// API Path: <c>/api/v2/workflow_approvals/<paramref name="id"/>/</c>
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static new async Task<Detail> Get(ulong id)
        {
            var apiResult = await RestAPI.GetAsync<Detail>($"{PATH}{id}/");
            return apiResult.Contents;
        }
        /// <summary>
        /// List Workflow Approvals.<br/>
        /// API Path: <c>/api/v2/workflow_approvals/</c>
        /// </summary>
        /// <param name="query"></param>
        /// <param name="getAll"></param>
        /// <returns></returns>
        public static new async IAsyncEnumerable<WorkflowApproval> Find(NameValueCollection? query, bool getAll = false)
        {
            await foreach (var result in RestAPI.GetResultSetAsync<WorkflowApproval>(PATH, query, getAll))
            {
                foreach (var workflowJob in result.Contents.Results)
                {
                    yield return workflowJob;
                }
            }
        }

        public record Summary(WorkflowJobTemplateSummary WorkflowJobTemplate,
                              WorkflowJobSummary WorkflowJob,
                              WorkflowApprovalTemplateSummary WorkflowApprovalTemplate,
                              UnifiedJobTemplateSummary? UnifiedJobTemplate,
                              UserSummary? ApprovedOrDeniedBy,
                              UserSummary? CreatedBy,
                              Capability UserCapabilities,
                              SourceWorkflowJobSummary SourceWorkflowJob);

        public RelatedDictionary Related { get; } = related;
        public Summary SummaryFields { get; } = summaryFields;
        public string Description { get; } = description;
        public ulong? UnifiedJobTemplate { get; } = unifiedJobTemplate;
        public bool CanApproveOrDeny { get; } = canApproveOrDeny;
        public DateTime? ApprovalExpiration { get; } = approvalExpiration;
        public bool TimedOut { get; } = timedOut;

        public class Detail(ulong id, ResourceType type, string url, RelatedDictionary related, Summary summaryFields,
                            DateTime created, DateTime? modified, string name, string description,
                            ulong? unifiedJobTemplate, JobLaunchType launchType, JobStatus status,
                            ulong? executionEnvironment, bool failed, DateTime? started, DateTime? finished,
                            DateTime? canceledOn, double elapsed, string jobArgs, string jobCwd,
                            Dictionary<string, string> jobEnv, string jobExplanation, string resultTraceback,
                            LaunchedBy launchedBy, string? workUnitId, bool canApproveOrDeny,
                            DateTime? approvalExpiration, bool timedOut)
            : WorkflowApproval(id, type, url, related, summaryFields, created, modified, name, description, unifiedJobTemplate,
                               launchType, status, executionEnvironment, failed, started, finished, canceledOn, elapsed,
                               jobExplanation, launchedBy, workUnitId, canApproveOrDeny, approvalExpiration, timedOut),
              IJobDetail, IResource<Summary>
        {
            public string JobArgs { get; } = jobArgs;
            public string JobCwd { get; } = jobCwd;
            public Dictionary<string, string> JobEnv { get; } = jobEnv;
            public string ResultTraceback { get; } = resultTraceback;
        }
    }
}
