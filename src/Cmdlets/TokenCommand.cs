using AWX.Resources;
using System.Management.Automation;

namespace AWX.Cmdlets
{
    [Cmdlet(VerbsCommon.Get, "Token")]
    [OutputType(typeof(OAuth2AccessToken))]
    public class GetTokenCommand : GetCmdletBase
    {
        protected override void ProcessRecord()
        {
            if (Type != null && Type != ResourceType.OAuth2AccessToken)
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
                var result = GetResource<OAuth2AccessToken>($"{OAuth2AccessToken.PATH}{IdSet.First()}/");
                WriteObject(result);
            }
            else
            {
                Query.Add("id__in", string.Join(',', IdSet));
                Query.Add("page_size", $"{IdSet.Count}");
                foreach (var resultSet in GetResultSet<OAuth2AccessToken>(OAuth2AccessToken.PATH, Query, true))
                {
                    WriteObject(resultSet.Results, true);
                }
            }
        }
    }

    [Cmdlet(VerbsCommon.Find, "Token", DefaultParameterSetName = "All")]
    [OutputType(typeof(OAuth2AccessToken))]
    public class FindTokenCommand : FindCmdletBase
    {
        [Parameter(Mandatory = true, ParameterSetName = "AssociatedWith", ValueFromPipelineByPropertyName = true)]
        [ValidateSet(nameof(ResourceType.OAuth2Application),
                     nameof(ResourceType.User))]
        public override ResourceType Type { get; set; }
        [Parameter(Mandatory = true, ParameterSetName = "AssociatedWith", ValueFromPipelineByPropertyName = true)]
        public override ulong Id { get; set; }

        /// <summary>
        /// Filter by Personal Access Token(<c>Personal</c>) or
        /// User Authorized Token(<c>Authorized</c>) or both.<br/>
        /// Ignored when <c>Type</c> parameter is <c>OAuth2Application</c>
        /// </summary>
        [Parameter()]
        public ETokenType TokenType { get; set; } = ETokenType.Both;

        [Parameter()]
        public override string[] OrderBy { get; set; } = ["id"];

        public enum ETokenType
        {
            Both, Personal, Authorized
        }

        protected override void ProcessRecord()
        {
            Query.Clear();
            if (Type != ResourceType.OAuth2Application)
            {
                switch (TokenType)
                {
                    case ETokenType.Personal:
                        Query.Add("application", "null");
                        break;
                    case ETokenType.Authorized:
                        Query.Add("not__application", "null");
                        break;
                }
            }
            SetupCommonQuery();
            var path = Type switch
            {
                ResourceType.OAuth2Application => $"{Application.PATH}{Id}/tokens/",
                ResourceType.User => $"{User.PATH}{Id}/tokens/",
                _ => OAuth2AccessToken.PATH
            };
            foreach (var resultSet in GetResultSet<OAuth2AccessToken>(path, Query, All))
            {
                WriteObject(resultSet.Results, true);
            }
        }
    }

    [Cmdlet(VerbsCommon.New, "Token", SupportsShouldProcess = true, DefaultParameterSetName = "Application")]
    [OutputType(typeof(OAuth2AccessToken))]
    public class AddTokenCommand : APICmdletBase
    {
        [Parameter(Mandatory = true, ParameterSetName = "User", Position = 0)]
        public SwitchParameter ForMe { get; set; }

        [Parameter(Mandatory = true, ParameterSetName = "Application", Position = 0)]
        [Parameter(ParameterSetName = "User", Position = 1)]
        [ResourceIdTransformation(AcceptableTypes = [ResourceType.OAuth2Application])]
        public ulong? Application { get; set; }

        [Parameter()]
        [ValidateSet("read", "write")]
        public string Scope { get; set; } = "write";

        [Parameter()]
        [AllowEmptyString]
        public string? Description { get; set; }

        protected override void ProcessRecord()
        {
            var sendData = new Dictionary<string, object>();
            if (Description != null)
                sendData.Add("description", Description);
            sendData.Add("scope", Scope);

            ulong? id;
            string path;
            string target;
            if (ForMe)
            {
                id = 0;
                target = "PersonalAccessToken";
                path = $"{Resources.User.PATH}{id}/tokens/";
                if (Application != null)
                    sendData.Add("application", Application);
            }
            else
            {
                id = Application;
                target = $"Application [{id}]";
                path = $"{Resources.Application.PATH}{id}/tokens/";
            }

            var dataDescription = Json.Stringify(sendData, pretty: true);
            if (ShouldProcess(target, $"Create Token {dataDescription}"))
            {
                try
                {
                    var apiResult = CreateResource<OAuth2AccessToken>(path, sendData);
                    if (apiResult.Contents == null)
                        return;

                    WriteObject(apiResult.Contents, false);
                }
                catch (RestAPIException) { }
            }
        }
    }

    [Cmdlet(VerbsCommon.Remove, "Token", SupportsShouldProcess = true, ConfirmImpact = ConfirmImpact.High)]
    public class RemoveTokenCommand : APICmdletBase
    {
        [Parameter(Mandatory = true, ValueFromPipeline = true, Position = 0)]
        [ResourceIdTransformation(AcceptableTypes = [ResourceType.OAuth2AccessToken])]
        public ulong Id { get; set; }

        [Parameter()]
        public SwitchParameter Force { get; set; }

        protected override void ProcessRecord()
        {
            if (Force || ShouldProcess($"Token [{Id}]", "Delete completely"))
            {
                try
                {
                    var apiResult = DeleteResource($"{OAuth2AccessToken.PATH}{Id}/");
                    if (apiResult?.IsSuccessStatusCode ?? false)
                    {
                        WriteVerbose($"Token {Id} is removed.");
                    }
                }
                catch (RestAPIException) { }
            }
        }
    }

    [Cmdlet(VerbsData.Update, "Token", SupportsShouldProcess = true)]
    [OutputType(typeof(OAuth2AccessToken))]
    public class UpdateTokenCommand : APICmdletBase
    {
        [Parameter(Mandatory = true, ValueFromPipeline = true, Position = 0)]
        [ResourceIdTransformation(AcceptableTypes = [ResourceType.OAuth2AccessToken])]
        public ulong Id { get; set; }

        [Parameter()]
        [AllowEmptyString]
        public string? Description { get; set; }

        [Parameter()]
        [ValidateSet("read", "write")]
        public string? Scope { get; set; }

        protected override void ProcessRecord()
        {
            var sendData = new Dictionary<string, object>();
            if (Description != null)
                sendData.Add("description", Description);
            if (Scope != null)
                sendData.Add("scope", Scope);

            if (sendData.Count == 0)
                return;

            var dataDescription = Json.Stringify(sendData, pretty: true);
            if (ShouldProcess($"Token [{Id}]", $"Update {dataDescription}"))
            {
                try
                {
                    var after = PatchResource<OAuth2AccessToken>($"{OAuth2AccessToken.PATH}{Id}/", sendData);
                    WriteObject(after, false);
                }
                catch (RestAPIException) { }
            }
        }
    }
}
