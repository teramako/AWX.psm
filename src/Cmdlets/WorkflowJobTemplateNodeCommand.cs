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
            if (Type != null && Type != ResourceType.WorkflowJobTemplateNode)
            {
                return;
            }
            foreach (var id in Id)
            {
                IdSet.Add(id);
            }
        }
        protected override void EndProcessing()
        {
            if (IdSet.Count == 1)
            {
                var res = GetResource<WorkflowJobTemplateNode>($"{WorkflowJobTemplateNode.PATH}{IdSet.First()}/");
                WriteObject(res);
            }
            else
            {
                Query.Add("id__in", string.Join(',', IdSet));
                Query.Add("page_size", $"{IdSet.Count}");
                foreach (var resultSet in GetResultSet<WorkflowJobTemplateNode>(WorkflowJobTemplateNode.PATH, Query, true))
                {
                    WriteObject(resultSet.Results, true);
                }
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

    [Cmdlet(VerbsCommon.Find, "WorkflowJobTemplateNodeFor")]
    [OutputType(typeof(WorkflowJobTemplateNode))]
    public class FindWorkflowJobTemplateNodeForCommand : FindCmdletBase
    {
        [Parameter(ValueFromPipelineByPropertyName = true)]
        [ValidateSet(nameof(ResourceType.WorkflowJobTemplateNode))]
        public override ResourceType Type { get; set; }
        [Parameter(Mandatory = true, ValueFromPipelineByPropertyName = true)]
        public override ulong Id { get; set; }

        [Parameter(Mandatory = true, Position = 0)]
        public NodeType For { get; set; }

        [Parameter()]
        public override string[] OrderBy { get; set; } = ["!id"];

        public enum NodeType
        {
            Always, Failure, Success
        }

        protected override void BeginProcessing()
        {
            SetupCommonQuery();
        }
        protected override void ProcessRecord()
        {
            var path = For switch
            {
                NodeType.Always => $"{WorkflowJobTemplateNode.PATH}{Id}/always_nodes/",
                NodeType.Failure => $"{WorkflowJobTemplateNode.PATH}{Id}/failure_nodes/",
                NodeType.Success => $"{WorkflowJobTemplateNode.PATH}{Id}/success_nodes/",
                _ => throw new ArgumentException()
            };
            foreach (var resultSet in GetResultSet<WorkflowJobTemplateNode>(path, Query, All))
            {
                WriteObject(resultSet.Results, true);
            }
        }
    }
}
