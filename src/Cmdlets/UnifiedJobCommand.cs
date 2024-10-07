using AWX.Resources;
using System.Collections.Specialized;
using System.Management.Automation;

namespace AWX.Cmdlets
{
    [Cmdlet(VerbsCommon.Find, "UnifiedJob", DefaultParameterSetName = "All")]
    [OutputType(typeof(IUnifiedJob))]
    public class FindUnifiedJobCommand : FindCmdletBase
    {
        [Parameter(Mandatory = true, ParameterSetName = "AssociatedWith", ValueFromPipelineByPropertyName = true)]
        [ValidateSet(nameof(ResourceType.JobTemplate),
                     nameof(ResourceType.WorkflowJobTemplate),
                     nameof(ResourceType.Project),
                     nameof(ResourceType.InventorySource),
                     nameof(ResourceType.SystemJobTemplate),
                     nameof(ResourceType.Inventory),
                     nameof(ResourceType.Host),
                     nameof(ResourceType.Group),
                     nameof(ResourceType.Schedule),
                     nameof(ResourceType.Instance),
                     nameof(ResourceType.InstanceGroup))]
        public override ResourceType Type { get; set; }
        [Parameter(Mandatory = true, ParameterSetName = "AssociatedWith", ValueFromPipelineByPropertyName = true)]
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
        private void WriteResultSet(string path)
        {
            foreach (var resultSet in GetResultSet(path, Query, All))
            {
                WriteObject(resultSet.Results, true);
            }
        }
        private void WriteResultSet<T>(string path) where T : class
        {
            foreach (var resultSet in GetResultSet<T>(path, Query, All))
            {
                WriteObject(resultSet.Results, true);
            }
        }
        protected override void BeginProcessing()
        {
            SetupCommonQuery();
        }
        protected override void ProcessRecord()
        {
            switch (Type)
            {
                case ResourceType.JobTemplate:
                    WriteResultSet<JobTemplateJob>($"{JobTemplate.PATH}{Id}/jobs/");
                    break;
                case ResourceType.WorkflowApprovalTemplate:
                    WriteResultSet<WorkflowJob>($"{WorkflowJobTemplate.PATH}{Id}/workflow_jobs/");
                    break;
                case ResourceType.Project:
                    WriteResultSet<ProjectUpdateJob>($"{Project.PATH}{Id}/project_updates/");
                    break;
                case ResourceType.InventorySource:
                    WriteResultSet<InventoryUpdateJob>($"{InventorySource.PATH}{Id}/inventory_updates/");
                    break;
                case ResourceType.SystemJobTemplate:
                    WriteResultSet<SystemJob>($"{SystemJob.PATH}{Id}/jobs/");
                    break;
                case ResourceType.Inventory:
                    WriteResultSet<AdHocCommand>($"{Inventory.PATH}{Id}/ad_hoc_commands/");
                    break;
                case ResourceType.Host:
                    WriteResultSet<AdHocCommand>($"{Host.PATH}{Id}/ad_hoc_commands/");
                    break;
                case ResourceType.Group:
                    WriteResultSet<AdHocCommand>($"{Group.PATH}{Id}/ad_hoc_commands/");
                    break;
                case ResourceType.Schedule:
                    WriteResultSet($"{Schedule.PATH}{Id}/jobs/");
                    break;
                case ResourceType.Instance:
                    WriteResultSet($"{Instance.PATH}{Id}/jobs/");
                    break;
                case ResourceType.InstanceGroup:
                    WriteResultSet($"{InstanceGroup.PATH}{Id}/jobs/");
                    break;
                default:
                    WriteResultSet(UnifiedJob.PATH);
                    break;
            };
        }
    }

    [Cmdlet(VerbsLifecycle.Wait, "UnifiedJob")]
    [OutputType(typeof(JobTemplateJob),
                typeof(ProjectUpdateJob),
                typeof(InventoryUpdateJob),
                typeof(SystemJob),
                typeof(AdHocCommand),
                typeof(WorkflowJob))]
    public class WaitJobCommand : LaunchJobCommandBase
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
            JobProgressManager.Add(Id, new JobProgress(Id, Type));
        }
        protected override void EndProcessing()
        {
            JobProgressManager.UpdateJob();
            ShowJobLog(SuppressJobLog);
            JobProgressManager.CleanCompleted();
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
