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
            if (IdSet.Count == 1)
            {
                var res = GetResource<Label>($"{Label.PATH}{IdSet.First()}/");
                WriteObject(res);
            }
            else
            {
                Query.Add("id__in", string.Join(',', IdSet));
                Query.Add("page_size", $"{IdSet.Count}");
                foreach (var resultSet in GetResultSet<Label>(Label.PATH, Query, true))
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

    [Cmdlet(VerbsCommon.New, "Label", SupportsShouldProcess = true)]
    [OutputType(typeof(Label))]
    public class NewLabelCommand : APICmdletBase
    {
        [Parameter(Mandatory = true, ValueFromPipeline = true)]
        public string Name { get; set; } = string.Empty;

        [Parameter(Mandatory = true)]
        [ValidateRange(1, ulong.MaxValue)]
        public ulong Organization { get; set; }

        protected override void ProcessRecord()
        {
            var sendData = new Dictionary<string, object>()
            {
                { "name", Name },
                { "organization", Organization },
            };

            var newDescription = string.Join(", ", sendData.Select(kv => $"{kv.Key} = {kv.Value}"));
            if (ShouldProcess($"{{ {newDescription} }}"))
            {
                try
                {
                    var apiResult = CreateResource<Label>(Label.PATH, sendData);
                    WriteObject(apiResult.Contents, false);
                }
                catch (RestAPIException) { }
            }
        }
    }
}
