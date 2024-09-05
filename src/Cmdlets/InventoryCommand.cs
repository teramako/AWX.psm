using AWX.Resources;
using System.Management.Automation;

namespace AWX.Cmdlets
{
    [Cmdlet(VerbsCommon.Get, "Inventory")]
    [OutputType(typeof(Inventory))]
    public class GetInventoryCommand : GetCmdletBase
    {
        protected override void ProcessRecord()
        {
            if (Type != null && Type != ResourceType.Inventory)
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
                var res = GetResource<Inventory>($"{Inventory.PATH}{IdSet.First()}/");
                WriteObject(res);
            }
            else
            {
                Query.Add("id__in", string.Join(',', IdSet));
                Query.Add("page_size", $"{IdSet.Count}");
                foreach (var resultSet in GetResultSet<Inventory>(Inventory.PATH, Query, true))
                {
                    WriteObject(resultSet.Results, true);
                }
            }
        }
    }

    [Cmdlet(VerbsCommon.Find, "Inventory", DefaultParameterSetName = "All")]
    [OutputType(typeof(Inventory))]
    public class FindInventoryCommand : FindCmdletBase
    {
        [Parameter(Mandatory = true, ParameterSetName = "AssociatedWith", ValueFromPipelineByPropertyName = true)]
        [ValidateSet(nameof(ResourceType.Organization),
                     nameof(ResourceType.Inventory),
                     nameof(ResourceType.Host))]
        public override ResourceType Type { get; set; }
        [Parameter(Mandatory = true, ParameterSetName = "AssociatedWith", ValueFromPipelineByPropertyName = true)]
        public override ulong Id { get; set; }

        [Parameter()]
        public InventoryKind Kind { get; set; } = InventoryKind.All;

        [Parameter()]
        public override string[] OrderBy { get; set; } = ["id"];

        public enum InventoryKind
        {
            All, Normal, Smart, Constructed
        }

        protected override void BeginProcessing()
        {
            switch (Kind)
            {
                case InventoryKind.Normal:
                    Query.Add("kind", "");
                    break;
                case InventoryKind.Smart:
                case InventoryKind.Constructed:
                    Query.Add("kind", Kind.ToString().ToLowerInvariant());
                    break;
                case InventoryKind.All:
                default:
                    break;
            }
            SetupCommonQuery();
        }
        protected override void ProcessRecord()
        {
            var path = Type switch
            {
                ResourceType.Organization => $"{Organization.PATH}{Id}/inventories/",
                ResourceType.Inventory => $"{Inventory.PATH}{Id}/input_inventories/",
                ResourceType.Host => $"{Host.PATH}{Id}/smart_inventories/",
                _ => Inventory.PATH
            };
            foreach (var resultSet in GetResultSet<Inventory>(path, Query, All))
            {
                WriteObject(resultSet.Results, true);
            }
        }
    }
}
