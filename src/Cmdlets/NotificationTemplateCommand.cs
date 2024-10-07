using AWX.Resources;
using System.Collections;
using System.Management.Automation;

namespace AWX.Cmdlets
{
    [Cmdlet(VerbsCommon.Get, "NotificationTemplate")]
    [OutputType(typeof(NotificationTemplate))]
    public class GetNotificationTemplateCommand : GetCommandBase
    {
        protected override void ProcessRecord()
        {
            if (Type != null && Type != ResourceType.NotificationTemplate)
            {
                return;
            }
            foreach (var id in Id)
            {
                IdSet.Add(id);
            }
        }
        protected override void EndProcessing()
        {
            if (IdSet.Count == 1)
            {
                var res = GetResource<NotificationTemplate>($"{NotificationTemplate.PATH}{IdSet.First()}/");
                WriteObject(res);
            }
            else
            {
                Query.Add("id__in", string.Join(',', IdSet));
                Query.Add("page_size", $"{IdSet.Count}");
                foreach (var resultSet in GetResultSet<NotificationTemplate>(NotificationTemplate.PATH, Query, true))
                {
                    WriteObject(resultSet.Results, true);
                }
            }
        }
    }

    [Cmdlet(VerbsCommon.Find, "NotificationTemplate", DefaultParameterSetName = "All")]
    [OutputType(typeof(NotificationTemplate))]
    public class FindNotificationTemplateCommand : FindCommandBase
    {
        [Parameter(ParameterSetName = "AssociatedWith", ValueFromPipelineByPropertyName = true)]
        [ValidateSet(nameof(ResourceType.Organization))]
        public override ResourceType Type { get; set; }
        [Parameter(Mandatory = true, ParameterSetName = "AssociatedWith", ValueFromPipelineByPropertyName = true)]
        public override ulong Id { get; set; }

        [Parameter()]
        public override string[] OrderBy { get; set; } = ["id"];

        protected override void BeginProcessing()
        {
            SetupCommonQuery();
        }
        protected override void ProcessRecord()
        {
            var path = Id > 0 ? $"{Organization.PATH}{Id}/notification_templates/" : NotificationTemplate.PATH;
            foreach (var resultSet in GetResultSet<NotificationTemplate>(path, Query, All))
            {
                WriteObject(resultSet.Results, true);
            }
        }
    }

    [Cmdlet(VerbsCommon.Find, "NotificationTemplateForApproval")]
    [OutputType(typeof(NotificationTemplate))]
    public class FindNotificationTemplateForApprovalCommand : FindCommandBase
    {
        [Parameter(Mandatory = true, ValueFromPipelineByPropertyName = true)]
        [ValidateSet(nameof(ResourceType.Organization),
                     nameof(ResourceType.WorkflowJobTemplate))]
        public override ResourceType Type { get; set; }
        [Parameter(Mandatory = true, ValueFromPipelineByPropertyName = true)]
        public override ulong Id { get; set; }

        [Parameter()]
        public override string[] OrderBy { get; set; } = ["id"];

        protected override void BeginProcessing()
        {
            SetupCommonQuery();
        }
        protected override void ProcessRecord()
        {
            var path = Type switch
            {
                ResourceType.Organization => $"{Organization.PATH}{Id}/notification_templates_approvals/",
                ResourceType.WorkflowJobTemplate => $"{WorkflowJobTemplate.PATH}{Id}/notification_templates_approvals/",
                _ => throw new ArgumentException()
            };
            foreach (var resultSet in GetResultSet<NotificationTemplate>(path, Query, All))
            {
                WriteObject(resultSet.Results, true);
            }
        }
    }

