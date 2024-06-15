using System.Text.Json.Serialization;

namespace AWX.Resources
{
    [JsonConverter(typeof(Json.EnumUpperCamelCaseStringConverter<JobStatus>))]
    public enum JobStatus
    {
        New,
        Pending,
        Waiting,
        Running,
        Successful,
        Failed,
        Error,
        Canceled,
    }
}

