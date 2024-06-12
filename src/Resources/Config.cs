using System.Text.Json.Serialization;

namespace AnsibleTower.Resources
{
    [ResourceType(ResourceType.Config)]
    public class Config(string timeZone,
                        Config.ConfigLicenseInfo licenseInfo,
                        string version,
                        string eula,
                        string configAnalyticsStatus,
                        Config.ConfigAnalyticsCollectors analyticsCollectors,
                        string[][] becomeMethods,
                        bool uiNext,
                        string projectBaseDir,
                        string[] projectLocalPaths,
                        string[] customVirtualenvs)
    {
        public string TimeZone { get; } = timeZone;
        public ConfigLicenseInfo LicenseInfo { get; } = licenseInfo;
        public string Version { get; } = version;
        public string Eula { get; } = eula;
        public string ConfigAnalyticsStatus { get; } = configAnalyticsStatus;
        public ConfigAnalyticsCollectors AnalyticsCollectors { get; } = analyticsCollectors;
        public string[][] BecomeMethods { get; } = becomeMethods;
        public bool UiNext { get; } = uiNext;
        public string ProjectBaseDir { get; } = projectBaseDir;
        public string[] ProjectLocalPaths { get; } = projectLocalPaths;
        public string[] CustomVirtualenvs { get; } = customVirtualenvs;

        public class ConfigLicenseInfo(string licenseType, bool validKey, string subscriptionName, string productName)
        {
            public string LicenseType { get; } = licenseType;
            public bool ValidKey { get; } = validKey;
            public string SubscriptionName { get; } = subscriptionName;
            public string ProductName { get; } = productName;
        }
        public class ConfigAnalyticsCollectors : Dictionary<string, ConfigValue>
        {
        }
        public class ConfigValue(string name, string version, string description)
        {
            public string Name { get; } = name;
            public string Version { get; } = version;
            public string Description { get; } = description;

        }
    }
}

