using AWX.Resources;
using System.Management.Automation;

namespace AWX.Cmdlets
{
    [Cmdlet(VerbsCommon.Get, "Application")]
    [OutputType(typeof(Application))]
    public class GetApplicationCommand : GetCmdletBase
    {
        protected override void ProcessRecord()
        {
            if (Type != null && Type != ResourceType.OAuth2Application)
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
            string path;
            if (IdSet.Count == 1)
            {
                path = $"{Application.PATH}{IdSet.First()}/";
                var res = GetResource<Application>(path);
                WriteObject(res);
            }
            else
            {
                path = Application.PATH;
                Query.Add("id__in", string.Join(',', IdSet));
                Query.Add("page_size", $"{IdSet.Count}");
                foreach (var resultSet in GetResultSet<Application>(path, Query))
                {
                    WriteObject(resultSet.Results, true);
                }
            }
        }
    }
    [Cmdlet(VerbsCommon.Find, "Application", DefaultParameterSetName = "All")]
    [OutputType(typeof(Application))]
    public class FindApplicationCommand : FindCmdletBase
    {
        [Parameter(Mandatory = true, ParameterSetName = "AssociatedWith", ValueFromPipelineByPropertyName = true)]
        [ValidateSet(nameof(ResourceType.Organization), nameof(ResourceType.User))]
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
                ResourceType.Organization => $"{Organization.PATH}{Id}/applications/",
                ResourceType.User => $"{User.PATH}{Id}/applications/",
                _ => Application.PATH
            };
            foreach (var resultSet in GetResultSet<Application>(path, Query, All))
            {
                WriteObject(resultSet.Results, true);
            }
        }
    }
}
