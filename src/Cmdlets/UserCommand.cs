using AWX.Resources;
using System.Management.Automation;

namespace AWX.Cmdlets
{

    [Cmdlet(VerbsCommon.Get, "Me")]
    [OutputType(typeof(User))]
    public class GetMeCommand: APICmdletBase
    {
        const string Path = "/api/v2/me/";
        private Uri RequestUri { get; set; } = new(ApiConfig.Instance.Origin, Path);
        protected override void EndProcessing()
        {
            foreach (var resultSet in GetResultSet<User>(Path, true))
            {
                WriteObject(resultSet.Results, true);
            }
        }
    }

    [Cmdlet(VerbsCommon.Get, "User")]
    [OutputType(typeof(User))]
    public class GetUserCommand : GetCmdletBase
    {
        protected override void ProcessRecord()
        {
            foreach (var id in Id)
            {
                IdSet.Add(id);
            }
        }
        protected override void EndProcessing()
        {
            Query.Add("id__in", string.Join(',', IdSet));
            Query.Add("page_size", $"{IdSet.Count}");
            foreach (var resultSet in GetResultSet<User>($"{User.PATH}?{Query}", true))
            {
                WriteObject(resultSet.Results, true);
            }
        }
    }

    [Cmdlet(VerbsCommon.Find, "User", DefaultParameterSetName = "All")]
    [OutputType(typeof(User))]
    public class FindUserCommand : FindCmdletBase
    {
        [Parameter(Mandatory = true, ParameterSetName = "AssociatedWith", ValueFromPipelineByPropertyName = true)]
        public override ulong Id { get; set; }
        [Parameter(Mandatory = true, ParameterSetName = "AssociatedWith", ValueFromPipelineByPropertyName = true)]
        [ValidateSet(nameof(ResourceType.Organization),
                     nameof(ResourceType.Team),
                     nameof(ResourceType.Credential),
                     nameof(ResourceType.Role))]
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
        protected override void ProcessRecord()
        {
            var path = User.PATH;
            if (Id > 0)
            {
                switch (Type)
                {
                    case ResourceType.Organization:
                        path = $"{Organization.PATH}{Id}/users/";
                        break;
                    case ResourceType.Team:
                        path = $"{Team.PATH}{Id}/users/";
                        break;
                    case ResourceType.Credential:
                        path = $"{Credential.PATH}{Id}/owner_users/";
                        break;
                    case ResourceType.Role:
                        path = $"{Role.PATH}{Id}/users/";
                        break;
                    default:
                        WriteError(new ErrorRecord(new ArgumentException(nameof(Type)),
                                                   "",
                                                   ErrorCategory.InvalidArgument, Type));
                        return;
                }
            }
            Find<User>(path);
        }
    }
}
