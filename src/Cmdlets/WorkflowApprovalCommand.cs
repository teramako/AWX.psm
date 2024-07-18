using AWX.Resources;
using System.Management.Automation;
using System.Web;

namespace AWX.Cmdlets
{
    [Cmdlet(VerbsCommon.Get, "WorkflowApproval")]
    [OutputType(typeof(WorkflowApproval.Detail))]
    public class GetWorkflowApprovalCommand : GetCmdletBase
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
                var res = GetResource<WorkflowApproval.Detail>($"{WorkflowApproval.PATH}{id}/");
                if (res != null)
                {
                    WriteObject(res);
                }
            }
        }
    }

    [Cmdlet(VerbsCommon.Find, "WorkflowApproval", DefaultParameterSetName = "All")]
    [OutputType(typeof(WorkflowApproval))]
    public class FindWorkflowApprovalCommand : FindCmdletBase
    {
        [Parameter(ParameterSetName = "AssociatedWith", ValueFromPipelineByPropertyName = true)]
        [ValidateSet(nameof(ResourceType.WorkflowApprovalTemplate))]
        public override ResourceType Type { get; set; } = ResourceType.WorkflowApproval;
        [Parameter(Mandatory = true, ParameterSetName = "AssociatedWith", ValueFromPipelineByPropertyName = true)]
        public override ulong Id { get; set; }

        [Parameter()]
        [ValidateSet(nameof(JobStatus.Pending), nameof(JobStatus.Successful), nameof(JobStatus.Failed))]
        public JobStatus[]? Status { get; set; }

        [Parameter()]
        public override string[] OrderBy { get; set; } = ["!id"];

        protected override void BeginProcessing()
        {
            if (Status != null)
            {
                Query.Add("status__in", string.Join(',', Status.Select(s => $"{s}".ToLowerInvariant())));
            }
            SetupCommonQuery();
        }
        protected override void ProcessRecord()
        {
            var path = Id switch
            {
                > 0 => $"{WorkflowApprovalTemplate.PATH}{Id}/approvals/",
                _ => WorkflowApproval.PATH
            };
            foreach (var resultSet in GetResultSet<WorkflowApproval>(path, Query, All))
            {
                WriteObject(resultSet.Results, true);
            }
        }
    }

    public abstract class WorkflowApprovalProcessCommand : APICmdletBase
    {
        protected abstract string Command { get; }

        [Parameter(ValueFromPipelineByPropertyName = true)]
        [ValidateSet(nameof(ResourceType.WorkflowApproval))]
        public ResourceType Type { get; set; } = ResourceType.WorkflowApproval;
        [Parameter(Mandatory = true, ValueFromPipelineByPropertyName = true, ValueFromPipeline = true, Position = 0)]
        public ulong Id { get; set; }

        protected readonly HashSet<ulong> treatedIds = [];

        protected override void ProcessRecord()
        {
            if (treatedIds.Contains(Id))
            {
                return;
            }

            var result = CreateResource<string>($"{WorkflowApproval.PATH}{Id}/{Command}/");
            if (result == null)
            {
                return;
            }
            treatedIds.Add(Id);
        }
        protected override void EndProcessing()
        {
            if (treatedIds.Count == 0)
            {
                return;
            }

            var query = HttpUtility.ParseQueryString("");
            query.Add("id__in", string.Join(',', treatedIds));
            query.Add("page_size", $"{treatedIds.Count}");
            foreach (var resultSet in GetResultSet<WorkflowApproval>(WorkflowApproval.PATH, query, false))
            {
                WriteObject(resultSet.Results, true);
            }
        }
    }

    [Cmdlet(VerbsLifecycle.Approve, "Workflow")]
    [OutputType(typeof(WorkflowApproval))]
    public class ApproveWorkflowApprovalCommand : WorkflowApprovalProcessCommand
    {
        protected override string Command => "approve";
    }

    [Cmdlet(VerbsLifecycle.Deny, "Workflow")]
    [OutputType(typeof(WorkflowApproval))]
    public class DenyWorkflowApprovalCommand : WorkflowApprovalProcessCommand
    {
        protected override string Command => "deny";
    }
}
