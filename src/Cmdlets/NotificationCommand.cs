using AWX.Resources;
using System.Management.Automation;

namespace AWX.Cmdlets
{
    [Cmdlet(VerbsCommon.Get, "Notification")]
    [OutputType(typeof(Notification))]
    public class GetNotificationCommand : GetCmdletBase
    {
        protected override void ProcessRecord()
        {
            foreach (var id in Id)
            {
                IdSet.Add(id);
            }
        }
        protected override void EndProcessing()
        {
            string path;
            if (IdSet.Count == 1)
            {
                path = $"{Notification.PATH}{IdSet.First()}/";
                var res = GetResource<Notification>(path);
                WriteObject(res);
            }
            else
            {
                path = Notification.PATH;
                Query.Add("id__in", string.Join(',', IdSet));
                Query.Add("page_size", $"{IdSet.Count}");
                foreach (var resultSet in GetResultSet<Notification>(path, Query))
                {
                    WriteObject(resultSet.Results, true);
                }
            }
        }
    }

    [Cmdlet(VerbsCommon.Find, "Notification", DefaultParameterSetName = "All")]
    [OutputType(typeof(Notification))]
    public class FindNotificationCommand : FindCmdletBase
    {
        [Parameter(Mandatory = true, ParameterSetName = "AssociatedWith", ValueFromPipelineByPropertyName = true)]
        [ValidateSet(nameof(ResourceType.NotificationTemplate),
                     nameof(ResourceType.Job),
                     nameof(ResourceType.WorkflowJob),
                     nameof(ResourceType.SystemJob),
                     nameof(ResourceType.ProjectUpdate),
                     nameof(ResourceType.InventoryUpdate),
                     nameof(ResourceType.AdHocCommand))]
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
            var path = Type switch
            {
                ResourceType.NotificationTemplate => $"{NotificationTemplate.PATH}{Id}/notifications/",
                ResourceType.Job => $"{JobTemplateJob.PATH}{Id}/notifications/",
                ResourceType.WorkflowJob => $"{WorkflowJob.PATH}{Id}/notifications/",
                ResourceType.SystemJob => $"{SystemJob.PATH}{Id}/notifications/",
                ResourceType.ProjectUpdate => $"{ProjectUpdateJob.PATH}{Id}/notifications/",
                ResourceType.InventoryUpdate => $"{InventoryUpdateJob.PATH}{Id}/notifications/",
                ResourceType.AdHocCommand => $"{AdHocCommand.PATH}{Id}/notifications/",
                _ => Notification.PATH
            };
            foreach (var resultSet in GetResultSet<Notification>(path, Query, All))
            {
                WriteObject(resultSet.Results, true);
            }
        }
    }
}

