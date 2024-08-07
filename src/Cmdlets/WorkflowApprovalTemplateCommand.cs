using AWX.Resources;
using System.Management.Automation;

namespace AWX.Cmdlets
{
    [Cmdlet(VerbsCommon.Get, "WorkflowApprovalTemplate")]
    [OutputType(typeof(WorkflowApprovalTemplate))]
    public class GetWorkflowApprovalTemplate : GetCmdletBase
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
                var res = GetResource<WorkflowApprovalTemplate>($"{WorkflowApprovalTemplate.PATH}{id}/");
                if (res != null)
                {
                    WriteObject(res);
                }
            }
        }
    }
}
