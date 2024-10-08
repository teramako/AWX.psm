using AWX.Resources;
using System.Management.Automation;

namespace AWX.Cmdlets
{
    [Cmdlet(VerbsCommon.Get, "Label")]
    [OutputType(typeof(Label))]
    public class GetLabelCommand : GetCommandBase<Label>
    {
        protected override ResourceType AcceptType => ResourceType.Label;

        protected override void ProcessRecord()
        {
            GatherResourceId();
        }
        protected override void EndProcessing()
        {
            WriteObject(GetResultSet(), true);
        }
    }

    [Cmdlet(VerbsCommon.Find, "Label", DefaultParameterSetName = "All")]
    [OutputType(typeof(Label))]
    public class FindLabelCommand : FindCommandBase
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

            var dataDescription = Json.Stringify(sendData, pretty: true);
            if (ShouldProcess(dataDescription))
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

    [Cmdlet(VerbsCommon.Add, "Label", SupportsShouldProcess = true)]
    public class AddLabelCommand : APICmdletBase
    {
        [Parameter(Mandatory = true, ValueFromPipeline = true, Position = 0)]
        [ResourceIdTransformation(AcceptableTypes = [ResourceType.Label])]
        public ulong Id { get; set; }

        [Parameter(Mandatory = true, Position = 1)]
        [ResourceTransformation(AcceptableTypes = [
                ResourceType.Inventory,
                ResourceType.JobTemplate,
                ResourceType.Schedule,
                ResourceType.WorkflowJobTemplate,
                ResourceType.WorkflowJobTemplateNode
        ])]
        public IResource To { get; set; } = new Resource(0, 0);

        protected override void ProcessRecord()
        {
            var path = To.Type switch
            {
                ResourceType.Inventory => $"{Inventory.PATH}{To.Id}/labels/",
                ResourceType.JobTemplate => $"{JobTemplate.PATH}{To.Id}/labels/",
                ResourceType.Schedule => $"{Schedule.PATH}{To.Id}/labels/",
                ResourceType.WorkflowJobTemplate => $"{WorkflowJobTemplate.PATH}{To.Id}/labels/",
                ResourceType.WorkflowJobTemplateNode => $"{WorkflowJobTemplateNode.PATH}{To.Id}/labels/",
                _ => throw new ArgumentException($"Invalid resource type: {To.Type}")
            };

            if (ShouldProcess($"Label {Id}", $"Add to {To.Type} [{To.Id}]"))
            {
                var sendData = new Dictionary<string, object>()
                {
                    { "id", Id },
                };
                try
                {
                    var apiResult = CreateResource<string>(path, sendData);
                    if (apiResult.Response.IsSuccessStatusCode)
                    {
                        WriteVerbose($"Label [{Id}] is associated to {To.Type} [{To.Id}].");
                    }
                }
                catch (RestAPIException) { }
            }
        }
    }

    [Cmdlet(VerbsCommon.Remove, "Label", SupportsShouldProcess = true, ConfirmImpact = ConfirmImpact.High)]
    public class RemoveLabelCommand : APICmdletBase
    {
        [Parameter(Mandatory = true, Position = 0)]
        [ResourceTransformation(AcceptableTypes = [
                ResourceType.Inventory,
                ResourceType.JobTemplate,
                ResourceType.Schedule,
                ResourceType.WorkflowJobTemplate,
                ResourceType.WorkflowJobTemplateNode
        ])]
        public IResource? From { get; set; }

        [Parameter(Mandatory = true, Position = 1, ValueFromPipeline = true)]
        [ResourceIdTransformation(AcceptableTypes = [ResourceType.Label])]
        public ulong Id { get; set; }

        [Parameter()]
        public SwitchParameter Force { get; set; }

        protected override void ProcessRecord()
        {
            if (From == null) return;

            var path = From.Type switch
            {
                ResourceType.Inventory => $"{Inventory.PATH}{From.Id}/labels/",
                ResourceType.JobTemplate => $"{JobTemplate.PATH}{From.Id}/labels/",
                ResourceType.Schedule => $"{Schedule.PATH}{From.Id}/labels/",
                ResourceType.WorkflowJobTemplate => $"{WorkflowJobTemplate.PATH}{From.Id}/labels/",
                ResourceType.WorkflowJobTemplateNode => $"{WorkflowJobTemplateNode.PATH}{From.Id}/labels/",
                _ => throw new ArgumentException($"Invalid resource type: {From.Type}")
            };

            if (Force || ShouldProcess($"Label {Id}", $"Disassociate from {From.Type} [{From.Id}]"))
            {
                var sendData = new Dictionary<string, object>()
                {
                    { "id",  Id },
                    { "disassociate", true }
                };
                try
                {
                    var apiResult = CreateResource<string>(path, sendData);
                    if (apiResult.Response.IsSuccessStatusCode)
                    {
                        WriteVerbose($"Label {Id} is disassociate from {From.Type} [{From.Id}].");
                    }
                }
                catch (RestAPIException) { }
            }

        }
    }

    [Cmdlet(VerbsData.Update, "Label", SupportsShouldProcess = true)]
    [OutputType(typeof(Label))]
    public class UpdateLabelCommand : UpdateCommandBase<Label>
    {
        [Parameter(Mandatory = true, ValueFromPipeline = true, Position = 0)]
        [ResourceIdTransformation(AcceptableTypes = [ResourceType.Label])]
        public override ulong Id { get; set; }

        [Parameter()]
        public string Name { get; set; } = string.Empty;

        [Parameter()]
        [ResourceIdTransformation(AcceptableTypes = [ResourceType.Organization])]
        public ulong Organization { get; set; }

        protected override Dictionary<string, object?> CreateSendData()
        {
            var sendData = new Dictionary<string, object?>();
            if (!string.IsNullOrEmpty(Name))
                sendData.Add("name", Name);
            if (Organization > 0)
                sendData.Add("organization", Organization);

            return sendData;
        }

        protected override void ProcessRecord()
        {
            if (TryPatch(Id, out var result))
            {
                WriteObject(result, false);
            }
        }
    }
}