    [Cmdlet(VerbsCommon.Find, "NotificationTemplateForError")]
    [OutputType(typeof(NotificationTemplate))]
    public class FindNotificationTemplateForErrorCommand : FindCommandBase
    {
        [Parameter(Mandatory = true, ValueFromPipelineByPropertyName = true)]
        [ValidateSet(nameof(ResourceType.Organization),
                     nameof(ResourceType.Project),
                     nameof(ResourceType.InventorySource),
                     nameof(ResourceType.JobTemplate),
                     nameof(ResourceType.SystemJobTemplate),
                     nameof(ResourceType.WorkflowJobTemplate))]
        public override ResourceType Type { get; set; }
        [Parameter(Mandatory = true, ValueFromPipelineByPropertyName = true)]
        public override ulong Id { get; set; }

        [Parameter()]
        public override string[] OrderBy { get; set; } = ["id"];

        protected override void BeginProcessing()
        {
            SetupCommonQuery();
        }
        protected override void ProcessRecord()
        {
            var path = Type switch
            {
                ResourceType.Organization => $"{Organization.PATH}{Id}/notification_templates_error/",
                ResourceType.Project => $"{Project.PATH}{Id}/notification_templates_error/",
                ResourceType.InventorySource => $"{InventorySource.PATH}{Id}/notification_templates_error/",
                ResourceType.JobTemplate => $"{JobTemplate.PATH}{Id}/notification_templates_error/",
                ResourceType.SystemJobTemplate => $"{SystemJobTemplate.PATH}{Id}/notification_templates_error/",
                ResourceType.WorkflowJobTemplate => $"{WorkflowJobTemplate.PATH}{Id}/notification_templates_error/",
                _ => throw new ArgumentException()
            };
            foreach (var resultSet in GetResultSet<NotificationTemplate>(path, Query, All))
            {
                WriteObject(resultSet.Results, true);
            }
        }
    }

    [Cmdlet(VerbsCommon.Find, "NotificationTemplateForStarted")]
    [OutputType(typeof(NotificationTemplate))]
    public class FindNotificationTemplateForStartedCommand : FindCommandBase
    {
        [Parameter(Mandatory = true, ValueFromPipelineByPropertyName = true)]
        [ValidateSet(nameof(ResourceType.Organization),
                     nameof(ResourceType.Project),
                     nameof(ResourceType.InventorySource),
                     nameof(ResourceType.JobTemplate),
                     nameof(ResourceType.SystemJobTemplate),
                     nameof(ResourceType.WorkflowJobTemplate))]
        public override ResourceType Type { get; set; }
        [Parameter(Mandatory = true, ValueFromPipelineByPropertyName = true)]
        public override ulong Id { get; set; }

        [Parameter()]
        public override string[] OrderBy { get; set; } = ["id"];

        protected override void BeginProcessing()
        {
            SetupCommonQuery();
        }
        protected override void ProcessRecord()
        {
            var path = Type switch
            {
                ResourceType.Organization => $"{Organization.PATH}{Id}/notification_templates_started/",
                ResourceType.Project => $"{Project.PATH}{Id}/notification_templates_started/",
                ResourceType.InventorySource => $"{InventorySource.PATH}{Id}/notification_templates_started/",
                ResourceType.JobTemplate => $"{JobTemplate.PATH}{Id}/notification_templates_started/",
                ResourceType.SystemJobTemplate => $"{SystemJobTemplate.PATH}{Id}/notification_templates_started/",
                ResourceType.WorkflowJobTemplate => $"{WorkflowJobTemplate.PATH}{Id}/notification_templates_started/",
                _ => throw new ArgumentException()
            };
            foreach (var resultSet in GetResultSet<NotificationTemplate>(path, Query, All))
            {
                WriteObject(resultSet.Results, true);
            }
        }
    }

    [Cmdlet(VerbsCommon.Find, "NotificationTemplateForSuccess")]
    [OutputType(typeof(NotificationTemplate))]
    public class FindNotificationTemplateForSuccessCommand : FindCommandBase
    {
        [Parameter(Mandatory = true, ValueFromPipelineByPropertyName = true)]
        [ValidateSet(nameof(ResourceType.Organization),
                     nameof(ResourceType.Project),
                     nameof(ResourceType.InventorySource),
                     nameof(ResourceType.JobTemplate),
                     nameof(ResourceType.SystemJobTemplate),
                     nameof(ResourceType.WorkflowJobTemplate))]
        public override ResourceType Type { get; set; }
        [Parameter(Mandatory = true, ValueFromPipelineByPropertyName = true)]
        public override ulong Id { get; set; }

