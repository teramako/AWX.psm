using AWX.Resources;
using System.Management.Automation;

namespace AWX.Cmdlets
{

    [Cmdlet(VerbsCommon.Get, "Me")]
    [OutputType(typeof(User))]
    public class GetMeCommand: APICmdletBase
    {
        protected override void EndProcessing()
        {
            foreach (var resultSet in GetResultSet<User>("/api/v2/me/", true))
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
            string path;
            if (IdSet.Count == 1)
            {
                path = $"{User.PATH}{IdSet.First()}/";
                var res = GetResource<User>(path);
                WriteObject(res);
            }
            else
            {
                path = User.PATH;
                Query.Add("id__in", string.Join(',', IdSet));
                Query.Add("page_size", $"{IdSet.Count}");
                foreach (var resultSet in GetResultSet<User>(path, Query))
                {
                    WriteObject(resultSet.Results, true);
                }
            }
        }
    }

    [Cmdlet(VerbsCommon.Find, "User", DefaultParameterSetName = "All")]
    [OutputType(typeof(User))]
    public class FindUserCommand : FindCmdletBase
    {
        [Parameter(Mandatory = true, ParameterSetName = "AssociatedWith", ValueFromPipelineByPropertyName = true)]
        [ValidateSet(nameof(ResourceType.Organization),
                     nameof(ResourceType.Team),
                     nameof(ResourceType.Credential),
                     nameof(ResourceType.Role))]
        public override ResourceType Type { get; set; }
        [Parameter(Mandatory = true, ParameterSetName = "AssociatedWith", ValueFromPipelineByPropertyName = true)]
        public override ulong Id { get; set; }

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
                Query.Add("email__in", string.Join(",", Email));
            }
            SetupCommonQuery();
        }
        protected override void ProcessRecord()
        {
            var path = Type switch
            {
                ResourceType.Organization => $"{Organization.PATH}{Id}/users/",
                ResourceType.Team => $"{Team.PATH}{Id}/users/",
                ResourceType.Credential => $"{Credential.PATH}{Id}/owner_users/",
                ResourceType.Role => $"{Role.PATH}{Id}/users/",
                _ => User.PATH
            };
            foreach (var resultSet in GetResultSet<User>(path, Query, All))
            {
                WriteObject(resultSet.Results, true);
            }
        }
    }

    [Cmdlet(VerbsCommon.Find, "AccessList")]
    [OutputType(typeof(User))]
    public class FindAccessListCommand : FindCmdletBase
    {
        [Parameter(Mandatory = true, Position = 0, ValueFromPipelineByPropertyName = true)]
        [ValidateSet(nameof(ResourceType.InstanceGroup),
                     nameof(ResourceType.Organization),
                     nameof(ResourceType.User),
                     nameof(ResourceType.Project),
                     nameof(ResourceType.Team),
                     nameof(ResourceType.Credential),
                     nameof(ResourceType.Inventory),
                     nameof(ResourceType.JobTemplate),
                     nameof(ResourceType.WorkflowJobTemplate))]
        public override ResourceType Type { get; set; }
        [Parameter(Mandatory = true, Position = 1, ValueFromPipelineByPropertyName = true)]
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
                ResourceType.InstanceGroup => $"{InstanceGroup.PATH}{Id}/access_list/",
                ResourceType.Organization => $"{Organization.PATH}{Id}/access_list/",
                ResourceType.User => $"{User.PATH}{Id}/access_list/",
                ResourceType.Project => $"{Project.PATH}{Id}/access_list/",
                ResourceType.Team => $"{Team.PATH}{Id}/access_list/",
                ResourceType.Credential => $"{Credential.PATH}{Id}/access_list/",
                ResourceType.Inventory => $"{Inventory.PATH}{Id}/access_list/",
                ResourceType.JobTemplate => $"{JobTemplate.PATH}{Id}/access_list/",
                ResourceType.WorkflowJobTemplate => $"{WorkflowJobTemplate.PATH}{Id}/access_list/",
                _ => throw new ArgumentException($"Can't handle the type: {Type}")
            };
            foreach (var resultSet in GetResultSet<User>(path, Query, All))
            {
                WriteObject(resultSet.Results, true);
            }
        }
    }
}
