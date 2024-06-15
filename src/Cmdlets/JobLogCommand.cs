using AWX.Resources;
using System.Collections.Specialized;
using System.Management.Automation;
using System.Web;

namespace AWX.Cmdlets
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
            var path = string.Empty;
            switch (Type)
            {
                case ResourceType.Job:
                    path = JobTemplateJob.PATH;
                    break;
                case ResourceType.ProjectUpdate:
                    path = ProjectUpdateJob.PATH;
                    break;
                case ResourceType.InventoryUpdate:
                    path = InventoryUpdateJob.PATH;
                    break;
                case ResourceType.SystemJob:
                case ResourceType.AdHocCommand:
                default:
                    throw new NotImplementedException();
            }
            var log = base.GetResource<JobLog>($"{path}?{Query}");
            WriteObject(log?.Content);
        }
    }
}