        [Parameter()]
        public override string[] OrderBy { get; set; } = ["id"];

        protected override void BeginProcessing()
        {
            SetupCommonQuery();
        }
        protected override void ProcessRecord()
        {
            var path = Type switch
            {
                ResourceType.Organization => $"{Organization.PATH}{Id}/notification_templates_success/",
                ResourceType.Project => $"{Project.PATH}{Id}/notification_templates_success/",
                ResourceType.InventorySource => $"{InventorySource.PATH}{Id}/notification_templates_success/",
                ResourceType.JobTemplate => $"{JobTemplate.PATH}{Id}/notification_templates_success/",
                ResourceType.SystemJobTemplate => $"{SystemJobTemplate.PATH}{Id}/notification_templates_success/",
                ResourceType.WorkflowJobTemplate => $"{WorkflowJobTemplate.PATH}{Id}/notification_templates_success/",
                _ => throw new ArgumentException()
            };
            foreach (var resultSet in GetResultSet<NotificationTemplate>(path, Query, All))
            {
                WriteObject(resultSet.Results, true);
            }
        }
    }

    [Cmdlet(VerbsCommon.New, "NotificationTemplate", SupportsShouldProcess = true)]
    [OutputType(typeof(NotificationTemplate))]
    public class NewNotificationTemplateCommand : APICmdletBase
    {
        [Parameter(Mandatory = true, Position = 0)]
        public string Name { get; set; } = string.Empty;

        [Parameter()]
        [AllowEmptyString]
        public string? Description { get; set; }

        [Parameter(Mandatory = true)]
        [ResourceIdTransformation(AcceptableTypes = [ResourceType.Organization])]
        public ulong Organization { get; set; }

        [Parameter(Mandatory = true)]
        public NotificationType Type { get; set; }

        [Parameter()]
        public IDictionary Configuration { get; set; } = new Hashtable();

        [Parameter()]
        public IDictionary Messages { get; set; } = new Hashtable();

        protected override void ProcessRecord()
        {
            var sendData = new Dictionary<string, object>()
            {
                { "name", Name },
                { "organization", Organization },
                { "notification_type", $"{Type}".ToLowerInvariant() },
                { "notification_configuration", Configuration },
                { "messages", Messages }
            };
            if (Description != null)
                sendData.Add("description", Description);

            var dataDescription = Json.Stringify(sendData, pretty: true);
            if (ShouldProcess(dataDescription))
            {
                try
                {
                    var apiResult = CreateResource<NotificationTemplate>(NotificationTemplate.PATH, sendData);
                    if (apiResult.Contents == null)
                        return;

                    WriteObject(apiResult.Contents, false);
                }
                catch (RestAPIException) { }
            }
        }
    }

    [Cmdlet(VerbsData.Update, "NotificationTemplate", SupportsShouldProcess = true)]
    [OutputType(typeof(NotificationTemplate))]
    public class UpdateNotificationTemplateCommand : APICmdletBase
    {
        [Parameter(Mandatory = true, ValueFromPipeline = true, Position = 0)]
        [ResourceIdTransformation(AcceptableTypes = [ResourceType.NotificationTemplate])]
        public ulong Id { get; set; }

        [Parameter()]
        public string? Name { get; set; }

        [Parameter()]
        [AllowEmptyString]
        public string? Description { get; set; }

        [Parameter()]
        [ResourceIdTransformation(AcceptableTypes = [ResourceType.Organization])]
        public ulong? Organization { get; set; }

        [Parameter()]
        public NotificationType? Type { get; set; }

        [Parameter()]
        public IDictionary? Configuration { get; set; }

        [Parameter()]
        public IDictionary? Messages { get; set; }

