using AWX.Resources;
using System.Management.Automation;

namespace AWX.Cmdlets
{
    [Cmdlet(VerbsCommon.Get, "ActivityStream")]
    [OutputType(typeof(ActivityStream))]
    public class GetActivityStreamCommand : GetCmdletBase
    {
        protected override void ProcessRecord()
        {
            if (Type != null && Type != ResourceType.ActivityStream)
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
            Query.Add("id__in", string.Join(',', IdSet));
            Query.Add("page_size", $"{IdSet.Count}");
            foreach (var resultSet in GetResultSet<ActivityStream>($"{ActivityStream.PATH}?{Query}", true))
            {
                WriteObject(resultSet.Results, true);
            }
        }
    }

    [Cmdlet(VerbsCommon.Find, "ActivityStream", DefaultParameterSetName = "All")]
    [OutputType(typeof(ActivityStream))]
    public class FindActivityStreamCommand : FindCmdletBase
    {
        [Parameter(Mandatory = true, ParameterSetName = "AssociatedWith", ValueFromPipelineByPropertyName = true)]
        [ValidateSet(nameof(ResourceType.OAuth2Application),
                     nameof(ResourceType.OAuth2AccessToken),
                     nameof(ResourceType.Organization),
                     nameof(ResourceType.User),
                     nameof(ResourceType.Project),
                     nameof(ResourceType.Team),
                     nameof(ResourceType.Credential),
                     nameof(ResourceType.CredentialType),
                     nameof(ResourceType.Inventory),
                     nameof(ResourceType.InventorySource),
                     nameof(ResourceType.Group),
                     nameof(ResourceType.Host),
                     nameof(ResourceType.JobTemplate),
                     nameof(ResourceType.Job),
                     nameof(ResourceType.AdHocCommand),
                     nameof(ResourceType.WorkflowJobTemplate),
                     nameof(ResourceType.WorkflowJob),
                     nameof(ResourceType.ExecutionEnvironment))]
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
            var path = Type switch
            {
                ResourceType.OAuth2Application => $"{Application.PATH}{Id}/activity_stream/",
                ResourceType.OAuth2AccessToken => $"{OAuth2AccessToken.PATH}{Id}/activity_stream/",
                ResourceType.Organization => $"{Organization.PATH}{Id}/activity_stream/",
                ResourceType.User => $"{User.PATH}{Id}/activity_stream/",
                ResourceType.Project => $"{Project.PATH}{Id}/activity_stream/",
                ResourceType.Team => $"{Team.PATH}{Id}/activity_stream/",
                ResourceType.Credential => $"{Credential.PATH}{Id}/activity_stream/",
                ResourceType.CredentialType => $"{CredentialType.PATH}{Id}/activity_stream/",
                ResourceType.Inventory => $"{Inventory.PATH}{Id}/activity_stream/",
                ResourceType.InventorySource => $"{InventorySource.PATH}{Id}/activity_stream/",
                ResourceType.Group => $"{Group.PATH}{Id}/activity_stream/",
                ResourceType.Host => $"{Host.PATH}{Id}/activity_stream/",
                ResourceType.JobTemplate => $"{JobTemplate.PATH}{Id}/activity_stream/",
                ResourceType.Job => $"{JobTemplateJob.PATH}{Id}/activity_stream/",
                ResourceType.AdHocCommand => $"{AdHocCommand.PATH}{Id}/activity_stream/",
                ResourceType.WorkflowJobTemplate => $"{WorkflowJobTemplate.PATH}{Id}/activity_stream/",
                ResourceType.WorkflowJob => $"{WorkflowJob.PATH}{Id}/activity_stream/",
                ResourceType.ExecutionEnvironment => $"{ExecutionEnvironment.PATH}{Id}/activity_stream/",
                _ => ActivityStream.PATH
            };
            foreach (var resultSet in GetResultSet<ActivityStream>(path, Query, All))
            {
                WriteObject(resultSet.Results, true);
            }
        }
    }
}
