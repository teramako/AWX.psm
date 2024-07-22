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

    public abstract class LaunchWorkflowJobTemplateCommandBase : InvokeJobBase
    {
        [Parameter(Mandatory = true, ParameterSetName = "Id", ValueFromPipeline = true, Position = 0)]
        [Parameter(Mandatory = true, ParameterSetName = "GetRequirementsId", ValueFromPipeline = true, Position = 0)]
        public ulong Id { get; set; }
        [Parameter(Mandatory = true, ParameterSetName = "JobTemplate", ValueFromPipeline = true, Position = 0)]
        [Parameter(Mandatory = true, ParameterSetName = "GetRequirementsJobTemplate", ValueFromPipeline = true, Position = 0)]
        public WorkflowJobTemplate? WorkflowJobTemplate { get; set; }

        [Parameter(Mandatory = true, ParameterSetName = "GetRequirementsId")]
        [Parameter(Mandatory = true, ParameterSetName = "GetRequirementsJobTemplate")]
        public SwitchParameter GetRequirements { get; set; }

        [Parameter(ParameterSetName = "Id")]
        [Parameter(ParameterSetName = "JobTemplate")]
        public string? Limit { get; set; }

        [Parameter(ParameterSetName = "Id")]
        [Parameter(ParameterSetName = "JobTemplate")]
        public ulong? Inventory { get; set; }

        private Hashtable CreateSendData()
        {
            var dict = new Hashtable();
            if (Inventory != null)
            {
                dict.Add("inventory", Inventory);
            }
            if (Limit != null)
            {
                dict.Add("limit", Limit);
            }
            return dict;
        }
        protected void GetLaunchRequirements(ulong id)
        {
            var res = base.GetResource<WorkflowJobTemplateLaunchRequirements>($"{WorkflowJobTemplate.PATH}{id}/launch/");
            if (res == null)
            {
                return;
            }
            WriteObject(res, false);
        }
        protected WorkflowJob.LaunchResult Launch(ulong id)
        {
            var apiResult = CreateResource<WorkflowJob.LaunchResult>($"{WorkflowJobTemplate.PATH}{id}/launch/", CreateSendData());
            var launchResult = apiResult.Contents;
            WriteVerbose($"Launch WorkflowJobTemplate:{id} => Job:[{launchResult.Id}]");
            if (launchResult.IgnoredFields.Count > 0)
            {
                foreach (var (key ,val) in launchResult.IgnoredFields)
                {
                    WriteWarning($"Ignored field: {key} ({val})");
                }
            }
            return launchResult;
        }
    }

    [Cmdlet(VerbsLifecycle.Invoke, "WorkflowJobTemplate")]
    [OutputType(typeof(JobTemplateJob), ParameterSetName = ["Id", "JobTemplate"])]
    [OutputType(typeof(JobTemplateLaunchRequirements), ParameterSetName = ["GetRequirementsId", "GetRequirementsJobTemplate"])]
    public class InvokeWorkflowJobTemplateCommand : LaunchWorkflowJobTemplateCommandBase
    {
        [Parameter(ParameterSetName = "Id")]
        [Parameter(ParameterSetName = "JobTemplate")]
        [ValidateRange(5, int.MaxValue)]
        public int IntervalSeconds { get; set; } = 5;

        [Parameter(ParameterSetName = "Id")]
        [Parameter(ParameterSetName = "JobTemplate")]
        public SwitchParameter SuppressJobLog { get; set; }

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
                try
                {
                    var launchResult = Launch(Id);
                    JobManager.Add(launchResult);
                }
                catch (RestAPIException) {}
            }
        }
        protected override void EndProcessing()
        {
            if (GetRequirements)
            {
                return;
            }
            WaitJobs("Launch WorkflowJobTemplate", IntervalSeconds, SuppressJobLog);
        }
    }

    [Cmdlet(VerbsLifecycle.Start, "WorkflowJobTemplate")]
    [OutputType(typeof(JobTemplateJob.LaunchResult), ParameterSetName = ["Id", "JobTemplate"])]
    [OutputType(typeof(JobTemplateLaunchRequirements), ParameterSetName = ["GetRequirementsId", "GetRequirementsJobTemplate"])]
    public class StartWorkflowJobTemplateCommand : LaunchWorkflowJobTemplateCommandBase
    {
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
                try
                {
                    var launchResult = Launch(Id);
                    WriteObject(launchResult, false);
                }
                catch (RestAPIException) {}
            }
        }
    }
}
