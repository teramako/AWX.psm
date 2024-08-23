using AWX.Resources;
using System.Management.Automation;

namespace AWX.Cmdlets
{
    [Cmdlet(VerbsCommon.Get, "Dashboard")]
    [OutputType(typeof(Dashboard))]
    public class GetDashboardCommand : APICmdletBase
    {
        protected override void ProcessRecord()
        {
            var apiResult = GetResource<Dashboard>(Dashboard.PATH);
            WriteObject(apiResult, false);
        }
    }
}
