using AWX.Resources;
using System.Management.Automation;
using System.Text;
using System.Text.Json;

namespace AWX.Cmdlets
{
    [Cmdlet(VerbsCommon.Get, "WorkflowJobTemplate")]
    [OutputType(typeof(WorkflowJobTemplate))]
    public class GetWorkflowJobTemplateCommand : GetCmdletBase
    {
        protected override void ProcessRecord()
        {
            if (Type != null && Type != ResourceType.WorkflowJobTemplate)
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
                var res = GetResource<WorkflowJobTemplate>($"{WorkflowJobTemplate.PATH}{IdSet.First()}/");
                WriteObject(res);
            }
            else
            {
                Query.Add("id__in", string.Join(',', IdSet));
                Query.Add("page_size", $"{IdSet.Count}");
                foreach (var resultSet in GetResultSet<WorkflowJobTemplate>(WorkflowJobTemplate.PATH, Query, true))
                {
                    WriteObject(resultSet.Results, true);
                }
            }
        }
    }

    [Cmdlet(VerbsCommon.Find, "WorkflowJobTemplate", DefaultParameterSetName = "All")]
    [OutputType(typeof(WorkflowJobTemplate))]
    public class FindWorkflowJobTemplateCommand : FindCmdletBase
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
            var path = Id > 0 ? $"{Organization.PATH}{Id}/workflow_job_templates/" : WorkflowJobTemplate.PATH;
            foreach (var resultSet in GetResultSet<WorkflowJobTemplate>(path, Query, All))
            {
                WriteObject(resultSet.Results, true);
            }
        }
    }

    public abstract class LaunchWorkflowJobTemplateCommandBase : InvokeJobBase
    {
        [Parameter(Mandatory = true, ParameterSetName = "Id", ValueFromPipeline = true, Position = 0)]
        public ulong Id { get; set; }
        [Parameter(Mandatory = true, ParameterSetName = "JobTemplate", ValueFromPipeline = true, Position = 0)]
        public WorkflowJobTemplate? WorkflowJobTemplate { get; set; }

        [Parameter()]
        public string? Limit { get; set; }

        [Parameter()]
        public ulong? Inventory { get; set; }

        [Parameter()]
        public string? ScmBranch { get; set; }

        [Parameter()] // XXX: Should be string[] and created if not exists ?
        public ulong[]? Labels { get; set; }

        [Parameter()]
        public string[]? Tags { get; set; }

        [Parameter()]
        public string[]? SkipTags { get; set; }

        [Parameter()] // TODO: Should accept `IDctionary` (convert to JSON serialized string)
        public string? ExtraVars { get; set; }

        [Parameter()]
        public SwitchParameter Interactive { get; set; }

        private IDictionary<string, object?> CreateSendData()
        {
            var dict = new Dictionary<string, object?>();
            if (Inventory != null)
            {
                dict.Add("inventory", Inventory);
            }
            if (ScmBranch != null)
            {
                dict.Add("scm_branch", ScmBranch);
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
            return dict;
        }
        protected void GetLaunchRequirements(ulong id)
        {
            var res = base.GetResource<WorkflowJobTemplateLaunchRequirements>($"{WorkflowJobTemplate.PATH}{id}/launch/");
            if (res == null)
            {
                return;
            }
            WriteObject(res, false);
        }
        private void ShowJobTemplateInfo(WorkflowJobTemplateLaunchRequirements requirements)
        {
            var wjt = requirements.WorkflowJobTemplateData;
            var def = requirements.Defaults;
            var (fixedColor, implicitColor, explicitColor, requiredColor) =
                ((ConsoleColor?)null, ConsoleColor.Magenta, ConsoleColor.Green, ConsoleColor.Red);
            WriteHost($"[{wjt.Id}] {wjt.Name} - {wjt.Description}\n");
            var fmt = "{0,22} : {1}\n";
            if (def.Inventory.Id != null || Inventory != null)
            {
                var inventoryVal = $"[{def.Inventory.Id}] {def.Inventory.Name}"
                                   + (requirements.AskInventoryOnLaunch && Inventory != null ? $" => {Inventory}" : "");
                WriteHost(string.Format(fmt, "Inventory", inventoryVal),
                            foregroundColor: requirements.AskInventoryOnLaunch ? (Inventory == null ? implicitColor : explicitColor) : fixedColor);
            }
            if (!string.IsNullOrEmpty(def.Limit) || Limit != null)
            {
                var limitVal = def.Limit
                               + (requirements.AskLimitOnLaunch && Limit != null ? $" => {Limit}" : "");
                WriteHost(string.Format(fmt, "Limit", limitVal),
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
            if (requirements.NodePromptsRejected.Length > 0)
            {
                WriteWarning("Prompt Input will be ignored in following nodes: " +
                        $"[{string.Join(", ", requirements.NodePromptsRejected)}]");
            }
        }

        /// <summary>
        /// Show input prompt and Update <paramref name="sendData"/>.
        /// </summary>
        /// <param name="requirements"></param>
        /// <param name="sendData">Dictionary object that is the source of the JSON string sent to AWX/AnsibleTower</param>
        /// <param name="checkOptional">
        ///   <c>true</c>(`Interactive` mode)  => Check both <c>***NeededToStart</c> and <c>**AskInventoryOnLaunch</c>.
        ///   <c>false</c> => Check only <c>***NeededToStart</c>.
        /// </param>
        /// <returns>Whether the prompt is inputed(<c>true</c>) or Canceled(<c>false</c>)</returns>
        protected bool TryAskOnLaunch(WorkflowJobTemplateLaunchRequirements requirements,
                                      IDictionary<string, object?> sendData,
                                      bool checkOptional = false)
        {
            if (requirements.CanStartWithoutUserInput)
            {
                return true;
            }
            if (CommandRuntime.Host == null)
            {
                return false;
            }
            var prompt = new AskPrompt(CommandRuntime.Host);
            string key;
            string label;
            string skipFormat = "Skip {0} prompt. Already specified: {1:g}";

            // Inventory
            if (checkOptional && requirements.AskInventoryOnLaunch)
            {
                key = "inventory"; label = "Inventory";
                if (sendData.ContainsKey(key))
                {
                    WriteHost(string.Format(skipFormat, label, sendData[key]), dontshow: true);
                }
                else if (prompt.Ask<ulong>(label, "",
                                           defaultValue: requirements.Defaults.Inventory.Id,
                                           helpMessage: "Input an Inventory ID.",
                                           required: false,
                                           out var inventoryAnswer))
                {
                    if (!inventoryAnswer.IsEmpty && inventoryAnswer.Input > 0)
                    {
                        sendData[key] = inventoryAnswer.Input;
                        PrintPromptResult(label, $"{inventoryAnswer.Input}");
                    }
                    else
                    {
                        PrintPromptResult(label, $"{requirements.Defaults.Inventory}", true);
                    }
                }
                else { return false; }
            }

            // ScmBranch
            if (checkOptional && requirements.AskScmBranchOnLaunch)
            {
                key = "scm_branch"; label = "ScmBranch";
                if (sendData.ContainsKey(key))
                {
                    WriteHost(string.Format(skipFormat, label, sendData[key]), dontshow: true);
                }
                else if (prompt.Ask(label, "",
                                    defaultValue: requirements.Defaults.ScmBranch,
                                    helpMessage: "Enter the SCM branch name (or commit hash or tag)",
                                    out var branchAnswer))
                {
                    if (!branchAnswer.IsEmpty)
                    {
                        sendData[key] = branchAnswer.Input;
                        PrintPromptResult(label, $"\"{branchAnswer.Input}\"");
                    }
                    else
                    {
                        PrintPromptResult(label, $"\"{requirements.Defaults.ScmBranch}\"", true);
                    }
                }
                else { return false; }
            }

            // Labels
            if (checkOptional && requirements.AskLabelsOnLaunch)
            {
                key = "labels"; label = "Labels";
                if (sendData.ContainsKey(key))
                {
                    var strData = $"[{string.Join(", ", (ulong[]?)sendData[key] ?? [])}]";
                    WriteHost(string.Format(skipFormat, label, strData), dontshow: true);
                }
                else if (prompt.AskList<ulong>(label, "",
                                               defaultValues: requirements.Defaults.Labels?.Select(x => $"[{x.Id}] {x.Name}") ?? [],
                                               helpMessage: "Enter Label ID(s).",
                                               out var labelsAnswer))
                {
                    if (!labelsAnswer.IsEmpty)
                    {
                        var arr = labelsAnswer.Input.Where(x => x > 0).ToArray();
                        sendData[key] = arr;
                        PrintPromptResult(label, $"[{string.Join(", ", arr)}]");
                    }
                    else
                    {
                        PrintPromptResult(label,
                                    $"[{string.Join(", ", requirements.Defaults.Labels?.Select(x => $"{x}") ?? [])}]",
                                    true);
                    }
                }
                else { return false; }
            }

            // Limit
            if (checkOptional && requirements.AskLimitOnLaunch)
            {
                key = "limit"; label = "Limit";
                if (sendData.ContainsKey(key))
                {
                    WriteHost(string.Format(skipFormat, label, sendData[key]), dontshow: true);
                }
                else if (prompt.Ask(label, "",
                                    defaultValue: requirements.Defaults.Limit,
                                    helpMessage: """
                                    Enter the host pattern to further constrain the list of host managed or affected by the playbook.
                                    Multiple patterns can be separated by commas(`,`) or colons(`:`).
                                    """,
                                    out var limitAnswer))
                {
                    if (!limitAnswer.IsEmpty)
                    {
                        sendData[key] = limitAnswer.Input;
                        PrintPromptResult(label, $"\"{limitAnswer.Input}\"");
                    }
                    else
                    {
                        PrintPromptResult(label, $"\"{requirements.Defaults.Limit}\"", true);
                    }
                }
                else { return false; }
            }

            // Tags
            if (checkOptional && requirements.AskTagsOnLaunch)
            {
                key = "job_tags"; label = "Job Tags";
                if (sendData.ContainsKey(key))
                {
                    WriteHost(string.Format(skipFormat, label, sendData[key]), dontshow: true);
                }
                else if (prompt.Ask(label, "",
                                    defaultValue: requirements.Defaults.JobTags,
                                    helpMessage: """
                                    Enter the tags. Multiple values can be separated by commas(`,`).
                                    This is same as the `--tags` command-line parameter for `ansible-playbook`.
                                    """,
                                    out var jobTagsAnswer))
                {
                    if (!jobTagsAnswer.IsEmpty)
                    {
                        sendData[key] = jobTagsAnswer.Input;
                        PrintPromptResult(label, $"\"{jobTagsAnswer.Input}\"");
                    }
                    else
                    {
                        PrintPromptResult(label, $"\"{requirements.Defaults.JobTags}\"", true);
                    }
                }
                else { return false; }
            }

            // SkipTags
            if (checkOptional && requirements.AskSkipTagsOnLaunch)
            {
                key = "skip_tags"; label = "Skip Tags";
                if (sendData.ContainsKey(key))
                {
                    WriteHost(string.Format(skipFormat, label, sendData[key]), dontshow: true);
                }
                else if (prompt.Ask(label, "",
                                    defaultValue: requirements.Defaults.JobTags,
                                    helpMessage: """
                                    Enter the skip tags. Multiple values can be separated by commas(`,`).
                                    This is same as the `--skip-tags` command-line parameter for `ansible-playbook`.
                                    """,
                                    out var skipTagsAnswer))
                {
                    if (!skipTagsAnswer.IsEmpty)
                    {
                        sendData[key] = skipTagsAnswer.Input;
                        PrintPromptResult(label, $"\"{skipTagsAnswer.Input}\"");
                    }
                    else
                    {
                        PrintPromptResult(label, $"\"{requirements.Defaults.SkipTags}\"", true);
                    }
                }
                else { return false; }
            }

            // ExtraVars
            if (checkOptional && requirements.AskVariablesOnLaunch)
            {
                key = "extra_vars"; label = "Extra Variables";
                if (sendData.ContainsKey(key))
                {
                    WriteHost(string.Format(skipFormat, label, sendData[key]), dontshow: true);
                }
                else if (prompt.Ask(label, "",
                                    defaultValue: requirements.Defaults.ExtraVars,
                                    helpMessage: """
                                    Enter the extra variables provided key/value pairs using either YAML or JSON, to be passed to the playbook.
                                    This is same as the `-e` or `--extra-vars` command-line parameter for `ansible-playbook`.
                                    """,
                                    out var extraVarsAnswer))
                {
                    if (!extraVarsAnswer.IsEmpty)
                    {
                        sendData[key] = extraVarsAnswer.Input;
                        PrintPromptResult(label, extraVarsAnswer.Input);
                    }
                    else
                    {
                        PrintPromptResult(label, requirements.Defaults.ExtraVars, true);
                    }
                }
                else { return false; }
            }

            // FIXME: implement Survey

            return true;
        }

        protected WorkflowJob.LaunchResult? Launch(ulong id)
        {
            var requirements = GetResource<WorkflowJobTemplateLaunchRequirements>($"{WorkflowJobTemplate.PATH}{id}/launch/");
            if (requirements == null)
            {
                return null;
            }
            ShowJobTemplateInfo(requirements);
            if (requirements.NodeTemplatesMissing.Length > 0)
            {
                var missingNodes = string.Join(", ", requirements.NodeTemplatesMissing);
                WriteError(new ErrorRecord(new InvalidOperationException($"Missing Templates in Nodes: [{missingNodes}]"),
                                           "MissingNodeTemplates",
                                           ErrorCategory.ResourceUnavailable,
                                           requirements));
                return null;
            }
            var sendData = CreateSendData();
            if (!TryAskOnLaunch(requirements, sendData, checkOptional: Interactive))
            {
                WriteWarning("Launch canceled.");
                return null;
            }
            var apiResult = CreateResource<WorkflowJob.LaunchResult>($"{WorkflowJobTemplate.PATH}{id}/launch/", sendData);
            var launchResult = apiResult.Contents;
            WriteVerbose($"Launch WorkflowJobTemplate:{id} => Job:[{launchResult.Id}]");
            if (launchResult.IgnoredFields.Count > 0)
            {
                foreach (var (key ,val) in launchResult.IgnoredFields)
                {
                    WriteWarning($"Ignored field: {key} ({JsonSerializer.Serialize(val, Json.DeserializeOptions)})");
                }
            }
            return launchResult;
        }
    }

    [Cmdlet(VerbsLifecycle.Invoke, "WorkflowJobTemplate")]
    [OutputType(typeof(WorkflowJob))]
    public class InvokeWorkflowJobTemplateCommand : LaunchWorkflowJobTemplateCommandBase
    {
        [Parameter()]
        [ValidateRange(5, int.MaxValue)]
        public int IntervalSeconds { get; set; } = 5;

        [Parameter()]
        public SwitchParameter SuppressJobLog { get; set; }

        protected override void ProcessRecord()
        {
            if (WorkflowJobTemplate != null)
            {
                Id = WorkflowJobTemplate.Id;
            }
            try
            {
                var launchResult = Launch(Id);
                if (launchResult != null)
                {
                    JobManager.Add(launchResult);
                }
            }
            catch (RestAPIException) { }
        }
        protected override void EndProcessing()
        {
            WaitJobs("Launch WorkflowJobTemplate", IntervalSeconds, SuppressJobLog);
        }
    }

    [Cmdlet(VerbsLifecycle.Start, "WorkflowJobTemplate")]
    [OutputType(typeof(WorkflowJob.LaunchResult))]
    public class StartWorkflowJobTemplateCommand : LaunchWorkflowJobTemplateCommandBase
    {
        protected override void ProcessRecord()
        {
            if (WorkflowJobTemplate != null)
            {
                Id = WorkflowJobTemplate.Id;
            }
            try
            {
                var launchResult = Launch(Id);
                if (launchResult != null)
                {
                    WriteObject(launchResult, false);
                }
            }
            catch (RestAPIException) { }
        }
    }
}
