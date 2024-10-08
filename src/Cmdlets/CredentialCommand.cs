using AWX.Resources;
using System.Collections;
using System.Management.Automation;

namespace AWX.Cmdlets
{
    [Cmdlet(VerbsCommon.Get, "Credential")]
    [OutputType(typeof(Credential))]
    public class GetCredentialCommand : GetCommandBase<Credential>
    {
        protected override ResourceType AcceptType => ResourceType.Credential;

        protected override void ProcessRecord()
        {
            GatherResourceId();
        }
        protected override void EndProcessing()
        {
            WriteObject(GetResultSet(), true);
        }
    }

    [Cmdlet(VerbsCommon.Find, "Credential", DefaultParameterSetName = "All")]
    [OutputType(typeof(Credential))]
    public class FindCredentialCommand : FindCommandBase
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
        [AllowEmptyString]
        public string? Description { get; set; }

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
            if (Description != null)
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

    [Cmdlet(VerbsLifecycle.Register, "Credential", SupportsShouldProcess = true)]
    [OutputType(typeof(bool))]
    public class RegisterCredentialCommand : RegistrationCommandBase<Credential>
    {
        [Parameter(Mandatory = true, ValueFromPipeline = true, Position = 0)]
        [ResourceIdTransformation(AcceptableTypes = [ResourceType.Credential])]
        public ulong Id { get; set; }

        [Parameter(Mandatory = true, Position = 1)]
        [ResourceTransformation(AcceptableTypes = [
                ResourceType.InventorySource,
                ResourceType.JobTemplate,
                ResourceType.Schedule,
                ResourceType.WorkflowJobTemplateNode
        ])]
        public IResource To { get; set; } = new Resource(0 ,0);

        protected override void ProcessRecord()
        {
            var path = To.Type switch
            {
                ResourceType.Organization => $"{Organization.PATH}{To.Id}/galaxy_credentials/",
                ResourceType.InventorySource => $"{InventorySource.PATH}{To.Id}/credentials/",
                ResourceType.JobTemplate => $"{JobTemplate.PATH}{To.Id}/credentials/",
                ResourceType.Schedule => $"{Schedule.PATH}{To.Id}/credentials/",
                ResourceType.WorkflowJobTemplateNode => $"{WorkflowJobTemplateNode.PATH}{To.Id}/credentials/",
                _ => throw new ArgumentException($"Invalid resource type: {To.Type}")
            };
            WriteObject(Register(path, Id, To));
        }
    }
    [Cmdlet(VerbsLifecycle.Unregister, "Credential", SupportsShouldProcess = true)]
    [OutputType(typeof(bool))]
    public class UnregisterCredentialCommand : RegistrationCommandBase<Credential>
    {
        [Parameter(Mandatory = true, ValueFromPipeline = true, Position = 0)]
        [ResourceIdTransformation(AcceptableTypes = [ResourceType.Credential])]
        public ulong Id { get; set; }

        [Parameter(Mandatory = true, Position = 1)]
        [ResourceTransformation(AcceptableTypes = [
                ResourceType.InventorySource,
                ResourceType.JobTemplate,
                ResourceType.Schedule,
                ResourceType.WorkflowJobTemplateNode
        ])]
        public IResource From { get; set; } = new Resource(0 ,0);

        protected override void ProcessRecord()
        {
            var path = From.Type switch
            {
                ResourceType.Organization => $"{Organization.PATH}{From.Id}/galaxy_credentials/",
                ResourceType.InventorySource => $"{InventorySource.PATH}{From.Id}/credentials/",
                ResourceType.JobTemplate => $"{JobTemplate.PATH}{From.Id}/credentials/",
                ResourceType.Schedule => $"{Schedule.PATH}{From.Id}/credentials/",
                ResourceType.WorkflowJobTemplateNode => $"{WorkflowJobTemplateNode.PATH}{From.Id}/credentials/",
                _ => throw new ArgumentException($"Invalid resource type: {From.Type}")
            };
            WriteObject(Unregister(path, Id, From));
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
                        WriteVerbose($"Credential {Id} is deleted.");
                    }
                }
                catch (RestAPIException) { }
            }
        }
    }

    [Cmdlet(VerbsData.Update, "Credential", SupportsShouldProcess = true)]
    [OutputType(typeof(Credential))]
    public class UpdateCredentialCommand : UpdateCommandBase<Credential>
    {
        [Parameter(Mandatory = true, ValueFromPipeline = true, Position = 0)]
        [ResourceIdTransformation(AcceptableTypes = [ResourceType.Credential])]
        public override ulong Id { get; set; }

        [Parameter()]
        public string? Name { get; set; }

        [Parameter()]
        [AllowEmptyString]
        public string? Description { get; set; }

        [Parameter()]
        [ResourceIdTransformation(AcceptableTypes = [ResourceType.CredentialType])]
        public ulong? CredentialType { get; set; }

        [Parameter()]
        public IDictionary? Inputs { get; set; }

        [Parameter()]
        [ResourceIdTransformation(AcceptableTypes = [ResourceType.Organization])]
        public ulong? Organization { get; set; }

        protected override Dictionary<string, object?> CreateSendData()
        {
            var sendData = new Dictionary<string, object?>();
            if (!string.IsNullOrEmpty(Name))
                sendData.Add("name", Name);
            if (Description != null)
                sendData.Add("description", Description);
            if (CredentialType != null)
                sendData.Add("credential_type", CredentialType);
            if (Inputs != null)
                sendData.Add("inputs", Inputs);
            if (Organization != null)
                sendData.Add("organization", Organization);

            return sendData;
        }
        protected override void ProcessRecord()
        {
            // FIXME: Validation of Inputs value from CredentialType data

            if (TryPatch(Id, out var result))
            {
                WriteObject(result, false);
            }
        }
    }
}
