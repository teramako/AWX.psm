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
            foreach (var id in Id)
            {
                IdSet.Add(id);
            }
        }
        protected override void EndProcessing()
        {
            Query.Add("id__in", string.Join(',', IdSet));
            Query.Add("page_size", $"{IdSet.Count}");
            foreach(var application in Application.Find(Query).ToBlockingEnumerable())
            {
                WriteObject(application);
            }
        }
    }
    [Cmdlet(VerbsCommon.Find, "Application", DefaultParameterSetName = "All")]
    [OutputType(typeof(Application))]
    public class FindApplicationCommand : FindCmdletBase
    {
        [Parameter(Mandatory = true, ParameterSetName = "AssociatedWith", ValueFromPipelineByPropertyName = true)]
        public override ulong Id { get; set; }
        [Parameter(Mandatory = true, ParameterSetName = "AssociatedWith", ValueFromPipelineByPropertyName = true)]
        [ValidateSet(nameof(ResourceType.Organization), nameof(ResourceType.User))]
        public override ResourceType Type { get; set; }
        [Parameter()]
        public override string[] OrderBy { get; set; } = ["id"];

        protected override void BeginProcessing()
        {
            SetupCommonQuery();
        }
        protected override void ProcessRecord()
        {
            var asyncApps = Type switch
            {
                ResourceType.Organization => Application.FindFromOrganization(Id, Query, All),
                ResourceType.User => Application.FindFromUser(Id, Query, All),
                _ => Application.Find(Query, All)
            };
            foreach (var application in asyncApps.ToBlockingEnumerable())
            {
                WriteObject(application);
            }
        }
    }
}
