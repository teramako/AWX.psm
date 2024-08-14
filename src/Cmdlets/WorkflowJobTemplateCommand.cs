using AWX.Resources;
using System.Collections;
using System.Management.Automation;
using System.Text;

namespace AWX.Cmdlets
{
    [Cmdlet(VerbsCommon.Get, "WorkflowJobTemplate")]
    [OutputType(typeof(WorkflowJobTemplate))]
    public class GetWorkflowJobTemplateCommand : GetCmdletBase
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
            foreach (var resultSet in GetResultSet<WorkflowJobTemplate>($"{WorkflowJobTemplate.PATH}?{Query}", true))
            {
                WriteObject(resultSet.Results, true);
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

        private Hashtable CreateSendData()
        {
            var dict = new Hashtable();
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
            var (fixedColor, implicitColor, explicitColor) = ((ConsoleColor?)null, ConsoleColor.Magenta, ConsoleColor.Green);
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
            if (!string.IsNullOrEmpty(def.JobTags))
            {
                WriteHost(string.Format(fmt, "Job tags", def.JobTags),
                            foregroundColor: requirements.AskTagsOnLaunch ? implicitColor : explicitColor);
            }
            if (!string.IsNullOrEmpty(def.SkipTags))
            {
                WriteHost(string.Format(fmt, "Skip tags", def.SkipTags),
                            foregroundColor: requirements.AskSkipTagsOnLaunch ? implicitColor : explicitColor);
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
                            foregroundColor: requirements.AskVariablesOnLaunch ? implicitColor : explicitColor);
            }
        }
        protected WorkflowJob.LaunchResult? Launch(ulong id)
        {
            var requirements = GetResource<WorkflowJobTemplateLaunchRequirements>($"{WorkflowJobTemplate.PATH}{id}/launch/");
            if (requirements == null)
            {
                return null;
            }
            ShowJobTemplateInfo(requirements);
            var apiResult = CreateResource<WorkflowJob.LaunchResult>($"{WorkflowJobTemplate.PATH}{id}/launch/", CreateSendData());
            var launchResult = apiResult.Contents;
            WriteVerbose($"Launch WorkflowJobTemplate:{id} => Job:[{launchResult.Id}]");
            if (launchResult.IgnoredFields.Count > 0)
            {
                foreach (var (key ,val) in launchResult.IgnoredFields)
                {
                    WriteWarning($"Ignored field: {key} ({val})");
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
