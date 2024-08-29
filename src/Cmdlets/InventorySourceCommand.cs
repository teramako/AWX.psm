using AWX.Resources;
using System.Management.Automation;

namespace AWX.Cmdlets
{
    [Cmdlet(VerbsCommon.Get, "InventorySource")]
    [OutputType(typeof(InventorySource))]
    public class GetInventorySourceCommand : GetCmdletBase
    {
        protected override void ProcessRecord()
        {
            if (Type != null && Type != ResourceType.InventorySource)
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
                var res = GetResource<InventorySource>($"{InventorySource.PATH}{IdSet.First()}/");
                WriteObject(res);
            }
            else
            {
                Query.Add("id__in", string.Join(',', IdSet));
                Query.Add("page_size", $"{IdSet.Count}");
                foreach (var resultSet in GetResultSet<InventorySource>(InventorySource.PATH, Query, true))
                {
                    WriteObject(resultSet.Results, true);
                }
            }
        }
    }

    [Cmdlet(VerbsCommon.Find, "InventorySource", DefaultParameterSetName = "All")]
    [OutputType(typeof(InventorySource))]
    public class FindInventorySourceCommand : FindCmdletBase
    {
        [Parameter(Mandatory = true, ParameterSetName = "AssociatedWith", ValueFromPipelineByPropertyName = true)]
        [ValidateSet(nameof(ResourceType.Project),
                     nameof(ResourceType.Inventory),
                     nameof(ResourceType.Group),
                     nameof(ResourceType.Host))]
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
                ResourceType.Project => $"{Project.PATH}{Id}/scm_inventory_sources/",
                ResourceType.Inventory => $"{Inventory.PATH}{Id}/inventory_sources/",
                ResourceType.Group => $"{Group.PATH}{Id}/inventory_sources/",
                ResourceType.Host => $"{Host.PATH}{Id}/inventory_sources/",
                _ => InventorySource.PATH
            };
            foreach (var resultSet in GetResultSet<InventorySource>(path, Query, All))
            {
                WriteObject(resultSet.Results, true);
            }
        }
    }
}
