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

    [Cmdlet(VerbsLifecycle.Stop, "UnifiedJob", DefaultParameterSetName = "RequestCancel")]
    [OutputType(typeof(PSObject))]
    public class StopJobCommand : APICmdletBase
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

        [Parameter(Mandatory = true, ParameterSetName = "Determine")]
        public SwitchParameter Determine { get; set; }

        protected override void ProcessRecord()
        {
            var path = Type switch
            {
                ResourceType.Job => $"{JobTemplateJob.PATH}{Id}/cancel/",
                ResourceType.ProjectUpdate => $"{ProjectUpdateJob.PATH}{Id}/cancel/",
                ResourceType.InventoryUpdate => $"{InventoryUpdateJob.PATH}{Id}/cancel/",
                ResourceType.AdHocCommand => $"{AdHocCommand.PATH}{Id}/cancel/",
                ResourceType.SystemJob => $"{SystemJob.PATH}{Id}/cancel/",
                ResourceType.WorkflowJob => $"{WorkflowJob.PATH}{Id}/cancel/",
                _ => throw new NotImplementedException()
            };
            var psobject = new PSObject();
            psobject.Members.Add(new PSNoteProperty("Id", Id));
            psobject.Members.Add(new PSNoteProperty("Type", Type));
            if (Determine)
            {
                var result = GetResource<Dictionary<string, bool>>(path);
                if (result == null) return;
                result.TryGetValue("can_cancel", out bool canCancel);
                psobject.Members.Add(new PSNoteProperty("CanCancel", canCancel));
                WriteObject(psobject);
            }
            else
            {
                try
                {
                    var apiResult = CreateResource<string>(path);
                    psobject.Members.Add(new PSNoteProperty("Status", apiResult.Response.StatusCode));
                }
                catch (RestAPIException ex)
                {
                    psobject.Members.Add(new PSNoteProperty("Status", ex.Response.StatusCode));
                }
                WriteObject(psobject);
            }
        }
    }
}
