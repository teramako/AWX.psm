using AWX.Resources;
using System.Management.Automation;

namespace AWX.Cmdlets
{
    [Cmdlet(VerbsCommon.Get, "WorkflowJob")]
    [OutputType(typeof(WorkflowJob.Detail))]
    public class GetWorkflowJobCommand : GetCommandBase<WorkflowJob.Detail>
    {
        protected override string ApiPath => WorkflowJob.PATH;
        protected override ResourceType AcceptType => ResourceType.WorkflowJob;

        protected override void ProcessRecord()
        {
            WriteObject(GetResource(), true);
        }
    }

    [Cmdlet(VerbsCommon.Find, "WorkflowJob", DefaultParameterSetName = "All")]
    [OutputType(typeof(WorkflowJob))]
    public class FindWorkflowJobCommand : FindCommandBase
    {
        [Parameter(Mandatory = true, ParameterSetName = "AssociatedWith", ValueFromPipelineByPropertyName = true)]
        [ValidateSet(nameof(ResourceType.JobTemplate),
                     nameof(ResourceType.WorkflowJobTemplate))]
        public override ResourceType Type { get; set; }
        [Parameter(Mandatory = true, ParameterSetName = "AssociatedWith", ValueFromPipelineByPropertyName = true)]
        public override ulong Id { get; set; }

        [Parameter(Position = 0)]
        public string[]? Name { get; set; }

        [Parameter()]
        [ValidateSet(typeof(EnumValidateSetGenerator<JobStatus>))]
        public string[]? Status { get; set; }

        [Parameter()]
        [ValidateSet(typeof(EnumValidateSetGenerator<JobLaunchType>))]
        public string[]? LaunchType { get; set; }

        [Parameter()]
        public override string[] OrderBy { get; set; } = ["!id"];

        protected override void BeginProcessing()
        {
            if (Name != null)
            {
                Query.Add("name__in", string.Join(',', Name));
            }
            if (Status != null)
            {
                Query.Add("status__in", string.Join(',', Status));
            }
            if (LaunchType != null)
            {
                Query.Add("launch_type__in", string.Join(',', LaunchType));
            }
            SetupCommonQuery();
        }
        protected override void ProcessRecord()
        {
            var path = Type switch
            {
                ResourceType.JobTemplate => $"{JobTemplate.PATH}{Id}/slice_workflow_jobs/",
                ResourceType.WorkflowJobTemplate => $"{WorkflowJobTemplate.PATH}{Id}/workflow_jobs/",
                _ => WorkflowJob.PATH
            };
            foreach (var resultSet in GetResultSet<WorkflowJob>(path, Query, All))
            {
                WriteObject(resultSet.Results, true);
            }
        }
    }

    [Cmdlet(VerbsCommon.Remove, "WorkflowJob", SupportsShouldProcess = true, ConfirmImpact = ConfirmImpact.High)]
    public class RemoveWorkflowJobCommand : APICmdletBase
    {
        [Parameter(Mandatory = true, ValueFromPipeline = true, Position = 0)]
        [ResourceIdTransformation(AcceptableTypes = [ResourceType.WorkflowJob])]
        public ulong Id { get; set; }

        [Parameter()]
        public SwitchParameter Force { get; set; }

        protected override void ProcessRecord()
        {
            if (Force || ShouldProcess($"WorkflowJob [{Id}]", "Delete completely"))
            {
                try
                {
                    var apiResult = DeleteResource($"{WorkflowJob.PATH}{Id}/");
                    if (apiResult?.IsSuccessStatusCode ?? false)
                    {
                        WriteVerbose($"WorkflowJob {Id} is removed.");
                    }
                }
                catch (RestAPIException) { }
            }
        }
    }
}
