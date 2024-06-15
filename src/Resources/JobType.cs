using System.Text.Json.Serialization;

namespace AWX.Resources
{
    [JsonConverter(typeof(Json.EnumUpperCamelCaseStringConverter<JobType>))]
    public enum JobType
    {
        Run,
        Check,
        Scan
    }
}

