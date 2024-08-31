using System.Text;
using System.Text.Json.Serialization;

namespace AWX.Resources
{
    public class JobTemplateLaunchRequirements(bool canStartWithoutUserInput, string[] passwordsNeededToStart,
                                               bool askScmBranchOnLaunch, bool askVariablesOnLaunch,
                                               bool askTagsOnLaunch, bool askDiffModeOnLaunch, bool askSkipTagsOnLaunch,
                                               bool askJobTypeOnLaunch, bool askLimitOnLaunch, bool askVerbosityOnLaunch,
                                               bool askInventoryOnLaunch, bool askCredentialOnLaunch,
                                               bool askExecutionEnvironmentOnLaunch, bool askLabelsOnLaunch,
                                               bool askForksOnLaunch, bool askJobSliceCountOnLaunch,
                                               bool askTimeoutOnLaunch, bool askInstanceGroupsOnLaunch,
                                               bool surveyEnabled, string[] variablesNeededToStart,
                                               bool credentialNeededToStart, bool inventoryNeededToStart,
                                               TemplateData jobTemplateData, JobTemplateDefaults defaults)
    {
        [JsonPropertyName("can_start_without_user_input")]
        public bool CanStartWithoutUserInput { get; } = canStartWithoutUserInput;
        [JsonPropertyName("passwords_needed_to_start")]
        public string[] PasswordsNeededToStart { get; } = passwordsNeededToStart;
        [JsonPropertyName("ask_scm_branch_on_launch")]
        public bool AskScmBranchOnLaunch { get; } = askScmBranchOnLaunch;
        [JsonPropertyName("ask_variables_on_launch")]
        public bool AskVariablesOnLaunch { get; } = askVariablesOnLaunch;
        [JsonPropertyName("ask_tags_on_launch")]
        public bool AskTagsOnLaunch { get; } = askTagsOnLaunch;
        [JsonPropertyName("ask_diff_mode_on_launch")]
        public bool AskDiffModeOnLaunch { get; } = askDiffModeOnLaunch;
        [JsonPropertyName("ask_skip_tags_on_launch")]
        public bool AskSkipTagsOnLaunch { get; } = askSkipTagsOnLaunch;
        [JsonPropertyName("ask_job_type_on_launch")]
        public bool AskJobTypeOnLaunch { get; } = askJobTypeOnLaunch;
        [JsonPropertyName("ask_limit_on_launch")]
        public bool AskLimitOnLaunch { get; } = askLimitOnLaunch;
        [JsonPropertyName("ask_verbosity_on_launch")]
        public bool AskVerbosityOnLaunch { get; } = askVerbosityOnLaunch;
        [JsonPropertyName("ask_inventory_on_launch")]
        public bool AskInventoryOnLaunch { get; } = askInventoryOnLaunch;
        [JsonPropertyName("ask_credential_on_launch")]
        public bool AskCredentialOnLaunch { get; } = askCredentialOnLaunch;
        [JsonPropertyName("ask_execution_environment_on_launch")]
        public bool AskExecutionEnvironmentOnLaunch { get; } = askExecutionEnvironmentOnLaunch;
        [JsonPropertyName("ask_labels_on_launch")]
        public bool AskLabelsOnLaunch { get; } = askLabelsOnLaunch;
        [JsonPropertyName("ask_forks_on_launch")]
        public bool AskForksOnLaunch { get; } = askForksOnLaunch;
        [JsonPropertyName("ask_job_slice_count_on_launch")]
        public bool AskJobSliceCountOnLaunch { get; } = askJobSliceCountOnLaunch;
        [JsonPropertyName("ask_timeout_on_launch")]
        public bool AskTimeoutOnLaunch { get; } = askTimeoutOnLaunch;
        [JsonPropertyName("ask_instance_groups_on_launch")]
        public bool AskInstanceGroupsOnLaunch { get; } = askInstanceGroupsOnLaunch;
        [JsonPropertyName("survey_enabled")]
        public bool SurveyEnabled { get; } = surveyEnabled;
        [JsonPropertyName("variables_needed_to_start")]
        public string[] VariablesNeededToStart { get; } = variablesNeededToStart;
        [JsonPropertyName("credential_needed_to_start")]
        public bool CredentialNeededToStart { get; } = credentialNeededToStart;
        [JsonPropertyName("inventory_needed_to_start")]
        public bool InventoryNeededToStart { get; } = inventoryNeededToStart;
        [JsonPropertyName("job_template_data")]
        public TemplateData JobTemplateData { get; } = jobTemplateData;
        public JobTemplateDefaults Defaults { get; } = defaults;
    }

    public record JobTemplateDefaults(
        NamedData Inventory,
        string Limit,
        [property: JsonPropertyName("scm_branch")] string ScmBranch,
        NamedData[]? Labels,
        [property: JsonPropertyName("job_tags")] string JobTags,
        [property: JsonPropertyName("skip_tags")] string SkipTags,
        [property: JsonPropertyName("extra_vars")] string ExtraVars,
        [property: JsonPropertyName("diff_mode")] bool DiffMode,
        [property: JsonPropertyName("job_type")] JobType JobType,
        JobVerbosity Verbosity,
        CredentialData[]? Credentials,
        [property: JsonPropertyName("execution_environment")] NamedData ExecutionEnvironment,
        int Forks,
        [property: JsonPropertyName("job_slice_count")] int JobSliceCount,
        int Timeout
    );
    public record TemplateData(ulong Id, string Name, string Description)
    {
        public override string ToString()
        {
            return $"[{Id}] {Name}";
        }
    }
    public record NamedData(ulong? Id, string? Name)
    {
        public override string ToString()
        {
            var sb = new StringBuilder();
            if (Id != null)
            {
                sb.Append($"[{Id}]");
            }
            if (!string.IsNullOrEmpty(Name))
            {
                if (sb.Length > 0) sb.Append(' ');
                sb.Append(Name);
            }
            return sb.ToString();
        }
    }
    public record CredentialData(
        ulong Id,
        string? Name,
        [property: JsonPropertyName("credential_type")] ulong? CredentialType,
        [property: JsonPropertyName("passwords_needed")] string[]? Passwordsneeded
    )
    {
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append($"[{Id}]");
            if (!string.IsNullOrEmpty(Name))
            {
                sb.Append(' ');
                sb.Append(Name);
            }
            return sb.ToString();
        }
    }
}