        protected override void ProcessRecord()
        {
            var sendData = new Dictionary<string, object?>();
            if (!string.IsNullOrEmpty(Name))
                sendData.Add("name", Name);
            if (Description != null)
                sendData.Add("description", Description);
            if (Organization != null)
                sendData.Add("organization", Organization);
            if (Type != null)
                sendData.Add("notification_type", $"{Type}".ToLowerInvariant());
            if (Configuration != null)
                sendData.Add("notification_configuration", Configuration);
            if (Messages != null)
                sendData.Add("messages", Messages.Count == 0 ? null : Messages);

            if (sendData.Count == 0)
                return;

            var dataDescription = Json.Stringify(sendData, pretty: true);
            if (ShouldProcess($"NotificationTemplate [{Id}]", $"Update {dataDescription}"))
            {
                try
                {
                    var after = PatchResource<NotificationTemplate>($"{NotificationTemplate.PATH}{Id}/", sendData);
                    WriteObject(after, false);
                }
                catch (RestAPIException) { }
            }
        }
    }

    [Cmdlet(VerbsCommon.Remove, "NotificationTemplate", SupportsShouldProcess = true, ConfirmImpact = ConfirmImpact.High)]
    public class RemoveNotificationTemplateCommand : APICmdletBase
    {
        [Parameter(Mandatory = true, ValueFromPipeline = true, Position = 0)]
        [ResourceIdTransformation(AcceptableTypes = [ResourceType.NotificationTemplate])]
        public ulong Id { get; set; }

        [Parameter()]
        public SwitchParameter Force { get; set; }

        protected override void ProcessRecord()
        {
            if (Force || ShouldProcess($"NotificationTemplate [{Id}]", "Delete completely"))
            {
                try
                {
                    var apiResult = DeleteResource($"{NotificationTemplate.PATH}{Id}/");
                    if (apiResult?.IsSuccessStatusCode ?? false)
                    {
                        WriteVerbose($"NotificationTemplate {Id} is deleted.");
                    }
                }
                catch (RestAPIException) { }
            }
        }
    }

    [Cmdlet(VerbsLifecycle.Enable, "NotificationTemplate", SupportsShouldProcess = true)]
    public class EnableNotificationTemplateCommand : APICmdletBase
    {
        [Parameter(Mandatory = true, ValueFromPipeline = true, Position = 0)]
        [ResourceIdTransformation(AcceptableTypes = [ResourceType.NotificationTemplate])]
        public ulong Id { get; set; }

        [Parameter(Mandatory = true, Position = 1)]
        [ResourceTransformation(AcceptableTypes = [
                ResourceType.Organization,
                ResourceType.Project,
                ResourceType.InventorySource,
                ResourceType.JobTemplate,
                ResourceType.SystemJobTemplate,
                ResourceType.WorkflowJobTemplate
        ])]
        public IResource For { get; set; } = new Resource(0, 0);

        [Parameter(Mandatory = true, Position = 2)]
        [ValidateSet("Started", "Success", "Error", "Approval")]
        public string[] On { get; set; } = [];

