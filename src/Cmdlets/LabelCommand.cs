using AWX.Resources;
using System.Management.Automation;

namespace AWX.Cmdlets
{
    [Cmdlet(VerbsCommon.Get, "Label")]
    [OutputType(typeof(Label))]
    public class GetLabelCommand : GetCmdletBase
    {
        protected override void ProcessRecord()
        {
            if (Type != null && Type != ResourceType.Label)
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
            string path;
            if (IdSet.Count == 1)
            {
                path = $"{Label.PATH}{IdSet.First()}/";
                var res = GetResource<Label>(path);
                WriteObject(res);
            }
            else
            {
                path = Label.PATH;
                Query.Add("id__in", string.Join(',', IdSet));
                Query.Add("page_size", $"{IdSet.Count}");
                foreach (var resultSet in GetResultSet<Label>(path, Query))
                {
                    WriteObject(resultSet.Results, true);
                }
            }
        }
    }
    [Cmdlet(VerbsCommon.Find, "Label", DefaultParameterSetName = "All")]
    [OutputType(typeof(Label))]
    public class FindLabelCommand : FindCmdletBase
    {
        [Parameter(Mandatory = true, ParameterSetName = "AssociatedWith", ValueFromPipelineByPropertyName = true)]
        [ValidateSet(nameof(ResourceType.Inventory),
                     nameof(ResourceType.JobTemplate),
                     nameof(ResourceType.Job),
                     nameof(ResourceType.Schedule),
                     nameof(ResourceType.WorkflowJobTemplate),
                     nameof(ResourceType.WorkflowJob),
                     nameof(ResourceType.WorkflowJobTemplateNode),
                     nameof(ResourceType.WorkflowJobNode))]
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
                ResourceType.Inventory => $"{Inventory.PATH}{Id}/labels/",
                ResourceType.JobTemplate => $"{JobTemplate.PATH}{Id}/labels/",
                ResourceType.Job => $"{JobTemplateJob.PATH}{Id}/labels/",
                ResourceType.Schedule => $"{Schedule.PATH}{Id}/labels/",
                ResourceType.WorkflowJobTemplate => $"{WorkflowJobTemplate.PATH}{Id}/labels/",
                ResourceType.WorkflowJob => $"{WorkflowJob.PATH}{Id}/labels/",
                ResourceType.WorkflowJobTemplateNode => $"{WorkflowJobTemplateNode.PATH}{Id}/labels/",
                ResourceType.WorkflowJobNode => $"{WorkflowJobNode.PATH}{Id}/labels/",
                _ => Label.PATH
            };
            foreach (var resultSet in GetResultSet<Label>(path, Query, All))
            {
                WriteObject(resultSet.Results, true);
            }
        }
    }
}
