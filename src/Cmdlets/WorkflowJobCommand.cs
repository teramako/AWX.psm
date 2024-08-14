using AWX.Resources;
using System.Management.Automation;

namespace AWX.Cmdlets
{
    [Cmdlet(VerbsCommon.Get, "WorkflowJob")]
    [OutputType(typeof(WorkflowJob.Detail))]
    public class GetWorkflowJobCommand : GetCmdletBase
    {
        protected override void ProcessRecord()
        {
            foreach (var id in Id)
            {
                if (!IdSet.Add(id))
                {
                    // skip already processed
                    continue;
                }
                var res = GetResource<WorkflowJob.Detail>($"{WorkflowJob.PATH}{id}/");
                if (res != null)
                {
                    WriteObject(res);
                }
            }
        }
    }

    [Cmdlet(VerbsCommon.Find, "WorkflowJob", DefaultParameterSetName = "All")]
    [OutputType(typeof(WorkflowJob))]
    public class FindWorkflowJobCommand : FindCmdletBase
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
}
