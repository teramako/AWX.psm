using System.Text.Json.Serialization;

namespace AnsibleTower.Resources
{
    [JsonConverter(typeof(Json.EnumUpperCamelCaseStringConverter<JobLaunchType>))]
    public enum JobLaunchType
    {
        Manual,
        Relaunch,
        Callback,
        Scheduled,
        Dependency,
        Workflow,
        Webhook,
        Sync,
        Scm
    }
}

