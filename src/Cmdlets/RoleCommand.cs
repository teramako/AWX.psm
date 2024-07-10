using AWX.Resources;
using System.Management.Automation;

namespace AWX.Cmdlets
{
    [Cmdlet(VerbsCommon.Get, "Role")]
    [OutputType(typeof(Role))]
    public class GetRoleCommand : GetCmdletBase
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
                path = $"{Role.PATH}{IdSet.First()}/";
                var res = GetResource<Role>(path);
                WriteObject(res);
            }
            else
            {
                path = Role.PATH;
                Query.Add("id__in", string.Join(',', IdSet));
                Query.Add("page_size", $"{IdSet.Count}");
                foreach (var resultSet in GetResultSet<Role>(path, Query))
                {
                    WriteObject(resultSet.Results, true);
                }
            }
        }
    }

    [Cmdlet(VerbsCommon.Find, "Role", DefaultParameterSetName = "All")]
    [OutputType(typeof(Role))]
    public class FindRoleCommand : FindCmdletBase
    {
        [Parameter(Mandatory = true, ParameterSetName = "AssociatedWith", ValueFromPipelineByPropertyName = true)]
        [ValidateSet(nameof(ResourceType.User),
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
                ResourceType.User => $"{User.PATH}{Id}/roles/",
                ResourceType.Team => $"{Team.PATH}{Id}/roles/",
               _ => Role.PATH
            };
            foreach (var resultSet in GetResultSet<Role>(path, Query, All))
            {
                WriteObject(resultSet.Results, true);
            }
        }
    }

    [Cmdlet(VerbsCommon.Find, "ObjectRole", DefaultParameterSetName = "All")]
    [OutputType(typeof(Role))]
    public class FindObjectRoleCommand : FindCmdletBase
    {
        [Parameter(Mandatory = true, ValueFromPipelineByPropertyName = true)]
        [ValidateSet(nameof(ResourceType.InstanceGroup),
                     nameof(ResourceType.Organization),
                     nameof(ResourceType.Project),
                     nameof(ResourceType.Team),
                     nameof(ResourceType.Credential),
                     nameof(ResourceType.Inventory),
                     nameof(ResourceType.JobTemplate),
                     nameof(ResourceType.WorkflowJobTemplate))]
        public override ResourceType Type { get; set; }
        [Parameter(Mandatory = true, ValueFromPipelineByPropertyName = true)]
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
                ResourceType.InstanceGroup => $"{InstanceGroup.PATH}{Id}/object_roles/",
                ResourceType.Organization => $"{Organization.PATH}{Id}/object_roles/",
                ResourceType.Project => $"{Project.PATH}{Id}/object_roles/",
                ResourceType.Team => $"{Team.PATH}{Id}/object_roles/",
                ResourceType.Credential => $"{Credential.PATH}{Id}/object_roles/",
                ResourceType.Inventory => $"{Inventory.PATH}{Id}/object_roles/",
                ResourceType.JobTemplate => $"{JobTemplate.PATH}{Id}/object_roles/",
                ResourceType.WorkflowJobTemplate => $"{WorkflowJobTemplate.PATH}{Id}/object_roles/",
                _ => throw new ArgumentException()
            };
            foreach (var resultSet in GetResultSet<Role>(path, Query, All))
            {
                WriteObject(resultSet.Results, true);
            }
        }
    }
}
