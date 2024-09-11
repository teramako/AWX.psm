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
            if (IdSet.Count == 1)
            {
                var res = GetResource<Team>($"{Team.PATH}{IdSet.First()}/");
                WriteObject(res);
            }
            else
            {
                Query.Add("id__in", string.Join(',', IdSet));
                Query.Add("page_size", $"{IdSet.Count}");
                foreach (var resultSet in GetResultSet<Team>(Team.PATH, Query, true))
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

    [Cmdlet(VerbsCommon.New, "Team", SupportsShouldProcess = true)]
    [OutputType(typeof(Team))]
    public class AddTeamCommand : APICmdletBase
    {
        [Parameter(Mandatory = true)]
        [ResourceIdTransformation(AcceptableTypes = [ResourceType.Organization])]
        public ulong Organization { get; set; }

        [Parameter(Mandatory = true)]
        public string Name { get; set; } = string.Empty;

        [Parameter()]
        public string Description { get; set; } = string.Empty;

        protected override void ProcessRecord()
        {
            var sendData = new Dictionary<string, object>()
            {
                { "name", Name },
                { "description", Description },
                { "organization", Organization },
            };
            var dataDescription = string.Join(", ", sendData.Select(kv => $"{kv.Key} = {kv.Value}"));
            if (ShouldProcess($"{{ {dataDescription} }}"))
            {
                try
                {
                    var apiResult = CreateResource<Team>(Team.PATH, sendData);
                    if (apiResult.Contents == null)
                        return;

                    WriteObject(apiResult.Contents, false);
                }
                catch (RestAPIException) { }
            }
        }
    }

    [Cmdlet(VerbsCommon.Remove, "Team", SupportsShouldProcess = true)]
    public class RemoveTeamCommand : APICmdletBase
    {
        [Parameter(Mandatory = true, ValueFromPipeline = true, Position = 0)]
        [ResourceIdTransformation(AcceptableTypes = [ResourceType.Team])]
        public ulong Id { get; set; }

        protected override void ProcessRecord()
        {
            if (ShouldProcess($"Team [{Id}]"))
            {
                try
                {
                    var apiResult = DeleteResource($"{Team.PATH}{Id}/");
                    if (apiResult?.IsSuccessStatusCode ?? false)
                    {
                        WriteVerbose($"Team {Id} is removed.");
                    }
                }
                catch (RestAPIException) { }
            }
        }
    }
}
