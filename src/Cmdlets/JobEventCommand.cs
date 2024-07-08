using AWX.Resources;
using System.Management.Automation;

namespace AWX.Cmdlets
{
    [Cmdlet(VerbsCommon.Find, "JobEvent")]
    [OutputType(typeof(IJobEventBase))]
    public class FindJobEventCommand : FindCmdletBase
    {
        [Parameter(Mandatory = true, ValueFromPipelineByPropertyName = true, Position = 0)]
        [ValidateSet(nameof(ResourceType.Job),
                     nameof(ResourceType.ProjectUpdate),
                     nameof(ResourceType.InventoryUpdate),
                     nameof(ResourceType.SystemJob),
                     nameof(ResourceType.AdHocCommand),
                     nameof(ResourceType.Host),
                     nameof(ResourceType.Group))]
        public override ResourceType Type { get; set; }
        [Parameter(Mandatory = true, ValueFromPipelineByPropertyName = true, Position = 1)]
        public override ulong Id { get; set; }

        [Parameter()]
        public SwitchParameter AdHocCommandEvent { get; set; }

        [Parameter()]
        public override string[] OrderBy { get; set; } = ["counter"];

        private void FindJobEvent<T>(string path) where T : class
        {
            foreach (var resultSet in GetResultSet<T>(path, Query, All))
            {
                WriteObject(resultSet.Results, true);
            }
        }
        protected override void EndProcessing()
        {
            Query.Clear();
            SetupCommonQuery();

            switch (Type)
            {
                case ResourceType.Job:
                    FindJobEvent<JobEvent>($"{JobTemplateJob.PATH}{Id}/job_events/");
                    break;
                case ResourceType.Host:
                    if (AdHocCommandEvent)
                    {
                        FindJobEvent<AdHocCommandJobEvent>($"{Host.PATH}{Id}/ad_hoc_command_events/");
                    }
                    else
                    {
                        if (OrderBy.Length == 1 && OrderBy[0] == "counter")
                        {
                            Query.Set("order_by", "-job,counter");
                        }
                        FindJobEvent<JobEvent>($"{Host.PATH}{Id}/job_events/");
                    }
                    break;
                case ResourceType.Group:
                    if (OrderBy.Length == 1 && OrderBy[0] == "counter")
                    {
                        Query.Set("order_by", "-job,counter");
                    }
                    FindJobEvent<JobEvent>($"{Group.PATH}{Id}/job_events/");
                    break;
                case ResourceType.ProjectUpdate:
                    FindJobEvent<ProjectUpdateJobEvent>($"{ProjectUpdateJob.PATH}{Id}/events/");
                    break;
                case ResourceType.InventoryUpdate:
                    FindJobEvent<InventoryUpdateJobEvent>($"{InventoryUpdateJob.PATH}{Id}/events/");
                    break;
                case ResourceType.SystemJob:
                    FindJobEvent<SystemJobEvent>($"{SystemJob.PATH}{Id}/events/");
                    break;
                case ResourceType.AdHocCommand:
                    FindJobEvent<AdHocCommandJobEvent>($"{AdHocCommand.PATH}{Id}/events/");
                    break;
            }
        }
    }
}
