using AWX.Resources;
using System.Collections;
using System.Management.Automation;

namespace AWX.Cmdlets
{
    [Cmdlet(VerbsCommon.Get, "JobTemplate")]
    [OutputType(typeof(JobTemplate))]
    public class GetJobTemplate : GetCmdletBase
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
            Query.Add("id__in", string.Join(',', IdSet));
            Query.Add("page_size", $"{IdSet.Count}");
            foreach (var resultSet in GetResultSet<JobTemplate>($"{JobTemplate.PATH}?{Query}", true))
            {
                WriteObject(resultSet.Results, true);
            }
        }
    }

    [Cmdlet(VerbsCommon.Find, "JobTemplate", DefaultParameterSetName = "All")]
    [OutputType(typeof(JobTemplate))]
    public class FindJobTemplateCommand : FindCmdletBase
    {
        [Parameter(Mandatory = true, ParameterSetName = "AssociatedWith", ValueFromPipelineByPropertyName = true)]
        public override ulong Id { get; set; }
        [Parameter(Mandatory = true, ParameterSetName = "AssociatedWith", ValueFromPipelineByPropertyName = true)]
        [ValidateSet(nameof(ResourceType.Organization),
                     nameof(ResourceType.Inventory))]
        public override ResourceType Type { get; set; }

        [Parameter(Position = 0)]
        public string[]? Name { get; set; }

        [Parameter()]
        public override string[] OrderBy { get; set; } = ["!id"];

        protected override void BeginProcessing()
        {
            if (Name != null)
            {
                Query.Add("name__in", string.Join(',', Name));
            }
            SetupCommonQuery();
        }
        protected override void EndProcessing()
        {
            var path = Type switch
            {
                ResourceType.Organization => $"{Organization.PATH}{Id}/job_templates/",
                ResourceType.Inventory => $"{Inventory.PATH}{Id}/job_templates/",
                _ => JobTemplate.PATH
            };
            foreach (var resultSet in GetResultSet<JobTemplate>(path, Query, All))
            {
                WriteObject(resultSet.Results, true);
            }
        }
    }

    [Cmdlet(VerbsLifecycle.Invoke, "JobTemplate")]
    public class InvokeJobTemplateCommand : InvokeJobBase
    {
        [Parameter(Mandatory = true, ParameterSetName = "Id")]
        [Parameter(Mandatory = true, ParameterSetName = "AsyncId")]
        public ulong Id { get; set; }
        [Parameter(Mandatory = true, ParameterSetName = "JobTemplate", ValueFromPipeline = true)]
        [Parameter(Mandatory = true, ParameterSetName = "AsyncJobTemplate", ValueFromPipeline = true)]
        public JobTemplate? JobTemplate { get; set; }

        [Parameter(Mandatory = true, ParameterSetName = "AsyncId")]
        [Parameter(Mandatory = true, ParameterSetName = "AsyncJobTemplate")]
        public SwitchParameter Async { get; set; }

        [Parameter()]
        public string? Limit { get; set; }

        [Parameter(ParameterSetName = "Id")]
        [Parameter(ParameterSetName = "JobTemplate")]
        [ValidateRange(5, int.MaxValue)]
        public int IntervalSeconds { get; set; } = 5;

        [Parameter(ParameterSetName = "Id")]
        [Parameter(ParameterSetName = "JobTemplate")]
        public SwitchParameter SuppressJobLog { get; set; }

        private Hashtable CreateSendData()
        {
            var dict = new Hashtable();
            if (Limit != null)
            {
                dict.Add("limit", Limit);
            }
            return dict;
        }
        protected override void ProcessRecord()
        {
            if (JobTemplate != null)
            {
                Id = JobTemplate.Id;
            }
            var launchResult = CreateResource<JobTemplateJob.LaunchResult>($"{JobTemplate.PATH}{Id}/launch/", CreateSendData());
            if (launchResult == null)
            {
                return;
            }
            WriteVerbose($"Launch JobTemplate:{Id} => Job:[{launchResult.Id}]");
            if (Async)
            {
                WriteObject(launchResult, false);
            }
            else
            {
                jobTasks.Add(launchResult.Id, new JobTask(launchResult));
            }
        }
        protected override void EndProcessing()
        {
            if (Async)
            {
                return;
            }
            else
            {
                WaitJobs("Launch Jobs", IntervalSeconds, SuppressJobLog);
            }
        }
    }
}
