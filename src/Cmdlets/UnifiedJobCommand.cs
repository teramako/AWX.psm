using AWX.Resources;
using System.Collections.Specialized;
using System.Management.Automation;

namespace AWX.Cmdlets
{
    [Cmdlet(VerbsCommon.Find, "UnifiedJob")]
    [OutputType(typeof(IUnifiedJob))]
    public class FindUnifiedJobCommand : FindCmdletBase
    {
        public override ResourceType Type { get; set; }
        public override ulong Id { get; set; }

        [Parameter()]
        public override string[] OrderBy { get; set; } = ["!id"];

        private IEnumerable<ResultSet> GetResultSet(string path,
                                                    NameValueCollection? query = null,
                                                    bool getAll = false)
        {
            var nextPathAndQuery = path + (query == null ? "" : $"?{query}");
            do
            {
                WriteVerboseRequest(nextPathAndQuery, Method.GET);
                RestAPIResult<ResultSet>? result;
                try
                {
                    using var apiTask = RestAPI.GetAsync<ResultSet>(nextPathAndQuery);
                    apiTask.Wait();
                    result = apiTask.Result;
                    WriteVerboseResponse(result.Response);
                }
                catch (RestAPIException ex)
                {
                    WriteVerboseResponse(ex.Response);
                    WriteApiError(ex);
                    break;
                }
                var resultSet = result.Contents;

                yield return resultSet;

                nextPathAndQuery = string.IsNullOrEmpty(resultSet?.Next) ? string.Empty : resultSet.Next;
            } while (getAll && !string.IsNullOrEmpty(nextPathAndQuery));
        }
        protected override void BeginProcessing()
        {
            SetupCommonQuery();
        }
        protected override void ProcessRecord()
        {
            foreach (var resultSet in GetResultSet(UnifiedJob.PATH, Query, All))
            {
                WriteObject(resultSet.Results, true);
            }
        }
    }

    [Cmdlet(VerbsLifecycle.Wait, "UnifiedJob")]
    [OutputType(typeof(JobTemplateJob),
                typeof(ProjectUpdateJob),
                typeof(InventoryUpdateJob),
                typeof(SystemJob),
                typeof(AdHocCommand),
                typeof(WorkflowJob))]
    public class WaitJobCommand : InvokeJobBase
    {
        [Parameter(Mandatory = true, ValueFromPipelineByPropertyName = true, Position = 0)]
        [ValidateSet(nameof(ResourceType.Job),
                     nameof(ResourceType.ProjectUpdate),
                     nameof(ResourceType.InventoryUpdate),
                     nameof(ResourceType.SystemJob),
                     nameof(ResourceType.AdHocCommand),
                     nameof(ResourceType.WorkflowJob))]
        public ResourceType Type { get; set; }
        [Parameter(Mandatory = true, ValueFromPipelineByPropertyName = true, Position = 1)]
        public ulong Id { get; set; }

        [Parameter()]
        [ValidateRange(5, int.MaxValue)]
        public int IntervalSeconds { get; set; } = 5;

        [Parameter()]
        public SwitchParameter SuppressJobLog { get; set; }

        protected override void ProcessRecord()
        {
            JobManager.Add(Id, new JobProgress(Id, Type));
        }
        protected override void EndProcessing()
        {
            JobManager.UpdateJob();
            ShowJobLog(SuppressJobLog);
            JobManager.CleanCompleted();
            WaitJobs("Wait Job", IntervalSeconds, SuppressJobLog);
        }
    }
}
