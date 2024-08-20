using AWX.Resources;
using System.Management.Automation;

namespace AWX.Cmdlets
{
    [Cmdlet(VerbsCommon.Get, "Team")]
    [OutputType(typeof(Team))]
    public class GetTeamCommand : GetCmdletBase
    {
        protected override void ProcessRecord()
        {
            if (Type != null && Type != ResourceType.Team)
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
            string path;
            if (IdSet.Count == 1)
            {
                path = $"{Team.PATH}{IdSet.First()}/";
                var res = GetResource<Team>(path);
                WriteObject(res);
            }
            else
            {
                path = Team.PATH;
                Query.Add("id__in", string.Join(',', IdSet));
                Query.Add("page_size", $"{IdSet.Count}");
                foreach (var resultSet in GetResultSet<Team>(path, Query))
                {
                    WriteObject(resultSet.Results, true);
                }
            }
        }
    }
    [Cmdlet(VerbsCommon.Find, "Team", DefaultParameterSetName = "All")]
    [OutputType(typeof(Team))]
    public class FindTeamCommand : FindCmdletBase
    {
        [Parameter(Mandatory = true, ParameterSetName = "AssociatedWith", ValueFromPipelineByPropertyName = true)]
        [ValidateSet(nameof(ResourceType.Organization),
                     nameof(ResourceType.User),
                     nameof(ResourceType.Project),
                     nameof(ResourceType.Credential),
                     nameof(ResourceType.Role))]
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
                ResourceType.Organization => $"{Organization.PATH}{Id}/teams/",
                ResourceType.User => $"{User.PATH}{Id}/teams/",
                ResourceType.Project => $"{Project.PATH}{Id}/teams/",
                ResourceType.Credential => $"{Credential.PATH}{Id}/owner_teams/",
                ResourceType.Role => $"{Role.PATH}{Id}/teams/",
                _ => Team.PATH
            };
            foreach (var resultSet in GetResultSet<Team>(path, Query, All))
            {
                WriteObject(resultSet.Results, true);
            }
        }
    }
}
