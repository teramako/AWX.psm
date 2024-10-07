using AWX.Resources;
using System.Management.Automation;

namespace AWX.Cmdlets
{
    [Cmdlet(VerbsCommon.Get, "WorkflowJobNode")]
    [OutputType(typeof(WorkflowJobNode))]
    public class GetWorkflowJobNodeCommand : GetCommandBase
    {
        protected override void ProcessRecord()
        {
            if (Type != null && Type != ResourceType.WorkflowJobNode)
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
                var res = GetResource<WorkflowJobNode>($"{WorkflowJobNode.PATH}{IdSet.First()}/");
                WriteObject(res);
            }
            else
            {
                Query.Add("id__in", string.Join(',', IdSet));
                Query.Add("page_size", $"{IdSet.Count}");
                foreach (var resultSet in GetResultSet<WorkflowJobNode>(WorkflowJobNode.PATH, Query, true))
                {
                    WriteObject(resultSet.Results, true);
                }
            }
        }
    }

    [Cmdlet(VerbsCommon.Find, "WorkflowJobNode", DefaultParameterSetName = "All")]
    [OutputType(typeof(WorkflowJobNode))]
    public class FindWorkflowJobNodeCommand : FindCmdletBase
    {
        [Parameter(ParameterSetName = "AssociatedWith", ValueFromPipelineByPropertyName = true)]
        [ValidateSet(nameof(ResourceType.WorkflowJob))]
        public override ResourceType Type { get; set; }
        [Parameter(Mandatory = true, ParameterSetName = "AssociatedWith", ValueFromPipelineByPropertyName = true)]
        public override ulong Id { get; set; }

        [Parameter()]
        public override string[] OrderBy { get; set; } = ["!id"];

        protected override void BeginProcessing()
        {
            SetupCommonQuery();
        }
        protected override void ProcessRecord()
        {
            var path = Id switch
            {
                > 0 => $"{WorkflowJob.PATH}{Id}/workflow_nodes/",
                _ => WorkflowJobNode.PATH
            };
            foreach (var resultSet in GetResultSet<WorkflowJobNode>(path, Query, All))
            {
                WriteObject(resultSet.Results, true);
            }
        }
    }

    [Cmdlet(VerbsCommon.Find, "WorkflowJobNodeFor")]
    [OutputType(typeof(WorkflowJobNode))]
    public class FindWorkflowJobNodeForCommand : FindCmdletBase
    {
        [Parameter(ValueFromPipelineByPropertyName = true)]
        [ValidateSet(nameof(ResourceType.WorkflowJobNode))]
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
                NodeType.Always => $"{WorkflowJobNode.PATH}{Id}/always_nodes/",
                NodeType.Failure => $"{WorkflowJobNode.PATH}{Id}/failure_nodes/",
                NodeType.Success => $"{WorkflowJobNode.PATH}{Id}/success_nodes/",
                _ => throw new ArgumentException()
            };
            foreach (var resultSet in GetResultSet<WorkflowJobNode>(path, Query, All))
            {
                WriteObject(resultSet.Results, true);
            }
        }
    }
}
