using AWX.Resources;
using System.Collections;
using System.Management.Automation;

namespace AWX.Cmdlets
{
    [Cmdlet(VerbsCommon.Get, "WorkflowJobTemplate")]
    [OutputType(typeof(WorkflowJobTemplate))]
    public class GetWorkflowJobTemplateCommand : GetCmdletBase
    {
        protected override void ProcessRecord()
        {
            foreach (var id in Id)
            {
                IdSet.Add(id);
            }
        }
        protected override void EndProcessing()
        {
            Query.Add("id__in", string.Join(',', IdSet));
            Query.Add("page_size", $"{IdSet.Count}");
            foreach (var resultSet in GetResultSet<WorkflowJobTemplate>($"{WorkflowJobTemplate.PATH}?{Query}", true))
            {
                WriteObject(resultSet.Results, true);
            }
        }
    }

    [Cmdlet(VerbsCommon.Find, "WorkflowJobTemplate", DefaultParameterSetName = "All")]
    [OutputType(typeof(WorkflowJobTemplate))]
    public class FindWorkflowJobTemplateCommand : FindCmdletBase
    {
        [Parameter(Mandatory = true, ParameterSetName = "AssociatedWith", ValueFromPipelineByPropertyName = true)]
        [ValidateSet(nameof(ResourceType.Organization))]
        public override ResourceType Type { get; set; }
        [Parameter(Mandatory = true, ParameterSetName = "AssociatedWith", ValueFromPipelineByPropertyName = true)]
        public override ulong Id { get; set; }

        [Parameter()]
        public override string[] OrderBy { get; set; } = ["id"];

        protected override void BeginProcessing()
        {
            SetupCommonQuery();
        }
        protected override void ProcessRecord()
        {
            var path = Type switch
            {
                ResourceType.Organization => $"{Organization.PATH}{Id}/workflow_job_templates/",
                _ => WorkflowJobTemplate.PATH
            };
            foreach (var resultSet in GetResultSet<WorkflowJobTemplate>(path, Query, All))
            {
                WriteObject(resultSet.Results, true);
            }
        }
    }

    [Cmdlet(VerbsLifecycle.Invoke, "WorkJobTemplate")]
    [OutputType(typeof(JobTemplateJob), ParameterSetName = ["Id", "JobTemplate"])]
    [OutputType(typeof(JobTemplateJob.LaunchResult), ParameterSetName = ["AsyncId", "AsyncJobTemplate"])]
    [OutputType(typeof(JobTemplateLaunchRequirements), ParameterSetName = ["GetRequirementsId", "GetRequirementsJobTemplate"])]
    public class InvokeWorkJobTemplateCommand : InvokeJobBase
    {
        [Parameter(Mandatory = true, ParameterSetName = "Id", ValueFromPipeline = true, Position = 0)]
        [Parameter(Mandatory = true, ParameterSetName = "AsyncId", ValueFromPipeline = true, Position = 0)]
        [Parameter(Mandatory = true, ParameterSetName = "GetRequirementsId", ValueFromPipeline = true, Position = 0)]
        public ulong Id { get; set; }
        [Parameter(Mandatory = true, ParameterSetName = "JobTemplate", ValueFromPipeline = true, Position = 0)]
        [Parameter(Mandatory = true, ParameterSetName = "AsyncJobTemplate", ValueFromPipeline = true, Position = 0)]
        [Parameter(Mandatory = true, ParameterSetName = "GetRequirementsJobTemplate", ValueFromPipeline = true, Position = 0)]
        public WorkflowJobTemplate? WorkflowJobTemplate { get; set; }

        [Parameter(Mandatory = true, ParameterSetName = "GetRequirementsId")]
        [Parameter(Mandatory = true, ParameterSetName = "GetRequirementsJobTemplate")]
        public SwitchParameter GetRequirements { get; set; }

        [Parameter(Mandatory = true, ParameterSetName = "AsyncId")]
        [Parameter(Mandatory = true, ParameterSetName = "AsyncJobTemplate")]
        public SwitchParameter Async { get; set; }

        [Parameter(ParameterSetName = "Id")]
        [Parameter(ParameterSetName = "AsyncId")]
        [Parameter(ParameterSetName = "JobTemplate")]
        [Parameter(ParameterSetName = "AsyncJobTemplate")]
        public string? Limit { get; set; }

        [Parameter(ParameterSetName = "Id")]
        [Parameter(ParameterSetName = "JobTemplate")]
        [ValidateRange(5, int.MaxValue)]
        public int IntervalSeconds { get; set; } = 5;

        [Parameter(ParameterSetName = "Id")]
        [Parameter(ParameterSetName = "JobTemplate")]
        public SwitchParameter SuppressJobLog { get; set; }

        private Hashtable CreateSendData()
        {
            var dict = new Hashtable();
            if (Limit != null)
            {
                dict.Add("limit", Limit);
            }
            return dict;
        }
        private void GetLaunchRequirements(ulong id)
        {
            var res = base.GetResource<WorkflowJobTemplateLaunchRequirements>($"{WorkflowJobTemplate.PATH}{id}/launch/");
            if (res == null)
            {
                return;
            }
            WriteObject(res, false);
        }
        private void Launch(ulong id)
        {
            var launchResult = CreateResource<WorkflowJob.LaunchResult>($"{WorkflowJobTemplate.PATH}{id}/launch/", CreateSendData());
            if (launchResult == null)
            {
                return;
            }
            WriteVerbose($"Launch WorkflowJobTemplate:{id} => Job:[{launchResult.Id}]");
            if (Async)
            {
                WriteObject(launchResult, false);
            }
            else
            {
                JobManager.Add(launchResult);
            }
        }
        protected override void ProcessRecord()
        {
            if (WorkflowJobTemplate != null)
            {
                Id = WorkflowJobTemplate.Id;
            }
            if (GetRequirements)
            {
                GetLaunchRequirements(Id);
            }
            else
            {
                Launch(Id);
            }
        }
        protected override void EndProcessing()
        {
            if (GetRequirements)
            {
                return;
            }
            if (Async)
            {
                return;
            }
            WaitJobs("Launch WorkflowJobTemplate", IntervalSeconds, SuppressJobLog);
        }
    }
}
