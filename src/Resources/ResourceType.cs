using System.Reflection;
using System.Text.Json.Serialization;

namespace AWX.Resources
{
    /// <summary>
    /// Resource Type
    /// </summary>
    [JsonConverter(typeof(Json.EnumUpperCamelCaseStringConverter<ResourceType>))]
    public enum ResourceType
    {
        None,
        [ResourcePath("applications", typeof(Application))]
        [ResourceSubPath("activity_stream", typeof(ActivityStream))]
        [ResourceSubPath("tokens", typeof(OAuth2AccessToken))]
        OAuth2Application,
        [ResourcePath("tokens", typeof(OAuth2AccessToken))]
        OAuth2AccessToken,
        [ResourcePath("instances", typeof(Instance))]
        Instance,
        [ResourcePath("instance_groups", typeof(InstanceGroup))]
        InstanceGroup,
        [ResourcePath("config", typeof(Config))]
        Config,
        [ResourcePath("ping", typeof(Ping))]
        Ping,
        [ResourcePath("settings", typeof(Setting))]
        Setting,
        [ResourcePath("organizations", typeof(Organization))]
        Organization,
        [ResourcePath("me", typeof(User))]
        Me,
        [ResourcePath("users", typeof(User))]
        User,
        [ResourcePath("projects", typeof(Project))]
        Project,
        [ResourcePath("project_updates", typeof(ProjectUpdateJob))]
        ProjectUpdate,
        [ResourcePath("teams", typeof(Team))]
        Team,
        [ResourcePath("crendentials", typeof(Credential))]
        Credential,
        [ResourcePath("crendential_types", typeof(CredentialType))]
        CredentialType,
        [ResourcePath("constructed_inventories", typeof(ConstructedInventory))]
        ConstructedInventory,
        [ResourcePath("inventories", typeof(Inventory))]
        Inventory,
        [ResourcePath("inventory_sources", typeof(InventorySource))]
        InventorySource,
        [ResourcePath("inventory_updates", typeof(InventoryUpdateJob))]
        InventoryUpdate,
        [ResourcePath("groups", typeof(Group))]
        Group,
        [ResourcePath("hosts", typeof(Host))]
        Host,

        [ResourcePath("job_templates", typeof(JobTemplate), Method = Method.GET, Description = "List Job Templates")]
        [ResourcePath("job_templates", typeof(JobTemplate), Method = Method.POST, Description = "Create a Job Template")]
        [ResourceSubPath(Type = typeof(JobTemplate), Method = Method.GET, Description = "Retrieve a Job Template")]
        [ResourceSubPath(Type = typeof(JobTemplate), Method = Method.PUT, Description = "Update a Job Template")]
        [ResourceSubPath(Type = typeof(JobTemplate), Method = Method.PATCH, Description = "Update a Job Template")]
        [ResourceSubPath(Method = Method.DELETE, Description = "Delete a Job Template")]
        [ResourceSubPath("access_list", typeof(User), Description = "List Users")]
        [ResourceSubPath("activity_stream", typeof(ActivityStream), Description = "List Activity Streams for a Job Template")]
        [ResourceSubPath("copy", typeof(Dictionary<string, bool>), Description = "Determine a Job Template is copyable")]
        [ResourceSubPath("copy", typeof(JobTemplate), Description = "Copy the Job Template")]
        JobTemplate,

        [ResourcePath("jobs", typeof(JobTemplateJob), Description = "List jobs" )]
        [ResourceSubPath(Type = typeof(JobTemplateJob.Detail), Description = "Retrieve the detailed job.")]
        [ResourceSubPath(Method = Method.DELETE, Description = "Request delete to the job.")]
        [ResourceSubPath("cancel", typeof(Dictionary<string, bool>),
            Description = "Determine if the job can be canceld.")]
        [ResourceSubPath("cancel", typeof(string),
            Method = Method.POST,
            Description = "Request cancel the pending or running job.")]
        Job,
        [ResourcePath("job_events", typeof(JobEvent))]
        JobEvent,
        [ResourcePath("project_update_event", typeof(ProjectUpdateJobEvent))]
        ProjectUpdateEvent,
        [ResourcePath("inventory_update_event", typeof(InventoryUpdateJobEvent))]
        InventoryUpdateEvent,
        [ResourcePath("job_host_summaries")]
        JobHostSummary,
        [ResourcePath("ad_hoc_commands")]
        AdHocCommand,
        [ResourcePath("ad_hoc_command_events")]
        AdHocCommandEvent,
        [ResourcePath("system_job_templates", typeof(SystemJobTemplate))]
        SystemJobTemplate,
        [ResourcePath("system_jobs", typeof(SystemJob))]
        SystemJob,
        [ResourcePath("system_job_event", typeof(SystemJobEvent))]
        SystemJobEvent,
        [ResourcePath("schedules", typeof(Schedule))]
        Schedule,
        [ResourcePath("roles", typeof(Role))]
        Role,
        [ResourcePath("notification_templates", typeof(NotificationTemplate))]
        NotificationTemplate,
        [ResourcePath("notifications", typeof(Notification))]
        Notification,
        [ResourcePath("labels", typeof(Label))]
        Label,
        [ResourcePath("unified_job_templates", typeof(UnifiedJobTemplate))]
        UnifiedJobTemplate,
        [ResourcePath("unified_jobs")]
        UnifiedJob,
        [ResourcePath("activity_stream", typeof(ActivityStream))]
        ActivityStream,
        [ResourcePath("workflow_job_templates", typeof(WorkflowJobTemplate))]
        WorkflowJobTemplate,
        [ResourcePath("workflow_jobs", typeof(WorkflowJob))]
        WorkflowJob,
        [ResourcePath("workflow_job_template_nodes", typeof(WorkflowJobTemplateNode))]
        WorkflowJobTemplateNode,
        [ResourcePath("workflow_job_nodes", typeof(WorkflowJobNode))]
        WorkflowJobNode,
        // Analytices,
        // Bulk
        [ResourcePath("execution_environments", typeof(ExecutionEnvironment))]
        ExecutionEnvironment,
        [ResourcePath("metrics")]
        Metrics,
        [ResourcePath("workflow_approval_templates")]
        WorkflowApprovalTemplate,
        [ResourcePath("workflow_approvals")]
        WorkflowApproval,

        [ResourcePath("stdout", typeof(JobLog))]
        Stdout,
    }


    [AttributeUsage(AttributeTargets.Enum | AttributeTargets.Field, AllowMultiple = true)]
    public class ResourcePathAttribute : Attribute
    {
        public ResourcePathAttribute(string pathName, Type resourceType)
        {
            PathName = pathName;
            Type = resourceType;
        }
        public ResourcePathAttribute(string pathName)
        {
            PathName = pathName;
        }
        public Method Method { get; init; } = Method.GET;
        public string PathName { get; set; }
        public Type? Type { get; init; } = null;
        public string Description { get; init; } = string.Empty;
    }

    [AttributeUsage(AttributeTargets.Enum | AttributeTargets.Field, AllowMultiple = true)]
    public class ResourceSubPathAttribute : Attribute
    {
        public ResourceSubPathAttribute() { }
        public ResourceSubPathAttribute(string pathName, Type type)
        {
            PathName = pathName;
            Type = type;
        }

        public string PathName { get; init; } = string.Empty;
        public Type Type { get; init; } = typeof(string);
        public Method Method { get; init; } = Method.GET;
        public string Description { get; init; } = string.Empty;
    }

}
