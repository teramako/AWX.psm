using AnsibleTower.Resources;
using System.Management.Automation;

namespace AnsibleTower.Cmdlets
{
    [Cmdlet(VerbsCommon.Get, "Config")]
    [OutputType([typeof(Config)])]
    public class GetConfigCommand : APICmdletBase
    {
        private const string BasePath = "/api/v2/config/";
        protected override void EndProcessing()
        {
            Uri uri = new(ApiConfig.Instance.Origin, BasePath);
            var config = GetResource<Config>(uri);
            WriteObject(config, false);
        }
    }
}
