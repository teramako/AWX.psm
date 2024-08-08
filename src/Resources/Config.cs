using System.Text.Json.Serialization;

namespace AWX.Resources
{
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

        public record ConfigLicenseInfo(string LicenseType, bool ValidKey, string SubscriptionName, string ProductName)
        {
            public override string ToString()
            {
                var sb = new StringBuilder();
                sb.Append("{ ");
                if (PrintMembers(sb)) sb.Append(' ');
                sb.Append('}');
                return sb.ToString();
            }
        }
        public class ConfigAnalyticsCollectors : Dictionary<string, ConfigValue>
        {
        }
        public record ConfigValue(string Name, string Version, string Description)
        {
            public override string ToString()
            {
                var sb = new StringBuilder();
                sb.Append("{ ");
                if (PrintMembers(sb)) sb.Append(' ');
                sb.Append('}');
                return sb.ToString();
            }
        }
    }
}

