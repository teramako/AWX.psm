using AWX.Resources;
using System.Management.Automation;

namespace AWX.Cmdlets
{
    [Cmdlet(VerbsCommon.Get, "Group")]
    [OutputType(typeof(Group))]
    public class GetGroupCommand : GetCmdletBase
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
                path = $"{Group.PATH}{IdSet.First()}/";
                var res = GetResource<Group>(path);
                WriteObject(res);
            }
            else
            {
                path = Group.PATH;
                Query.Add("id__in", string.Join(',', IdSet));
                Query.Add("page_size", $"{IdSet.Count}");
                foreach (var resultSet in GetResultSet<Group>(path, Query))
                {
                    WriteObject(resultSet.Results, true);
                }
            }
        }
    }

    [Cmdlet(VerbsCommon.Find, "Group", DefaultParameterSetName = "All")]
    [OutputType(typeof(Group))]
    public class FindGroupCommand : FindCmdletBase
    {
        [Parameter(Mandatory = true, ParameterSetName = "AssociatedWith", ValueFromPipelineByPropertyName = true)]
        [ValidateSet(nameof(ResourceType.Inventory),
                     nameof(ResourceType.Group),
                     nameof(ResourceType.InventorySource),
                     nameof(ResourceType.Host))]
        public override ResourceType Type { get; set; }
        [Parameter(Mandatory = true, ParameterSetName = "AssociatedWith", ValueFromPipelineByPropertyName = true)]
        public override ulong Id { get; set; }

        /// <summary>
        /// List only root(Top-level) groups.
        /// Only affected for an Inventory Type
        /// </summary>
        [Parameter(ParameterSetName = "AssociatedWith")]
        public SwitchParameter OnlyRoot { get; set; }

        /// <summary>
        /// List only directly member groups.
        /// Only affected for a Host Type
        /// </summary>
        [Parameter(ParameterSetName = "AssociatedWith")]
        public SwitchParameter OnlyParnets { get; set; }

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
                ResourceType.Inventory => $"{Inventory.PATH}{Id}/" + (OnlyRoot ? "root_groups/" : "groups/"),
                ResourceType.Group => $"{Group.PATH}{Id}/children/",
                ResourceType.InventorySource => $"{InventorySource.PATH}{Id}/groups/",
                ResourceType.Host => $"{Host.PATH}{Id}/" + (OnlyParnets ? "groups/" : "all_groups/"),
                _ => Group.PATH
            };
            foreach (var resultSet in GetResultSet<Group>(path, Query, All))
            {
                WriteObject(resultSet.Results, true);
            }
        }
    }
}

