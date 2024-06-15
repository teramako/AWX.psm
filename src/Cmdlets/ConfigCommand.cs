using AWX.Resources;
using System.Management.Automation;

namespace AWX.Cmdlets
{
    [Cmdlet(VerbsCommon.Get, "Config")]
    [OutputType([typeof(Config)])]
    public class GetConfigCommand : APICmdletBase
    {
        private const string BasePath = "/api/v2/config/";
        protected override void EndProcessing()
        {
            var config = GetResource<Config>(BasePath);
            WriteObject(config, false);
        }
    }
}
