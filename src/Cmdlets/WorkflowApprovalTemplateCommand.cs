using AWX.Resources;
using System.Management.Automation;

namespace AWX.Cmdlets
{
    [Cmdlet(VerbsCommon.Get, "WorkflowApprovalTemplate")]
    [OutputType(typeof(WorkflowApprovalTemplate))]
    public class GetWorkflowApprovalTemplate : GetCommandBase<WorkflowApprovalTemplate>
    {
        protected override string ApiPath => WorkflowApprovalTemplate.PATH;
        protected override ResourceType AcceptType => ResourceType.WorkflowApprovalTemplate;

        protected override void ProcessRecord()
        {
            WriteObject(GetResource(), true);
        }
    }
}
