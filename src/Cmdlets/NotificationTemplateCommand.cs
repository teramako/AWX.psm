using AWX.Resources;
using System.Collections;
using System.Management.Automation;

namespace AWX.Cmdlets
{
    [Cmdlet(VerbsCommon.Get, "NotificationTemplate")]
    [OutputType(typeof(NotificationTemplate))]
    public class GetNotificationTemplateCommand : GetCmdletBase
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
    public class FindNotificationTemplateCommand : FindCmdletBase
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
    public class FindNotificationTemplateForApprovalCommand : FindCmdletBase
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
    public class FindNotificationTemplateForErrorCommand : FindCmdletBase
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
    public class FindNotificationTemplateForStartedCommand : FindCmdletBase
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
    public class FindNotificationTemplateForSuccessCommand : FindCmdletBase
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
        public string Description { get; set; } = string.Empty;

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
                { "description", Description },
                { "organization", Organization },
                { "notification_type", $"{Type}".ToLowerInvariant() },
                { "notification_configuration", Configuration },
                { "messages", Messages }
            };
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
}
