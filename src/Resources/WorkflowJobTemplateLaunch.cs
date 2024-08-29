using System.Text.Json.Serialization;

namespace AWX.Resources
{
    public class WorkflowJobTemplateLaunchRequirements(bool canStartWithoutUserInput, bool askInventoryOnLaunch,
                                                       bool askLimitOnLaunch, bool askScmBranchOnLaunch,
                                                       bool askVariablesOnLaunch, bool askLabelsOnLaunch,
                                                       bool askTagsOnLaunch, bool askSkipTagsOnLaunch,
                                                       bool surveyEnabled, string[] variablesNeededToStart,
                                                       ulong[] nodeTemplatesMissing, ulong[] nodePromptsRejected,
                                                       TemplateData workflowJobTemplateData,
                                                       WorkflowJobTemplateDefaults defaults)
    {
        [JsonPropertyName("can_start_without_user_input")]
        public bool CanStartWithoutUserInput { get; } = canStartWithoutUserInput;
        [JsonPropertyName("ask_inventory_on_launch")]
        public bool AskInventoryOnLaunch { get; } = askInventoryOnLaunch;
        [JsonPropertyName("ask_limit_on_launch")]
        public bool AskLimitOnLaunch { get; } = askLimitOnLaunch;
        [JsonPropertyName("ask_scm_branch_on_launch")]
        public bool AskScmBranchOnLaunch { get; } = askScmBranchOnLaunch;
        [JsonPropertyName("ask_variables_on_launch")]
        public bool AskVariablesOnLaunch { get; } = askVariablesOnLaunch;
        [JsonPropertyName("ask_labels_on_launch")]
        public bool AskLabelsOnLaunch { get; } = askLabelsOnLaunch;
        [JsonPropertyName("ask_tags_on_launch")]
        public bool AskTagsOnLaunch { get; } = askTagsOnLaunch;
        [JsonPropertyName("ask_skip_tags_on_launch")]
        public bool AskSkipTagsOnLaunch { get; } = askSkipTagsOnLaunch;
        [JsonPropertyName("survey_enabled")]
        public bool SurveyEnabled { get; } = surveyEnabled;
        [JsonPropertyName("variables_needed_to_start")]
        public string[] VariablesNeededToStart { get; } = variablesNeededToStart;

        [JsonPropertyName("node_templates_missing")]
        public ulong[] NodeTemplatesMissing { get; } = nodeTemplatesMissing;
        [JsonPropertyName("node_prompts_rejected")]
        public ulong[] NodePromptsRejected { get; } = nodePromptsRejected;

        [JsonPropertyName("workflow_job_template_data")]
        public TemplateData WorkflowJobTemplateData { get; } = workflowJobTemplateData;
        public WorkflowJobTemplateDefaults Defaults { get; } = defaults;
    }

    public record WorkflowJobTemplateDefaults(
        NamedData Inventory,
        string Limit,
        [property: JsonPropertyName("scm_branch")] string ScmBranch,
        NamedData[]? Labels,
        [property: JsonPropertyName("job_tags")] string JobTags,
        [property: JsonPropertyName("skip_tags")] string SkipTags,
        [property: JsonPropertyName("extra_vars")] string ExtraVars
    );
}