        protected override void ProcessRecord()
        {
            var path1 = For.Type switch
            {
                ResourceType.Organization => $"{Organization.PATH}{For.Id}/",
                ResourceType.Project => $"{Project.PATH}{For.Id}/",
                ResourceType.InventorySource => $"{InventorySource.PATH}{For.Id}/",
                ResourceType.JobTemplate => $"{JobTemplate.PATH}{For.Id}/",
                ResourceType.SystemJobTemplate => $"{SystemJobTemplate.PATH}{For.Id}/",
                ResourceType.WorkflowJobTemplate => $"{WorkflowJobTemplate.PATH}{For.Id}/",
                _ => throw new ArgumentException($"Invalid resource type: {For.Type}")
            };
            foreach (var timing in On)
            {
                if (timing == "Approval" && (For.Type != ResourceType.Organization
                                             && For.Type != ResourceType.WorkflowJobTemplate))
                {
                    WriteWarning($"{For.Type} has no \"{timing}\" notifications.");
                    continue;
                }
                var path2 = timing switch
                {
                    "Started" => "notification_templates_started/",
                    "Success" => "notification_templates_success/",
                    "Error" => "notification_templates_error/",
                    "Approval" => "notification_templates_approvals/",
                    _ => throw new ArgumentException($"(Invalid timing value: {timing}")
                };
                if (ShouldProcess($"NotificationTemplate [{Id}]", $"Enable to {For.Type} [{For.Id}] on {timing}"))
                {
                    var path = path1 + path2;
                    var sendData = new Dictionary<string, object>() { { "id", Id } };
                    try
                    {
                        var apiResult = CreateResource<string>(path, sendData);
                        if (apiResult.Response.IsSuccessStatusCode)
                        {
                            WriteVerbose($"NotificationTemplate {Id} is enabled to {For.Type} [{For.Id}] on {timing}.");
                        }
                    }
                    catch (RestAPIException) { }
                }
            }
        }
    }

    [Cmdlet(VerbsLifecycle.Disable, "NotificationTemplate", SupportsShouldProcess = true)]
    public class DisableNotificationTemplateCommand : APICmdletBase
    {
        [Parameter(Mandatory = true, ValueFromPipeline = true, Position = 0)]
        [ResourceIdTransformation(AcceptableTypes = [ResourceType.NotificationTemplate])]
        public ulong Id { get; set; }

        [Parameter(Mandatory = true, Position = 1)]
        [ResourceTransformation(AcceptableTypes = [
                ResourceType.Organization,
                ResourceType.Project,
                ResourceType.InventorySource,
                ResourceType.JobTemplate,
                ResourceType.SystemJobTemplate,
                ResourceType.WorkflowJobTemplate
        ])]
        public IResource For { get; set; } = new Resource(0, 0);

        [Parameter(Mandatory = true, Position = 2)]
        [ValidateSet("Started", "Success", "Error", "Approval")]
        public string[] On { get; set; } = [];

        protected override void ProcessRecord()
        {
            var path1 = For.Type switch
            {
                ResourceType.Organization => $"{Organization.PATH}{For.Id}/",
                ResourceType.Project => $"{Project.PATH}{For.Id}/",
                ResourceType.InventorySource => $"{InventorySource.PATH}{For.Id}/",
                ResourceType.JobTemplate => $"{JobTemplate.PATH}{For.Id}/",
                ResourceType.SystemJobTemplate => $"{SystemJobTemplate.PATH}{For.Id}/",
                ResourceType.WorkflowJobTemplate => $"{WorkflowJobTemplate.PATH}{For.Id}/",
                _ => throw new ArgumentException($"Invalid resource type: {For.Type}")
            };
            foreach (var timing in On)
            {
                if (timing == "Approval" && (For.Type != ResourceType.Organization
                                             && For.Type != ResourceType.WorkflowJobTemplate))
                {
                    WriteWarning($"{For.Type} has no \"{timing}\" notifications.");
                    continue;
                }
                var path2 = timing switch
                {
                    "Started" => "notification_templates_started/",
                    "Success" => "notification_templates_success/",
                    "Error" => "notification_templates_error/",
                    "Approval" => "notification_templates_approvals/",
                    _ => throw new ArgumentException($"(Invalid timing value: {timing}")
                };
                if (ShouldProcess($"NotificationTemplate [{Id}]", $"Disable to {For.Type} [{For.Id}] on {timing}"))
                {
                    var path = path1 + path2;
                    var sendData = new Dictionary<string, object>()
                    {
                        { "id", Id },
                        { "disassociate", true }
                    };
                    try
                    {
                        var apiResult = CreateResource<string>(path, sendData);
                        if (apiResult.Response.IsSuccessStatusCode)
                        {
                            WriteVerbose($"NotificationTemplate {Id} is disabled to {For.Type} [{For.Id}] on {timing}.");
                        }
                    }
                    catch (RestAPIException) { }
                }
            }
        }
    }
}
