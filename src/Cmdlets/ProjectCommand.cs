using AWX.Resources;
using System.Management.Automation;

namespace AWX.Cmdlets
{
    [Cmdlet(VerbsCommon.Get, "Project")]
    [OutputType(typeof(Project))]
    public class GetProjectCommand : GetCmdletBase
    {
        protected override void ProcessRecord()
        {
            if (Type != null && Type != ResourceType.Project)
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
                var res = GetResource<Project>($"{Project.PATH}{IdSet.First()}/");
                WriteObject(res);
            }
            else
            {
                Query.Add("id__in", string.Join(',', IdSet));
                Query.Add("page_size", $"{IdSet.Count}");
                foreach (var resultSet in GetResultSet<Project>(Project.PATH, Query, true))
                {
                    WriteObject(resultSet.Results, true);
                }
            }
        }
    }

    [Cmdlet(VerbsCommon.Find, "Project", DefaultParameterSetName = "All")]
    [OutputType(typeof(Project))]
    public class FindProjectCommand : FindCmdletBase
    {
        [Parameter(Mandatory = true, ParameterSetName = "AssociatedWith", ValueFromPipelineByPropertyName = true)]
        [ValidateSet(nameof(ResourceType.Organization),
                     nameof(ResourceType.User),
                     nameof(ResourceType.Team))]
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
                ResourceType.Organization => $"{Organization.PATH}{Id}/projects/",
                ResourceType.User => $"{User.PATH}{Id}/projects/",
                ResourceType.Team => $"{Team.PATH}{Id}/projects/",
                _ => Project.PATH
            };
            foreach (var resultSet in GetResultSet<Project>(path, Query, All))
            {
                WriteObject(resultSet.Results, true);
            }
        }
    }

    [Cmdlet(VerbsCommon.Get, "Playbook")]
    [OutputType(typeof(string))]
    public class GetPlaybookCommand : GetCmdletBase
    {
        protected override void ProcessRecord()
        {
            if (Type != null && Type != ResourceType.Project)
            {
                return;
            }
            foreach (var id in Id)
            {
                if (IdSet.Add(id))
                {
                    var playbooks = GetResource<string[]>($"{Project.PATH}{id}/playbooks/");
                    WriteObject(playbooks, true);
                }
            }
        }
    }

    [Cmdlet(VerbsCommon.Get, "InventoryFile")]
    [OutputType(typeof(string))]
    public class GetInventoryFileCommand : GetCmdletBase
    {
        protected override void ProcessRecord()
        {
            if (Type != null && Type != ResourceType.Project)
            {
                return;
            }
            foreach (var id in Id)
            {
                if (IdSet.Add(id))
                {
                    var inventoryFiles = GetResource<string[]>($"{Project.PATH}{id}/inventories/");
                    WriteObject(inventoryFiles, true);
                }
            }
        }
    }
}
