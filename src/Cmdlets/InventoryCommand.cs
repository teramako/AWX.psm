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

    [Cmdlet(VerbsCommon.New, "Inventory", SupportsShouldProcess = true, DefaultParameterSetName = "NormalInventory")]
    [OutputType(typeof(Inventory))]
    public class NewInventoryCommand : APICmdletBase
    {
        [Parameter(Mandatory = true, Position = 0)]
        [ResourceIdTransformation(AcceptableTypes = [ResourceType.Organization])]
        public ulong Organization { get; set; }

        [Parameter(Mandatory = true, Position = 1)]
        public string Name { get; set; } = string.Empty;

        [Parameter()]
        public string? Description { get; set; }

        [Parameter()]
        [ExtraVarsArgumentTransformation]
        public string? Variables { get; set; }

        [Parameter()]
        public SwitchParameter PreventInstanceGroupFallback { get; set; }

        [Parameter(Mandatory = true, ParameterSetName = "SmartInventory")]
        public SwitchParameter AsSmartInventory { get; set; }

        [Parameter(Mandatory = true, ParameterSetName = "SmartInventory")]
        public string HostFilter { get; set; } = string.Empty;

        protected override void ProcessRecord()
        {
            var sendData = new Dictionary<string, object>()
            {
                { "name", Name },
                { "organization", Organization },
            };
            if (!string.IsNullOrEmpty(Description))
                sendData.Add("description", Description);
            if (!string.IsNullOrEmpty(Variables))
                sendData.Add("variables", Variables);
            if (PreventInstanceGroupFallback)
                sendData.Add("prevent_instance_group_fallback", true);

            if (AsSmartInventory)
            {
                sendData.Add("kind", "smart");
                sendData.Add("host_filter", HostFilter);
            }

            var dataDescription = string.Join(", ", sendData.Select(kv => $"{kv.Key} = {kv.Value}"));
            if (ShouldProcess($"{{ {dataDescription} }}"))
            {
                try
                {
                    var apiResult = CreateResource<Inventory>(Inventory.PATH, sendData);
                    if (apiResult.Contents == null)
                        return;

                    WriteObject(apiResult.Contents, false);
                }
                catch (RestAPIException) { }
            }
        }
    }
}
