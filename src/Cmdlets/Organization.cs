using System.Management.Automation;
using AnsibleTower.Resources;

namespace AnsibleTower.Cmdlets
{
    [Cmdlet(VerbsCommon.Get, "Organization")]
    [OutputType(typeof(Organization))]
    public class GetOrganizationCommand : GetCmdletBase<Organization>
    {
    }

    [Cmdlet(VerbsCommon.Find, "Organization", DefaultParameterSetName = "All")]
    [OutputType(typeof(Organization))]
    public class FindOrganizationCommand : FindCmdletBase<Organization>
    {
        [Parameter(Mandatory = true, ParameterSetName = "AssociatedWith", ValueFromPipelineByPropertyName = true)]
        public override ulong Id { get; set; }
        [Parameter(Mandatory = true, ParameterSetName = "AssociatedWith", ValueFromPipelineByPropertyName = true)]
        public override ResourceType Type { get; set; }

        [Parameter(Position = 0)]
        public string[]? Name { get; set; }

        [Parameter()]
        public override string[] OrderBy { get; set; } = ["id"];

        protected override void BeginProcessing()
        {
            if (Name != null)
            {
                Query.Add("name__in", string.Join(",", Name));
            }
            base.BeginProcessing();
        }
    }
}
