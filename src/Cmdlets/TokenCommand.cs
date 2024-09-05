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
}
