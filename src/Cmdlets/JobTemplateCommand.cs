using AWX.Resources;
using System.Collections;
using System.Management.Automation;
using System.Text;

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
        public ulong Id { get; set; }
        [Parameter(Mandatory = true, ParameterSetName = "JobTemplate", ValueFromPipeline = true, Position = 0)]
        public JobTemplate? JobTemplate { get; set; }

        protected abstract bool Async { get; }

        [Parameter()]
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
        private void ShowJobTemplateInfo(JobTemplateLaunchRequirements requirements)
        {
            var jt = requirements.JobTemplateData;
            var def = requirements.Defaults;
            WriteHost($"[{jt.Id}] {jt.Name} - {jt.Description}\n");
            var fmt = "{0,22} : {1}\n";
            if (def.Inventory.Id != null)
            {
                WriteHost(string.Format(fmt, "Inventory", $"[{def.Inventory.Id}] {def.Inventory.Name}"),
                            foregroundColor: requirements.AskInventoryOnLaunch ? ConsoleColor.Magenta : ConsoleColor.Green);
            }
            if (!string.IsNullOrEmpty(def.Limit) || Limit != null)
            {
                var limitVal = def.Limit + (Limit != null ? $" => {Limit}" : "");

                WriteHost(string.Format(fmt, "Limit", $"{limitVal}"),
                            foregroundColor: requirements.AskLimitOnLaunch ? Limit == null ? ConsoleColor.Magenta : ConsoleColor.Green : null);
            }
            if (!string.IsNullOrEmpty(def.ScmBranch))
            {
                WriteHost(string.Format(fmt, "Scm Branch", def.ScmBranch),
                            foregroundColor: requirements.AskScmBranchOnLaunch? ConsoleColor.Magenta : ConsoleColor.Green);
            }
            if (def.Labels != null && def.Labels.Length > 0)
            {
                WriteHost(string.Format(fmt, "Labels", string.Join(", ", def.Labels.Select(l => $"[{l.Id}] {l.Name}"))),
                            foregroundColor: requirements.AskLabelsOnLaunch ? ConsoleColor.Magenta : ConsoleColor.Green);
            }
            if (!string.IsNullOrEmpty(def.JobTags))
            {
                WriteHost(string.Format(fmt, "Job tags", def.JobTags),
                            foregroundColor: requirements.AskTagsOnLaunch ? ConsoleColor.Magenta : ConsoleColor.Green);
            }
            if (!string.IsNullOrEmpty(def.SkipTags))
            {
                WriteHost(string.Format(fmt, "Skip tags", def.SkipTags),
                            foregroundColor: requirements.AskSkipTagsOnLaunch ? ConsoleColor.Magenta : ConsoleColor.Green);
            }
            if (!string.IsNullOrEmpty(def.ExtraVars))
            {
                var sb = new StringBuilder();
                var lines = def.ExtraVars.Split('\n');
                sb.Append(string.Format(fmt, "Extra vars", lines[0]));
                foreach (var line in lines[1..])
                {
                    sb.AppendLine("".PadLeft(25) + line);
                }
                WriteHost(sb.ToString(),
                            foregroundColor: requirements.AskVariablesOnLaunch ? ConsoleColor.Magenta : ConsoleColor.Green);
            }
            WriteHost(string.Format(fmt, "Diff Mode", def.DiffMode),
                            foregroundColor: requirements.AskDiffModeOnLaunch ? ConsoleColor.Magenta : ConsoleColor.Green);
            WriteHost(string.Format(fmt, "Job Type", def.JobType),
                            foregroundColor: requirements.AskJobTypeOnLaunch ? ConsoleColor.Magenta : ConsoleColor.Green);
            WriteHost(string.Format(fmt, "Verbosity", $"{def.Verbosity:d} ({def.Verbosity})"),
                            foregroundColor: requirements.AskVerbosityOnLaunch ? ConsoleColor.Magenta : ConsoleColor.Green);
            if (def.Credentials != null)
            {
                WriteHost(string.Format(fmt, "Credentials", string.Join(", ", def.Credentials.Select(c => $"[{c.Id}] {c.Name}"))),
                            foregroundColor: requirements.AskCredentialOnLaunch ? ConsoleColor.Magenta : ConsoleColor.Green);
            }
            if (def.ExecutionEnvironment.Id != null)
            {
                WriteHost(string.Format(fmt, "ExecutionEnvironment", $"[{def.ExecutionEnvironment.Id}] {def.ExecutionEnvironment.Name}"),
                            foregroundColor: requirements.AskExecutionEnvironmentOnLaunch ? ConsoleColor.Magenta : ConsoleColor.Green);
            }
            WriteHost(string.Format(fmt, "Forks", def.Forks),
                            foregroundColor: requirements.AskForksOnLaunch ? ConsoleColor.Magenta : ConsoleColor.Green);
            WriteHost(string.Format(fmt, "Job Slice Count", def.JobSliceCount),
                            foregroundColor: requirements.AskJobSliceCountOnLaunch ? ConsoleColor.Magenta : ConsoleColor.Green);
            WriteHost(string.Format(fmt, "Timeout", def.Timeout),
                            foregroundColor: requirements.AskTimeoutOnLaunch ? ConsoleColor.Magenta : ConsoleColor.Green);
        }
        protected JobTemplateJob.LaunchResult? Launch(ulong id)
        {
            var requirements = GetResource<JobTemplateLaunchRequirements>($"{JobTemplate.PATH}{id}/launch/");
            if (requirements == null)
            {
                return null;
            }
            ShowJobTemplateInfo(requirements);
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
    [OutputType(typeof(JobTemplateJob))]
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
            try
            {
                var launchResult = Launch(Id);
                if (launchResult != null)
                {
                    JobManager.Add(launchResult);
                }
            }
            catch (RestAPIException) {}
        }
        protected override void EndProcessing()
        {
            WaitJobs("Launch JobTemplate", IntervalSeconds, SuppressJobLog);
        }

    }

    [Cmdlet(VerbsLifecycle.Start, "JobTemplate")]
    [OutputType(typeof(JobTemplateJob.LaunchResult))]
    public class StartJobTemplateCommand : LaunchJobTemplateCommandBase
    {
        protected override bool Async { get; } = true;

        protected override void ProcessRecord()
        {
            if (JobTemplate != null)
            {
                Id = JobTemplate.Id;
            }
            try
            {
                var launchResult = Launch(Id);
                if (launchResult != null)
                {
                    WriteObject(launchResult, false);
                }
            }
            catch (RestAPIException) {}
        }
    }
}
