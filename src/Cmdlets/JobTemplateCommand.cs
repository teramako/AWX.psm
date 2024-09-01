using AWX.Resources;
using System.Management.Automation;
using System.Text;
using System.Text.Json;

namespace AWX.Cmdlets
{
    [Cmdlet(VerbsCommon.Get, "JobTemplate")]
    [OutputType(typeof(JobTemplate))]
    public class GetJobTemplate : GetCmdletBase
    {
        protected override void ProcessRecord()
        {
            if (Type != null && Type != ResourceType.JobTemplate)
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
                var result = GetResource<JobTemplate>($"{JobTemplate.PATH}{IdSet.First()}/");
                WriteObject(result);
            }
            else
            {
                Query.Add("id__in", string.Join(',', IdSet));
                Query.Add("page_size", $"{IdSet.Count}");
                foreach (var resultSet in GetResultSet<JobTemplate>(JobTemplate.PATH, Query, true))
                {
                    WriteObject(resultSet.Results, true);
                }
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

        [Parameter()]
        public ulong? Inventory { get; set; }

        [Parameter()]
        [ValidateSet(nameof(Resources.JobType.Run), nameof(Resources.JobType.Check))]
        public JobType? JobType { get; set; }

        [Parameter()]
        public string? ScmBranch { get; set; }

        [Parameter()]
        public ulong[]? Credentials { get; set; }

        [Parameter()]
        public string? Limit { get; set; }

        [Parameter()] // XXX: Should be string[] and created if not exists ?
        public ulong[]? Labels { get; set; }

        [Parameter()]
        public string[]? Tags { get; set; }

        [Parameter()]
        public string[]? SkipTags { get; set; }

        [Parameter()] // TODO: Should accept `IDctionary` (convert to JSON serialized string)
        public string? ExtraVars { get; set; }

        [Parameter()]
        public bool? DiffMode { get; set; }

        [Parameter()]
        public JobVerbosity? Verbosity { get; set; }

        [Parameter()]
        [ValidateRange(0, int.MaxValue)]
        public int? Forks { get; set; }

        [Parameter()]
        public ulong? ExecutionEnvironment { get; set; }

        [Parameter()]
        [ValidateRange(0, int.MaxValue)]
        public int? JobSliceCount { get; set; }

        [Parameter()]
        public int? Timeout { get; set; }

        private IDictionary<string, object?> CreateSendData()
        {
            var dict = new Dictionary<string, object?>();
            if (Inventory != null)
            {
                dict.Add("inventory", Inventory);
            }
            if (JobType != null)
            {
                dict.Add("job_type", $"{JobType}".ToLowerInvariant());
            }
            if (ScmBranch != null)
            {
                dict.Add("scm_branch", ScmBranch);
            }
            if (Credentials != null)
            {
                dict.Add("credentials", Credentials);
            }
            if (Limit != null)
            {
                dict.Add("limit", Limit);
            }
            if (Labels != null)
            {
                dict.Add("labels", Labels);
            }
            if (Tags != null)
            {
                dict.Add("job_tags", string.Join(',', Tags));
            }
            if (SkipTags != null)
            {
                dict.Add("skip_tags", string.Join(',', SkipTags));
            }
            if (!string.IsNullOrEmpty(ExtraVars))
            {
                dict.Add("extra_vars", ExtraVars);
            }
            if (DiffMode != null)
            {
                dict.Add("diff_mode", DiffMode);
            }
            if (Verbosity != null)
            {
                dict.Add("verbosity", (int)Verbosity);
            }
            if (Forks != null)
            {
                dict.Add("forks", Forks);
            }
            if (ExecutionEnvironment != null)
            {
                dict.Add("execution_environment", ExecutionEnvironment);
            }
            if (JobSliceCount != null)
            {
                dict.Add("job_slice_count", JobSliceCount);
            }
            if (Timeout != null)
            {
                dict.Add("timeout", Timeout);
            }
            return dict;
        }
        private void ShowJobTemplateInfo(JobTemplateLaunchRequirements requirements)
        {
            var jt = requirements.JobTemplateData;
            var def = requirements.Defaults;
            var (fixedColor, implicitColor, explicitColor, requiredColor) =
                ((ConsoleColor?)null, ConsoleColor.Magenta, ConsoleColor.Green, ConsoleColor.Red);
            WriteHost($"[{jt.Id}] {jt.Name} - {jt.Description}\n");
            var fmt = "{0,22} : {1}\n";
            {
                var inventoryVal = (def.Inventory.Id != null ? $"[{def.Inventory.Id}] {def.Inventory.Name}" : "Undefined")
                                   + (requirements.AskInventoryOnLaunch && Inventory != null ? $" => [{Inventory}]" : "");
                WriteHost(string.Format(fmt, "Inventory", inventoryVal),
                            foregroundColor: requirements.AskInventoryOnLaunch ? (Inventory == null ? implicitColor : explicitColor) : fixedColor);
            }
            if (!string.IsNullOrEmpty(def.Limit) || Limit != null)
            {
                var limitVal = def.Limit
                               + (requirements.AskLimitOnLaunch && Limit != null ? $" => {Limit}" : "");
                WriteHost(string.Format(fmt, "Limit", $"{limitVal}"),
                            foregroundColor: requirements.AskLimitOnLaunch ? (Limit == null ? implicitColor : explicitColor) : fixedColor);
            }
            if (!string.IsNullOrEmpty(def.ScmBranch) || ScmBranch != null)
            {
                var branchVal = def.ScmBranch
                                + (requirements.AskScmBranchOnLaunch && ScmBranch != null ? $" => {ScmBranch}" : "");
                WriteHost(string.Format(fmt, "Scm Branch", branchVal),
                            foregroundColor: requirements.AskScmBranchOnLaunch ? (ScmBranch == null ? implicitColor : explicitColor) : fixedColor);
            }
            if ((def.Labels != null && def.Labels.Length > 0) || Labels != null)
            {
                var labelsVal = string.Join(", ", def.Labels?.Select(l => $"[{l.Id}] {l.Name}") ?? [])
                                + (requirements.AskLabelsOnLaunch && Labels != null ? $" => {string.Join(',', Labels.Select(id => $"[{id}]"))}" : "");
                WriteHost(string.Format(fmt, "Labels", labelsVal),
                            foregroundColor: requirements.AskLabelsOnLaunch ? (Labels ==  null ? implicitColor : explicitColor) : fixedColor);
            }
            if (!string.IsNullOrEmpty(def.JobTags) || Tags != null)
            {
                var tagsVal = def.JobTags
                              + (requirements.AskTagsOnLaunch && Tags != null ? $" => {string.Join(", ", Tags)}" : "");
                WriteHost(string.Format(fmt, "Job tags", tagsVal),
                            foregroundColor: requirements.AskTagsOnLaunch ? (Tags == null ? implicitColor : explicitColor) : fixedColor);
            }
            if (!string.IsNullOrEmpty(def.SkipTags) || SkipTags != null)
            {
                var skipTagsVal = def.SkipTags
                                  + (requirements.AskSkipTagsOnLaunch && SkipTags != null ? $" => {string.Join(", ", SkipTags)}" : "");
                WriteHost(string.Format(fmt, "Skip tags", skipTagsVal),
                            foregroundColor: requirements.AskSkipTagsOnLaunch ? (SkipTags == null ? implicitColor : explicitColor) : fixedColor);
            }
            if (!string.IsNullOrEmpty(def.ExtraVars) || !string.IsNullOrEmpty(ExtraVars))
            {
                var sb = new StringBuilder();
                var lines = def.ExtraVars.Split('\n');
                var padding = "".PadLeft(25);
                sb.Append(string.Format(fmt, "Extra vars", lines[0]));
                foreach (var line in lines[1..])
                {
                    sb.AppendLine(padding + line);
                }
                if (!string.IsNullOrEmpty(ExtraVars))
                {
                    sb.AppendLine($"{padding}=> (overwrite or append)");
                    lines = ExtraVars.Split('\n');
                    foreach (var line in lines)
                    {
                        sb.AppendLine(padding + line);
                    }
                }
                WriteHost(sb.ToString(),
                            foregroundColor: requirements.AskVariablesOnLaunch ? (ExtraVars == null ? implicitColor : explicitColor) : fixedColor);
            }
            if (requirements.SurveyEnabled)
            {
                WriteHost(string.Format(fmt, "Survey", "Enabled"), foregroundColor: requiredColor);
            }
            if (requirements.VariablesNeededToStart.Length > 0)
            {
                WriteHost(string.Format(fmt, "Variables", $"[{string.Join(", ", requirements.VariablesNeededToStart)}]"),
                            foregroundColor: requiredColor);
            }
            {
                var diffModeVal = $"{def.DiffMode}"
                                  + (requirements.AskDiffModeOnLaunch && DiffMode != null ? $" => {DiffMode}" : "");
                WriteHost(string.Format(fmt, "Diff Mode", diffModeVal),
                                foregroundColor: requirements.AskDiffModeOnLaunch ? (DiffMode == null ? implicitColor : explicitColor) : fixedColor);
            }
            {
                var jobTypeVal = def.JobType
                                 + (requirements.AskJobTypeOnLaunch && JobType != null ? $" => {JobType}" : "");
                WriteHost(string.Format(fmt, "Job Type", jobTypeVal),
                                foregroundColor: requirements.AskJobTypeOnLaunch ? (JobType == null ? implicitColor : explicitColor) : fixedColor);
            }
            {
                var verbosityVal = $"{def.Verbosity:d} ({def.Verbosity})"
                                   + (requirements.AskVerbosityOnLaunch && Verbosity != null ? $" => {Verbosity:d} ({Verbosity})" : "");
                WriteHost(string.Format(fmt, "Verbosity", verbosityVal),
                                foregroundColor: requirements.AskVerbosityOnLaunch ? (Verbosity == null ? implicitColor : explicitColor) : fixedColor);
            }
            if (def.Credentials != null || Credentials != null)
            {
                var credentialsVal = string.Join(", ", def.Credentials?.Select(c => $"[{c.Id}] {c.Name}") ?? [])
                        + (requirements.AskCredentialOnLaunch && Credentials != null ? $" => {string.Join(',', Credentials.Select(id => $"[{id}]"))}" : "");
                WriteHost(string.Format(fmt, "Credentials", credentialsVal),
                            foregroundColor: requirements.AskCredentialOnLaunch ? (Credentials == null ? implicitColor : explicitColor) : fixedColor);
            }
            if (requirements.PasswordsNeededToStart.Length > 0)
            {
                WriteHost(string.Format(fmt, "CredentialPassswords", $"[{string.Join(", ", requirements.PasswordsNeededToStart)}]"),
                            foregroundColor: requiredColor);
            }
            if (def.ExecutionEnvironment.Id != null || ExecutionEnvironment != null)
            {
                var eeVal = $"[{def.ExecutionEnvironment.Id}] {def.ExecutionEnvironment.Name}"
                            + (requirements.AskExecutionEnvironmentOnLaunch && ExecutionEnvironment != null ? $" => [{ExecutionEnvironment}]" : "");
                WriteHost(string.Format(fmt, "ExecutionEnvironment", eeVal),
                            foregroundColor: requirements.AskExecutionEnvironmentOnLaunch ? (ExecutionEnvironment == null ? implicitColor : explicitColor) : fixedColor);
            }
            {
                var forksVal = $"{def.Forks}" + (requirements.AskForksOnLaunch && Forks != null ? $" => {Forks}" : "");
                WriteHost(string.Format(fmt, "Forks", forksVal),
                                foregroundColor: requirements.AskForksOnLaunch ? (Forks == null ? implicitColor : explicitColor) : fixedColor);
            }
            {
                var jobSliceVal = $"{def.JobSliceCount}"
                                  + (requirements.AskJobSliceCountOnLaunch && JobSliceCount != null ? $" => {JobSliceCount}" : "");
                WriteHost(string.Format(fmt, "Job Slice Count", jobSliceVal),
                                foregroundColor: requirements.AskJobSliceCountOnLaunch ? (JobSliceCount == null ? implicitColor : explicitColor) : fixedColor);
            }
            {
                var timeoutVal = $"{def.Timeout}"
                                 + (requirements.AskTimeoutOnLaunch && Timeout != null ? $" => {Timeout}" : "");
                WriteHost(string.Format(fmt, "Timeout", timeoutVal),
                                foregroundColor: requirements.AskTimeoutOnLaunch ? (Timeout == null ? implicitColor : explicitColor) : fixedColor);
            }
        }
        protected JobTemplateJob.LaunchResult? Launch(ulong id)
        {
            var requirements = GetResource<JobTemplateLaunchRequirements>($"{JobTemplate.PATH}{id}/launch/");
            if (requirements == null)
            {
                return null;
            }
            ShowJobTemplateInfo(requirements);
            var sendData = CreateSendData();
            var apiResult = CreateResource<JobTemplateJob.LaunchResult>($"{JobTemplate.PATH}{id}/launch/", sendData);
            var launchResult = apiResult.Contents;
            WriteVerbose($"Launch JobTemplate:{id} => Job:[{launchResult.Id}]");
            if (launchResult.IgnoredFields.Count > 0)
            {
                foreach (var (key, val) in launchResult.IgnoredFields)
                {
                    WriteWarning($"Ignored field: {key} ({JsonSerializer.Serialize(val, Json.DeserializeOptions)})");
                }
            }
            return launchResult;
        }
    }

    [Cmdlet(VerbsLifecycle.Invoke, "JobTemplate")]
    [OutputType(typeof(JobTemplateJob))]
    public class InvokeJobTemplateCommand : LaunchJobTemplateCommandBase
    {
        [Parameter()]
        [ValidateRange(5, int.MaxValue)]
        public int IntervalSeconds { get; set; } = 5;

        [Parameter()]
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
