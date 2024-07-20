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

    public abstract class LaunchJobTemplateCommandBase : InvokeJobBase
    {
        [Parameter(Mandatory = true, ParameterSetName = "Id", ValueFromPipeline = true, Position = 0)]
        [Parameter(Mandatory = true, ParameterSetName = "GetRequirementsId", ValueFromPipeline = true, Position = 0)]
        public ulong Id { get; set; }
        [Parameter(Mandatory = true, ParameterSetName = "JobTemplate", ValueFromPipeline = true, Position = 0)]
        [Parameter(Mandatory = true, ParameterSetName = "GetRequirementsJobTemplate", ValueFromPipeline = true, Position = 0)]
        public JobTemplate? JobTemplate { get; set; }

        [Parameter(Mandatory = true, ParameterSetName = "GetRequirementsId")]
        [Parameter(Mandatory = true, ParameterSetName = "GetRequirementsJobTemplate")]
        public SwitchParameter GetRequirements { get; set; }

        protected abstract bool Async { get; }

        [Parameter(ParameterSetName = "Id")]
        [Parameter(ParameterSetName = "JobTemplate")]
        public string? Limit { get; set; }

        private Hashtable CreateSendData()
        {
            var dict = new Hashtable();
            if (Limit != null)
            {
                dict.Add("limit", Limit);
            }
            return dict;
        }
        protected void GetLaunchRequirements(ulong id)
        {
            var res = GetResource<JobTemplateLaunchRequirements>($"{JobTemplate.PATH}{id}/launch/");
            if (res == null)
            {
                return;
            }
            WriteObject(res, false);
        }
        protected JobTemplateJob.LaunchResult Launch(ulong id)
        {
            var apiResult = CreateResource<JobTemplateJob.LaunchResult>($"{JobTemplate.PATH}{id}/launch/", CreateSendData());
            var launchResult = apiResult.Contents;
            WriteVerbose($"Launch JobTemplate:{id} => Job:[{launchResult.Id}]");
            if (launchResult.IgnoredFields.Count > 0)
            {
                foreach (var (key, val) in launchResult.IgnoredFields)
                {
                    WriteWarning($"Ignored field: {key} ({val})");
                }
            }
            return launchResult;
        }
    }

    [Cmdlet(VerbsLifecycle.Invoke, "JobTemplate")]
    [OutputType(typeof(JobTemplateJob), ParameterSetName = ["Id", "JobTemplate"])]
    [OutputType(typeof(JobTemplateLaunchRequirements), ParameterSetName = ["GetRequirementsId", "GetRequirementsJobTemplate"])]
    public class InvokeJobTemplateCommand : LaunchJobTemplateCommandBase
    {
        protected override bool Async { get; } = false;

        [Parameter(ParameterSetName = "Id")]
        [Parameter(ParameterSetName = "JobTemplate")]
        [ValidateRange(5, int.MaxValue)]
        public int IntervalSeconds { get; set; } = 5;

        [Parameter(ParameterSetName = "Id")]
        [Parameter(ParameterSetName = "JobTemplate")]
        public SwitchParameter SuppressJobLog { get; set; }

        protected override void ProcessRecord()
        {
            if (JobTemplate != null)
            {
                Id = JobTemplate.Id;
            }
            if (GetRequirements)
            {
                GetLaunchRequirements(Id);
            }
            else
            {
                try
                {
                    var launchResult = Launch(Id);
                    JobManager.Add(launchResult);
                }
                catch (RestAPIException) {}
            }
        }
        protected override void EndProcessing()
        {
            if (GetRequirements)
            {
                return;
            }
            WaitJobs("Launch JobTemplate", IntervalSeconds, SuppressJobLog);
        }

    }

    [Cmdlet(VerbsLifecycle.Start, "JobTemplate")]
    [OutputType(typeof(JobTemplateJob.LaunchResult), ParameterSetName = ["Id", "JobTemplate"])]
    [OutputType(typeof(JobTemplateLaunchRequirements), ParameterSetName = ["GetRequirementsId", "GetRequirementsJobTemplate"])]
    public class StartJobTemplateCommand : LaunchJobTemplateCommandBase
    {
        protected override bool Async { get; } = true;

        protected override void ProcessRecord()
        {
            if (JobTemplate != null)
            {
                Id = JobTemplate.Id;
            }
            if (GetRequirements)
            {
                GetLaunchRequirements(Id);
            }
            else
            {
                try
                {
                    var launchResult = Launch(Id);
                    WriteObject(launchResult, false);
                }
                catch (RestAPIException) { }
            }
        }
    }
}
