using AnsibleTower.Resources;
using System.Management.Automation;

namespace AnsibleTower.Cmdlets
{

    [Cmdlet(VerbsCommon.Get, "Me")]
    [OutputType(typeof(User))]
    public class GetMeCommand: APICmdletBase
    {
        const string Path = "/api/v2/me/";
        private Uri RequestUri { get; set; } = new(ApiConfig.Instance.Origin, Path);
        protected override void EndProcessing()
        {
            foreach (var resultSet in GetResultSet<User>(RequestUri, true))
            {
                WriteObject(resultSet.Results, true);
            }
        }
    }

    [Cmdlet(VerbsCommon.Get, "User")]
    [OutputType(typeof(User))]
    public class GetUserCommand : GetCmdletBase<User>
    {
    }

    [Cmdlet(VerbsCommon.Find, "User", DefaultParameterSetName = "All")]
    [OutputType(typeof(User))]
    public class FindUserCommand : FindCmdletBase<User>
    {
        [Parameter(Mandatory = true, ParameterSetName = "AssociatedWith", ValueFromPipelineByPropertyName = true)]
        public override ulong Id { get; set; }
        [Parameter(Mandatory = true, ParameterSetName = "AssociatedWith", ValueFromPipelineByPropertyName = true)]
        [ValidateSet(nameof(ResourceType.Organization),
                     nameof(ResourceType.ActivityStream))]
        public override ResourceType Type { get; set; }

        [Parameter(Position = 0)]
        public string[]? UserName { get; set; }

        [Parameter(Position = 1)]
        public string[]? Email {  get; set; }

        [Parameter()]
        public override string[] OrderBy { get; set; } = ["id"];

        protected override void BeginProcessing()
        {
            if (UserName != null)
            {
                Query.Add("username__in", string.Join(',', UserName));
            }
            if (Email != null)
            {
                Query.Add("email", string.Join(",", Email));
            }
            SetupCommonQuery();
        }
    }
}
