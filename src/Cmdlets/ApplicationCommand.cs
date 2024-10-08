using AWX.Resources;
using System.Management.Automation;

namespace AWX.Cmdlets
{
    [Cmdlet(VerbsCommon.Get, "Application")]
    [OutputType(typeof(Application))]
    public class GetApplicationCommand : GetCommandBase<Application>
    {
        protected override ResourceType AcceptType => ResourceType.OAuth2Application;

        protected override void ProcessRecord()
        {
            GatherResourceId();
        }
        protected override void EndProcessing()
        {
            WriteObject(GetResultSet(), true);
        }
    }
    [Cmdlet(VerbsCommon.Find, "Application", DefaultParameterSetName = "All")]
    [OutputType(typeof(Application))]
    public class FindApplicationCommand : FindCommandBase
    {
        [Parameter(Mandatory = true, ParameterSetName = "AssociatedWith", ValueFromPipelineByPropertyName = true)]
        [ValidateSet(nameof(ResourceType.Organization), nameof(ResourceType.User))]
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
                ResourceType.Organization => $"{Organization.PATH}{Id}/applications/",
                ResourceType.User => $"{User.PATH}{Id}/applications/",
                _ => Application.PATH
            };
            foreach (var resultSet in GetResultSet<Application>(path, Query, All))
            {
                WriteObject(resultSet.Results, true);
            }
        }
    }

    [Cmdlet(VerbsCommon.New, "Application", SupportsShouldProcess = true)]
    [OutputType(typeof(Application))]
    public class NewApplicationCommand : APICmdletBase
    {
        [Parameter(Mandatory = true, Position = 0)]
        public string Name { get; set; } = string.Empty;

        [Parameter()]
        [AllowEmptyString]
        public string? Description { get; set; }

        [Parameter(Mandatory = true)]
        public ulong Organization { get; set; }

        [Parameter(Mandatory = true)]
        [ValidateSet("password", "authorization-code")]
        public string AuthorizationGrantType { get; set; } = string.Empty;

        [Parameter()]
        [AllowEmptyString]
        public string? RedirectUris { get; set; }

        [Parameter(Mandatory = true)]
        public ApplicationClientType ClientType { get; set; }

        [Parameter()]
        public SwitchParameter SkipAuthorization { get; set; }

        protected override void ProcessRecord()
        {
            var sendData = new Dictionary<string, object>()
            {
                { "name", Name },
                { "organization", Organization },
                { "authorization_grant_type", AuthorizationGrantType },
                { "client_type", $"{ClientType}".ToLowerInvariant() }
            };
            if (Description != null)
                sendData.Add("description", Description);
            if (RedirectUris != null)
                sendData.Add("redirect_uris", RedirectUris);
            if (SkipAuthorization)
                sendData.Add("skip_authorization", true);

            var dataDescription = Json.Stringify(sendData, pretty: true);
            if (ShouldProcess(dataDescription))
            {
                try
                {
                    var apiResult = CreateResource<Application>(Application.PATH, sendData);
                    if (apiResult.Contents == null)
                        return;

                    WriteObject(apiResult.Contents, false);
                }
                catch (RestAPIException) { }
            }
        }
    }

    [Cmdlet(VerbsData.Update, "Application", SupportsShouldProcess = true)]
    [OutputType(typeof(Application))]
    public class UpdateApplicationCommand : UpdateCommandBase<Application>
    {
        [Parameter(Mandatory = true, Position = 0)]
        [ResourceIdTransformation(AcceptableTypes = [ResourceType.OAuth2Application])]
        public override ulong Id { get; set; }

        [Parameter()]
        public string? Name { get; set; }

        [Parameter()]
        [AllowEmptyString]
        public string? Description { get; set; }

        [Parameter()]
        public ulong? Organization { get; set; }

        [Parameter()]
        public string? RedirectUris { get; set; }

        [Parameter()]
        public ApplicationClientType? ClientType { get; set; }

        [Parameter()]
        public SwitchParameter SkipAuthorization { get; set; }

        protected override Dictionary<string, object?> CreateSendData()
        {
            var sendData = new Dictionary<string, object?>();
            if (!string.IsNullOrEmpty(Name))
                sendData.Add("name", Name);
            if (Description != null)
                sendData.Add("description", Description);
            if (Organization != null)
                sendData.Add("organization", Organization);
            if (RedirectUris != null)
                sendData.Add("redirect_uris", RedirectUris);
            if (ClientType != null)
                sendData.Add("client_type", $"{ClientType}".ToLowerInvariant());

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

    [Cmdlet(VerbsCommon.Remove, "Application", SupportsShouldProcess = true, ConfirmImpact = ConfirmImpact.High)]
    public class RemoveApplicationCommand : APICmdletBase
    {
        [Parameter(Mandatory = true, ValueFromPipeline = true, Position = 0)]
        [ResourceIdTransformation(AcceptableTypes = [ResourceType.OAuth2Application])]
        public ulong Id { get; set; }

        [Parameter()]
        public SwitchParameter Force { get; set; }

        private void Delete(ulong id)
        {
            if (Force || ShouldProcess($"Application [{id}]", "Delete completely"))
            {
                try
                {
                    var apiResult = DeleteResource($"{Application.PATH}{id}/");
                    if (apiResult?.IsSuccessStatusCode ?? false)
                    {
                        WriteVerbose($"Application {id} is deleted.");
                    }
                }
                catch (RestAPIException) { }
            }
        }

        protected override void ProcessRecord()
        {
            Delete(Id);
        }
    }
}
