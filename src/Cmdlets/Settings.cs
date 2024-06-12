using AnsibleTower.Resources;
using System.Collections.Specialized;
using System.Management.Automation;

namespace AnsibleTower.Cmdlets
{
    [Cmdlet(VerbsCommon.Get, "Setting")]
    [OutputType(typeof(Setting))]
    public class GetSettingCommand : APICmdletBase
    {
        [Parameter(Position = 0)]
        public string Name { get; set; } = string.Empty;

        private const string BasePath = "/api/v2/settings/";
        protected override void EndProcessing()
        {
            if (string.IsNullOrEmpty(Name))
            {
                WriteObject(GetSettingList(), true);
            }
            else
            {
                WriteObject(GetSetting(Name), false);
            }
        }
        private OrderedDictionary? GetSetting(string name)
        {
            Uri uri = new Uri(ApiConfig.Instance.Origin, $"{BasePath}{name}/");
            return GetResource<OrderedDictionary>(uri);
        }
        private Setting[]? GetSettingList()
        {
            Uri uri = new Uri(ApiConfig.Instance.Origin, BasePath);
            var resultSet = GetResultSet<Setting>(uri, false).First();
            return resultSet.Results;
        }
    }
}
