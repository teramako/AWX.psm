using AnsibleTower.Resources;
using System.Management.Automation;

namespace AnsibleTower.Cmdlets
{
    [Cmdlet(VerbsCommon.Get, "ApiHelp")]
    [OutputType(typeof(ApiHelp))]
    public class HelpCommand : APICmdletBase
    {
        [Parameter(Mandatory = true, Position = 0)]
        [ArgumentCompleter(typeof(ApiPathCompleter))]
        public string Path { get; set; } = string.Empty;

        protected override void EndProcessing()
        {
            Uri uri = new(ApiConfig.Instance.Origin, Path);
            var help = GetApiHelp(uri);
            WriteObject(help, false);
        }
    }
}
