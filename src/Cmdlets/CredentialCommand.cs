using AWX.Resources;
using System.Management.Automation;

namespace AWX.Cmdlets
{
    [Cmdlet(VerbsCommon.Get, "Credential")]
    [OutputType(typeof(Credential))]
    public class GetCredentialCommand : GetCmdletBase
    {
        protected override void ProcessRecord()
        {
            if (Type != null && Type != ResourceType.Credential)
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
                var res = GetResource<Credential>($"{Credential.PATH}{IdSet.First()}/");
                WriteObject(res);
            }
            else
            {
                Query.Add("id__in", string.Join(',', IdSet));
                Query.Add("page_size", $"{IdSet.Count}");
                foreach (var resultSet in GetResultSet<Credential>(Credential.PATH, Query, true))
                {
                    WriteObject(resultSet.Results, true);
                }
            }
        }
    }
    [Cmdlet(VerbsCommon.Find, "Credential", DefaultParameterSetName = "All")]
    [OutputType(typeof(Credential))]
    public class FindCredentialCommand : FindCmdletBase
    {
        [Parameter(Mandatory = true, ParameterSetName = "AssociatedWith", ValueFromPipelineByPropertyName = true)]
        [ValidateSet(nameof(ResourceType.Organization),
                     nameof(ResourceType.User),
                     nameof(ResourceType.Team),
                     nameof(ResourceType.CredentialType),
                     nameof(ResourceType.InventorySource),
                     nameof(ResourceType.InventoryUpdate),
                     nameof(ResourceType.JobTemplate),
                     nameof(ResourceType.Job),
                     nameof(ResourceType.Schedule),
                     nameof(ResourceType.WorkflowJobTemplateNode),
                     nameof(ResourceType.WorkflowJobNode))]
        public override ResourceType Type { get; set; }
        [Parameter(Mandatory = true, ParameterSetName = "AssociatedWith", ValueFromPipelineByPropertyName = true)]
        public override ulong Id { get; set; }

        [Parameter()]
        public string? Kind { get; set; }

        /// <summary>
        /// Only affected for an Organization
        /// </summary>
        [Parameter()]
        public SwitchParameter Galaxy { get; set; }

        [Parameter()]
        public override string[] OrderBy { get; set; } = ["id"];

        protected override void BeginProcessing()
        {
            if (Kind != null)
            {
                Query.Add("chain__credential_type__namespace__icontains", Kind);
            }
            SetupCommonQuery();
        }
        protected override void ProcessRecord()
        {
            var path = Type switch
            {
                ResourceType.Organization => $"{Organization.PATH}{Id}/" + (Galaxy ? "galaxy_credentials/" : "credentials/"),
                ResourceType.User => $"{User.PATH}{Id}/credentials/",
                ResourceType.Team => $"{Team.PATH}{Id}/credentials/",
                ResourceType.CredentialType => $"{CredentialType.PATH}{Id}/credentials/",
                ResourceType.InventorySource => $"{InventorySource.PATH}{Id}/credentials/",
                ResourceType.InventoryUpdate => $"{InventoryUpdateJob.PATH}{Id}/credentials/",
                ResourceType.JobTemplate => $"{JobTemplate.PATH}{Id}/credentials/",
                ResourceType.Job => $"{JobTemplateJob.PATH}{Id}/credentials/",
                ResourceType.Schedule => $"{Schedule.PATH}{Id}/credentials/",
                ResourceType.WorkflowJobTemplateNode => $"{WorkflowJobTemplateNode.PATH}{Id}/credentials/",
                ResourceType.WorkflowJobNode => $"{WorkflowJobNode.PATH}{Id}/credentials/",
                _ => Credential.PATH
            };
            foreach (var resultSet in GetResultSet<Credential>(path, Query, All))
            {
                WriteObject(resultSet.Results, true);
            }
        }
    }

    [Cmdlet(VerbsCommon.New, "Credential", SupportsShouldProcess = true)]
    [OutputType(typeof(Credential))]
    public class NewCredentialCommand : APICmdletBase
    {
        [Parameter(Mandatory = true)]
        [ResourceIdTransformation(AcceptableTypes = [ResourceType.CredentialType])]
        public ulong CredentialType { get; set; }

        [Parameter(Mandatory = true)]
        public string Name { get; set; } = string.Empty;

        [Parameter()]
        public string Description { get; set; } = string.Empty;

        [Parameter()]
        public IDictionary Inputs { get; set; } = new Hashtable();

        [Parameter()]
        [ResourceTransformation(AcceptableTypes = [
                ResourceType.Organization,
                ResourceType.Team,
                ResourceType.User
        ])]
        public IResource? Owner { get; set; }

        protected override void ProcessRecord()
        {
            var sendData = new Dictionary<string, object>()
            {
                { "name", Name },
                { "credential_type", CredentialType },
                { "inputs", Inputs }
            };
            if (!string.IsNullOrEmpty(Description))
                sendData.Add("description", Description);
            if (Owner != null)
            {
                switch (Owner.Type)
                {
                    case ResourceType.Organization:
                        sendData.Add("organization", Owner.Id);
                        break;
                    case ResourceType.Team:
                        sendData.Add("team", Owner.Id);
                        break;
                    case ResourceType.User:
                        sendData.Add("user", Owner.Id);
                        break;
                }
            }
            else
            {
                var userId = ApiConfig.Instance.UserId;
                if (userId != null)
                    sendData.Add("user", userId);
            }

            // FIXME: Validation of Inputs value from CredentialType data

            var dataDescription = Json.Stringify(sendData, pretty: true);
            if (ShouldProcess(dataDescription))
            {
                try
                {
                    var apiResult = CreateResource<Credential>(Credential.PATH, sendData);
                    if (apiResult.Contents == null)
                        return;

                    WriteObject(apiResult.Contents, false);
                }
                catch (RestAPIException) { }
            }
        }
    }

    [Cmdlet(VerbsCommon.Remove, "Credential", SupportsShouldProcess = true, ConfirmImpact = ConfirmImpact.High)]
    public class RemoveCredentialCommand : APICmdletBase
    {
        [Parameter(Mandatory = true, ValueFromPipeline = true, Position = 0)]
        [ResourceIdTransformation(AcceptableTypes = [ResourceType.Credential])]
        public ulong Id { get; set; }

        [Parameter()]
        public SwitchParameter Force { get; set; }

        protected override void ProcessRecord()
        {
            if (Force || ShouldProcess($"Credential [{Id}]", "Delete completely"))
            {
                try
                {
                    var apiResult = DeleteResource($"{Credential.PATH}{Id}/");
                    if (apiResult?.IsSuccessStatusCode ?? false)
                    {
                        WriteVerbose($"Credential {Id} is removed.");
                    }
                }
                catch (RestAPIException) { }
            }
        }
    }
}
