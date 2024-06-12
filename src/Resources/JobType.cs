using System.Text.Json.Serialization;

namespace AnsibleTower.Resources
{
    [JsonConverter(typeof(Json.EnumUpperCamelCaseStringConverter<JobType>))]
    public enum JobType
    {
        Run,
        Check,
        Scan
    }
}

