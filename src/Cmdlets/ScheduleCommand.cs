using AWX.Resources;
using System.Management.Automation;

namespace AWX.Cmdlets
{
    [Cmdlet(VerbsCommon.Get, "Schedule")]
    [OutputType(typeof(Schedule))]
    public class GetScheduleCommand : GetCmdletBase
    {
        protected override void ProcessRecord()
        {
            if (Type != null && Type != ResourceType.Schedule)
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
            string path;
            if (IdSet.Count == 1)
            {
                path = $"{Schedule.PATH}{IdSet.First()}/";
                var res = GetResource<Schedule>(path);
                WriteObject(res);
            }
            else
            {
                path = Schedule.PATH;
                Query.Add("id__in", string.Join(',', IdSet));
                Query.Add("page_size", $"{IdSet.Count}");
                foreach (var resultSet in GetResultSet<Schedule>(path, Query))
                {
                    WriteObject(resultSet.Results, true);
                }
            }
        }
    }
    [Cmdlet(VerbsCommon.Find, "Schedule", DefaultParameterSetName = "All")]
    [OutputType(typeof(Schedule))]
    public class FindScheduleCommand : FindCmdletBase
    {
        [Parameter(Mandatory = true, ParameterSetName = "AssociatedWith", ValueFromPipelineByPropertyName = true)]
        [ValidateSet(nameof(ResourceType.Project),
                     nameof(ResourceType.InventorySource),
                     nameof(ResourceType.JobTemplate),
                     nameof(ResourceType.SystemJobTemplate),
                     nameof(ResourceType.WorkflowJobTemplate))]
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
                ResourceType.Project => $"{Project.PATH}{Id}/schedules/",
                ResourceType.InventorySource => $"{InventorySource.PATH}{Id}/schedules/",
                ResourceType.JobTemplate => $"{JobTemplate.PATH}{Id}/schedules/",
                ResourceType.SystemJobTemplate => $"{SystemJobTemplate.PATH}{Id}/schedules/",
                ResourceType.WorkflowJobTemplate => $"{WorkflowJobTemplate.PATH}{Id}/schedules/",
                _ => Schedule.PATH
            };
            foreach (var resultSet in GetResultSet<Schedule>(path, Query, All))
            {
                WriteObject(resultSet.Results, true);
            }
        }
    }
}
