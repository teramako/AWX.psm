using System.Text.Json.Serialization;

namespace AWX.Resources
{
    public interface ISummaryFields
    {
    }

    [JsonConverter(typeof(Json.CapabilityConverter))]
    [Flags]
    public enum Capability
    {
        None  = 0,
        Edit =  1 << 0,
        Delete = 1 << 1,
        Start = 1 << 2,
        Schedule = 1 << 3,
        Copy = 1 << 4,
        Use = 1 << 5,
        AdHoc = 1 << 6,
    }

    public record NameSummary(ulong Id, string Name);
    public record NameDescriptionSummary(ulong Id, string Name, string Description);
    public record UserSummary(ulong Id,
                              string Username,
                              [property: JsonPropertyName("first_name")] string FirstName,
                              [property: JsonPropertyName("last_name")] string LastName);

    public record OAuth2AccessTokenSummary(ulong Id,
                                           [property: JsonPropertyName("user_id")] ulong UserId,
                                           string Description,
                                           string Scope);

    public record UserCapability(bool Edit = false,
                                 bool Delete = false,
                                 bool Copy = false,
                                 bool Use = false);

    public record RoleSummary(ulong Id,
                              string Name,
                              string Description,
                              [property: JsonPropertyName("user_only")] bool? UserOnly = null)
        : NameDescriptionSummary(Id, Name, Description);

    public record EnvironmentSummary(ulong Id,
                                     string Name,
                                     string Description,
                                     string Image)
        : NameDescriptionSummary(Id, Name, Description);

    public record RelatedFieldCountsSummary(
        int Inventories,
        int Teams,
        int Users,
        [property: JsonPropertyName("job_templates")] int JobTemplates,
        int Admins,
        int Projects);

    public record TokenSummary(ulong Id, string Token, string Scope);

    public record ListSummary<T>(int Count, T[] Results);

    public record CredentialSummary(ulong Id,
                                    string Name,
                                    string Description,
                                    string Kind,
                                    bool Cloud = false,
                                    bool Kubernetes = false,
                                    [property: JsonPropertyName("credential_type_id")] ulong? CredentialTypeId = null);
    public record JobTemplateCredentialSummary(ulong Id,
                                               string Name,
                                               string Description,
                                               string Kind,
                                               bool Cloud);

    public record LastJobSummary(ulong Id,
                                 string Name,
                                 string Description,
                                 DateTime? Finished,
                                 JobStatus Status,
                                 bool Failed);
    public record HostRecentJobSummary(ulong Id,
                                       string Name,
                                       ResourceType Type,
                                       JobStatus Status,
                                       DateTime? Finished);
    public record RecentJobSummary(ulong Id,
                                   JobStatus Status,
                                   DateTime? Finished,
                                   [property: JsonPropertyName("canceled_on")] DateTime? CanceledOn,
                                   ResourceType Type);
    public record JobSummary(ulong Id,
                             string Name,
                             string Description,
                             JobStatus Status,
                             bool Failed,
                             double Elapsed,
                             ResourceType Type);
    public record JobExSummary(ulong Id,
                               string Name,
                               string Description,
                               JobStatus Status,
                               bool Failed,
                               double Elapsed,
                               ResourceType Type,
                               [property: JsonPropertyName("job_template_id")] ulong JobTemplateId,
                               [property: JsonPropertyName("job_template_name")] string JobTemplateName);

    public record WorkflowJobSummary(ulong Id, string Name, string Description, JobStatus Status, bool Failed, double Elapsed);

    public record AncestorJobSummary(ulong Id, string Name, ResourceType Type, string Url);

    public record LastJobHostSummary(ulong Id, bool Failed);

    public record LastUpdateSummary(ulong Id,
                                    string Name,
                                    string Description,
                                    JobStatus Status,
                                    bool Failed)
        : NameDescriptionSummary(Id, Name, Description);
    public record InventorySummary(ulong Id,
                                   string Name,
                                   string Description,
                                   [property: JsonPropertyName("has_active_failures")] bool HasActiveFailures,
                                   [property: JsonPropertyName("total_hosts")] int TotalHosts,
                                   [property: JsonPropertyName("hosts_with_active_failures")] int HostsWithActiveFailures,
                                   [property: JsonPropertyName("total_groups")] int TotalGroups,
                                   [property: JsonPropertyName("has_inventory_sources")] bool HasInventorySources,
                                   [property: JsonPropertyName("total_inventory_sources")] int TotalInventorySources,
                                   [property: JsonPropertyName("inventory_sources_with_failures")] int InventorySourcesWithFailures,
                                   [property: JsonPropertyName("organization_id")] ulong OrganizationId,
                                   string Kind)
        : NameDescriptionSummary(Id, Name, Description);

    public record InventorySourceSummary(ulong Id,
                                         string Name,
                                         string Source,
                                         [property: JsonPropertyName("last_updated")] DateTime LastUpdated,
                                         JobStatus Status);
    public record ProjectSummary(ulong Id,
                                 string Name,
                                 string Description,
                                 JobTemplateStatus Status,
                                 [property: JsonPropertyName("scm_type")] string ScmType,
                                 [property: JsonPropertyName("allow_override")] bool AllowOverride)
        : NameDescriptionSummary(Id, Name, Description);
    public record ProjectUpdateSummary(ulong Id, string Name, string Description, JobStatus Status, bool Failed)
        : NameDescriptionSummary(Id, Name, Description);

    public record UnifiedJobTemplateSummary(ulong Id,
                                     string Name,
                                     string Description,
                                     [property: JsonPropertyName("unified_job_type")] ResourceType UnifiedJobType)
        : NameDescriptionSummary(Id, Name, Description);
    public record InstanceGroupSummary(ulong Id,
                                       string Name,
                                       [property: JsonPropertyName("is_container_group")] bool IsContainerGroup);

    public record OwnerSummary(ulong Id, string Type, string Name, string Description, string Url);

    public record ScheduleSummary(ulong Id,
                                  string Name,
                                  string Description,
                                  [property: JsonPropertyName("next_run")] DateTime NextRun);
    public record RecentNotificationSummary(ulong Id, JobStatus Status, DateTime Created, string Error);

    public record WorkflowApprovalTemplateSummary(ulong Id, string Name, string Description, int Timeout);
}
