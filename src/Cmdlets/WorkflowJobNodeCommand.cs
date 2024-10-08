using AWX.Resources;
using System.Management.Automation;

namespace AWX.Cmdlets
{
    [Cmdlet(VerbsCommon.Get, "WorkflowJobNode")]
    [OutputType(typeof(WorkflowJobNode))]
    public class GetWorkflowJobNodeCommand : GetCommandBase<WorkflowJobNode>
    {
        protected override ResourceType AcceptType => ResourceType.WorkflowJobNode;

        protected override void ProcessRecord()
        {
            GatherResourceId();
        }
        protected override void EndProcessing()
        {
            WriteObject(GetResultSet(), true);
        }
    }

    [Cmdlet(VerbsCommon.Find, "WorkflowJobNode", DefaultParameterSetName = "All")]
    [OutputType(typeof(WorkflowJobNode))]
    public class FindWorkflowJobNodeCommand : FindCommandBase
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
    public class FindWorkflowJobNodeForCommand : FindCommandBase
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
