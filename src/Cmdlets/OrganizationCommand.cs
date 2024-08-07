using System.Management.Automation;
using AWX.Resources;

namespace AWX.Cmdlets
{
    [Cmdlet(VerbsCommon.Get, "Organization")]
    [OutputType(typeof(Organization))]
    public class GetOrganizationCommand : GetCmdletBase
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
                path = $"{Organization.PATH}{IdSet.First()}/";
                var res = GetResource<Organization>(path);
                WriteObject(res);
            }
            else
            {
                path = Organization.PATH;
                Query.Add("id__in", string.Join(',', IdSet));
                Query.Add("page_size", $"{IdSet.Count}");
                foreach (var resultSet in GetResultSet<Organization>(path, Query))
                {
                    WriteObject(resultSet.Results, true);
                }
            }
        }
    }

    [Cmdlet(VerbsCommon.Find, "Organization", DefaultParameterSetName = "All")]
    [OutputType(typeof(Organization))]
    public class FindOrganizationCommand : FindCmdletBase
    {
        [Parameter(Mandatory = true, ParameterSetName = "AssociatedWith", ValueFromPipelineByPropertyName = true)]
        [ValidateSet(nameof(ResourceType.User))]
        public override ResourceType Type { get; set; }
        [Parameter(Mandatory = true, ParameterSetName = "AssociatedWith", ValueFromPipelineByPropertyName = true)]
        public override ulong Id { get; set; }
        [Parameter(ParameterSetName = "AssociatedWith")]
        public SwitchParameter Admin { get; set; }

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
            SetupCommonQuery();
        }
        protected override void ProcessRecord()
        {
            var path = Type switch {
                ResourceType.User => $"{User.PATH}{Id}/" + (Admin ? "admin_of_organizations/" : "organizations/"),
                _ => Organization.PATH
            };
            foreach (var resultSet in GetResultSet<Organization>(path, Query, All))
            {
                WriteObject(resultSet.Results, true);
            }
        }
    }
}
