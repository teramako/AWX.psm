using AWX.Resources;
using System.Management.Automation;

namespace AWX.Cmdlets
{
    [Cmdlet(VerbsCommon.Get, "WorkflowJobTemplateNode")]
    [OutputType(typeof(WorkflowJobTemplateNode))]
    public class GetWorkflowJobTemplateNodeCommand : GetCmdletBase
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
            foreach (var resultSet in GetResultSet<WorkflowJobTemplateNode>($"{WorkflowJobTemplateNode.PATH}?{Query}", true))
            {
                WriteObject(resultSet.Results, true);
            }
        }
    }

    [Cmdlet(VerbsCommon.Find, "WorkflowJobTemplateNode", DefaultParameterSetName = "All")]
    [OutputType(typeof(WorkflowJobTemplateNode))]
    public class FindWorkflowJobTemplateNodeCommand : FindCmdletBase
    {
        [Parameter(ParameterSetName = "AssociatedWith", ValueFromPipelineByPropertyName = true)]
        [ValidateSet(nameof(ResourceType.WorkflowJobTemplate))]
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
            var path = Id switch
            {
                > 0 => $"{WorkflowJobTemplate.PATH}{Id}/workflow_nodes/",
                _ => WorkflowJobTemplateNode.PATH
            };
            foreach (var resultSet in GetResultSet<WorkflowJobTemplateNode>(path, Query, All))
            {
                WriteObject(resultSet.Results, true);
            }
        }
    }
}
