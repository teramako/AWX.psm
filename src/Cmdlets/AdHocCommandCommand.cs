using AWX.Resources;
using System.Management.Automation;

namespace AWX.Cmdlets
{

    [Cmdlet(VerbsCommon.Get, "AdHocCommandJob")]
    [OutputType(typeof(AdHocCommand.Detail))]
    public class GetAdHocCommandJobCommand : GetCmdletBase
    {
        protected override void ProcessRecord()
        {
            foreach (var id in Id)
            {
                if (!IdSet.Add(id))
                {
                    // skip already processed
                    continue;
                }
                var res = GetResource<AdHocCommand.Detail>($"{AdHocCommand.PATH}{id}/");
                if (res != null)
                {
                    WriteObject(res);
                }
            }
        }
    }

    [Cmdlet(VerbsCommon.Find, "AdHocCommandJob", DefaultParameterSetName = "All")]
    [OutputType(typeof(AdHocCommand))]
    public class FindAdHocCommandJobCommand : FindCmdletBase
    {
        [Parameter(Mandatory = true, ParameterSetName = "AssociatedWith", ValueFromPipelineByPropertyName = true)]
        [ValidateSet(nameof(ResourceType.Inventory),
                     nameof(ResourceType.Host),
                     nameof(ResourceType.Group))]
        public override ResourceType Type { get; set; }
        [Parameter(Mandatory = true, ParameterSetName = "AssociatedWith", ValueFromPipelineByPropertyName = true)]
        public override ulong Id { get; set; }

        [Parameter()]
        public override string[] OrderBy { get; set; } = ["!id"];


        protected override void BeginProcessing()
        {
            SetupCommonQuery();
        }
        protected override void ProcessRecord()
        {
            var path = Type switch
            {
                ResourceType.Inventory => $"{Inventory.PATH}{Id}/ad_hoc_commands/",
                ResourceType.Host => $"{Host.PATH}{Id}/ad_hoc_commands/",
                ResourceType.Group => $"{Group.PATH}{Id}/ad_hoc_commands/",
                _ => AdHocCommand.PATH
            };
            foreach (var resultSet in GetResultSet<AdHocCommand>(path, Query, All))
            {
                WriteObject(resultSet.Results, true);
            }
        }
    }
}
