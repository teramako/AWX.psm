using AnsibleTower.Resources;
using System.Collections.Specialized;
using System.Management.Automation;
using System.Web;

namespace AnsibleTower.Cmdlets
{
    [Cmdlet(VerbsCommon.Get, "JobLog")]
    [OutputType(typeof(JobLog))]
    public class GetJobLogCommand : APICmdletBase
    {
        [Parameter(Mandatory = true, ValueFromPipelineByPropertyName = true)]
        public ulong Id { get; set; }
        [Parameter(ValueFromPipelineByPropertyName = true)]
        [ValidateSet(nameof(ResourceType.Job), "ProjectUpdate", "InventoryUpdate", "AdHocCommand")]
        public ResourceType Type { get; set; } = ResourceType.Job;

        private NameValueCollection Query = HttpUtility.ParseQueryString(string.Empty);

        protected override void BeginProcessing()
        {
            Query.Add("format", "json");
        }
        protected override void ProcessRecord()
        {
            Uri uri = CreateURI(APIv2RootPath, ResourceType.Stdout, Type, Id, Query);
            var log = base.GetResource<Resources.JobLog>(uri);
            WriteObject(log?.Content);
        }
    }
}
