using AWX.Resources;
using System.Management.Automation;

namespace AWX.Cmdlets
{
    [Cmdlet(VerbsCommon.Get, "Schedule")]
    [OutputType(typeof(Schedule))]
    public class GetScheduleCommand : GetCommandBase
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
            if (IdSet.Count == 1)
            {
                var res = GetResource<Schedule>($"{Schedule.PATH}{IdSet.First()}/");
                WriteObject(res);
            }
            else
            {
                Query.Add("id__in", string.Join(',', IdSet));
                Query.Add("page_size", $"{IdSet.Count}");
                foreach (var resultSet in GetResultSet<Schedule>(Schedule.PATH, Query, true))
                {
                    WriteObject(resultSet.Results, true);
                }
            }
        }
    }
    [Cmdlet(VerbsCommon.Find, "Schedule", DefaultParameterSetName = "All")]
    [OutputType(typeof(Schedule))]
    public class FindScheduleCommand : FindCommandBase
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

    [Cmdlet(VerbsCommon.New, "Schedule", SupportsShouldProcess = true)]
    [OutputType(typeof(Schedule))]
    public class NewScheduleCommand : APICmdletBase
    {
        [Parameter(Mandatory = true)]
        public string Name { get; set; } = string.Empty;

        [Parameter()]
        [AllowEmptyString]
        public string? Description { get; set; }

        [Parameter(Mandatory = true)]
        public string RRule { get; set; } = string.Empty;

        [Parameter()]
        public SwitchParameter Disabled { get; set; }

        [Parameter(Mandatory = true)]
        [ResourceTransformation(AcceptableTypes = [
                ResourceType.Project,
                ResourceType.InventorySource,
                ResourceType.JobTemplate,
                ResourceType.SystemJobTemplate,
                ResourceType.WorkflowJobTemplate
        ])]
        public IResource Template { get; set; } = new Resource(0, 0);

        [Parameter()]
        [AllowEmptyString]
        [ExtraVarsArgumentTransformation] // Translate IDictionary to JSON string
        public string? ExtraData { get; set; }

        [Parameter()]
        [ResourceIdTransformation(AcceptableTypes = [ResourceType.Inventory])]
        public ulong? Inventory { get; set; }

        [Parameter()]
        [AllowEmptyString]
        public string? ScmBranch { get; set; }

        [Parameter()]
        [ValidateSet(nameof(Resources.JobType.Run), nameof(Resources.JobType.Check))]
        public JobType? JobType { get; set; }

        [Parameter()]
        [AllowEmptyString]
        public string? Tags { get; set; }

        [Parameter()]
        [AllowEmptyString]
        public string? SkipTags { get; set; }

        [Parameter()]
        [AllowEmptyString]
        public string? Limit { get; set; }

        [Parameter()]
        public bool? DiffMode { get; set; }

        [Parameter()]
        public JobVerbosity? Verbosity { get; set; }

        [Parameter()]
        [ValidateRange(0, int.MaxValue)]
        public int? Forks { get; set; }

        [Parameter()]
        [ResourceIdTransformation(AcceptableTypes = [ResourceType.ExecutionEnvironment])]
        public ulong? ExecutionEnvironment { get; set; }

        [Parameter()]
        [ValidateRange(0, int.MaxValue)]
        public int? JobSliceCount { get; set; }

        [Parameter()]
        public int? Timeout { get; set; }

        protected IDictionary<string, object?> CreateSendData()
        {
            var dict = new Dictionary<string, object?>()
            {
                { "name", Name },
                { "rrule", RRule }
            };
            if (Description != null)
                dict.Add("description", Description);
            if (Disabled)
                dict.Add("enabled", false);
            if (ExtraData != null)
                dict.Add("extra_data", ExtraData);
            if (Inventory != null)
                dict.Add("inventory", Inventory);
            if (ScmBranch != null)
                dict.Add("scm_branch", ScmBranch);
            if (JobType != null)
                dict.Add("job_type", $"{JobType}".ToLowerInvariant());
            if (Tags != null)
                dict.Add("job_tags", Tags);
            if (SkipTags != null)
                dict.Add("skip_tags", SkipTags);
            if (Limit != null)
                dict.Add("limit", Limit);
            if (DiffMode != null)
                dict.Add("diff_mode", DiffMode);
            if (Verbosity != null)
                dict.Add("verbosity", (int)Verbosity);
            if (Forks != null)
                dict.Add("forks", Forks);
            if (ExecutionEnvironment != null)
                dict.Add("execution_environment", ExecutionEnvironment);
            if (JobSliceCount != null)
                dict.Add("job_slice_count", JobSliceCount);
            if (Timeout != null)
                dict.Add("timeout", Timeout);

            return dict;
        }

        protected override void ProcessRecord()
        {
            var path = Template.Type switch
            {
                ResourceType.Project => $"{Project.PATH}{Template.Id}/schedules/",
                ResourceType.InventorySource => $"{InventorySource.PATH}{Template.Id}/schedules/",
                ResourceType.JobTemplate => $"{JobTemplate.PATH}{Template.Id}/schedules/",
                ResourceType.SystemJobTemplate => $"{SystemJobTemplate.PATH}{Template.Id}/schedules/",
                ResourceType.WorkflowJobTemplate => $"{WorkflowJobTemplate.PATH}{Template.Id}/schedules/",
                _ => throw new ArgumentException("Invalid type")
            };
            var sendData = CreateSendData();
            var dataDescription = Json.Stringify(sendData, pretty: true);
            if (ShouldProcess(dataDescription))
            {
                var apiResult = CreateResource<Schedule>(path, sendData);
                if (apiResult.Contents == null)
                    return;

                WriteObject(apiResult.Contents, false);
            }
        }
    }

    [Cmdlet(VerbsData.Update, "Schedule", SupportsShouldProcess = true)]
    [OutputType(typeof(Schedule))]
    public class UpdateScheduleCommand : APICmdletBase
    {
        [Parameter(Mandatory = true, ValueFromPipeline = true, Position = 0)]
        [ResourceIdTransformation(AcceptableTypes = [ResourceType.Schedule])]
        public ulong Id { get; set; }

        [Parameter()]
        public string? Name { get; set; }

        [Parameter()]
        [AllowEmptyString]
        public string? Description { get; set; }

        [Parameter()]
        public string RRule { get; set; } = string.Empty;

        [Parameter()]
        public bool? Enable { get; set; }

        [Parameter()]
        [AllowEmptyString]
        [ExtraVarsArgumentTransformation] // Translate IDictionary to JSON string
        public string? ExtraData { get; set; }

        [Parameter()]
        [ResourceIdTransformation(AcceptableTypes = [ResourceType.Inventory])]
        public ulong? Inventory { get; set; }

        [Parameter()]
        [AllowEmptyString]
        public string? ScmBranch { get; set; }

        [Parameter()]
        [ValidateSet(nameof(Resources.JobType.Run), nameof(Resources.JobType.Check))]
        public JobType? JobType { get; set; }

        [Parameter()]
        [AllowEmptyString]
        public string? Tags { get; set; }

        [Parameter()]
        [AllowEmptyString]
        public string? SkipTags { get; set; }

        [Parameter()]
        [AllowEmptyString]
        public string? Limit { get; set; }

        [Parameter()]
        public bool? DiffMode { get; set; }

        [Parameter()]
        public JobVerbosity? Verbosity { get; set; }

        [Parameter()]
        [ValidateRange(0, int.MaxValue)]
        public int? Forks { get; set; }

        [Parameter()]
        [ResourceIdTransformation(AcceptableTypes = [ResourceType.ExecutionEnvironment])]
        public ulong? ExecutionEnvironment { get; set; }

        [Parameter()]
        [ValidateRange(0, int.MaxValue)]
        public int? JobSliceCount { get; set; }

        [Parameter()]
        public int? Timeout { get; set; }

        protected IDictionary<string, object?> CreateSendData()
        {
            var dict = new Dictionary<string, object?>();
            if (!string.IsNullOrEmpty(Name))
                dict.Add("name", Name);
            if (Description != null)
                dict.Add("description", Description);
            if (!string.IsNullOrEmpty(RRule))
                dict.Add("rrule", RRule);
            if (Enable != null)
                dict.Add("enabled", Enable);
            if (ExtraData != null)
                dict.Add("extra_data", ExtraData);
            if (Inventory != null)
                dict.Add("inventory", Inventory);
            if (ScmBranch != null)
                dict.Add("scm_branch", ScmBranch);
            if (JobType != null)
                dict.Add("job_type", $"{JobType}".ToLowerInvariant());
            if (Tags != null)
                dict.Add("job_tags", Tags);
            if (SkipTags != null)
                dict.Add("skip_tags", SkipTags);
            if (Limit != null)
                dict.Add("limit", Limit);
            if (DiffMode != null)
                dict.Add("diff_mode", DiffMode);
            if (Verbosity != null)
                dict.Add("verbosity", (int)Verbosity);
            if (Forks != null)
                dict.Add("forks", Forks);
            if (ExecutionEnvironment != null)
                dict.Add("execution_environment", ExecutionEnvironment);
            if (JobSliceCount != null)
                dict.Add("job_slice_count", JobSliceCount);
            if (Timeout != null)
                dict.Add("timeout", Timeout);

            return dict;
        }

        protected override void ProcessRecord()
        {
            var sendData = CreateSendData();
            var dataDescription = Json.Stringify(sendData, pretty: true);
            if (ShouldProcess($"Schedule [{Id}]", $"Update {dataDescription}"))
            {
                try
                {
                    var after = PatchResource<Schedule>($"{Schedule.PATH}{Id}/", sendData);
                    WriteObject(after, false);
                }
                catch (RestAPIException) { }
            }
        }
    }

    [Cmdlet(VerbsCommon.Remove, "Schedule", SupportsShouldProcess = true, ConfirmImpact = ConfirmImpact.High)]
    public class RemoveScheduleCommand : APICmdletBase
    {
        [Parameter(Mandatory = true, ValueFromPipeline = true, Position = 0)]
        [ResourceIdTransformation(AcceptableTypes = [ResourceType.Schedule])]
        public ulong Id { get; set; }

        [Parameter()]
        public SwitchParameter Force { get; set; }

        protected override void ProcessRecord()
        {
            if (Force || ShouldProcess($"Schedule [{Id}]", "Delete completely"))
            {
                try
                {
                    var apiResult = DeleteResource($"{Schedule.PATH}{Id}/");
                    if (apiResult?.IsSuccessStatusCode ?? false)
                    {
                        WriteVerbose($"Schedule {Id} is deleted.");
                    }
                }
                catch (RestAPIException) { }
            }
        }
    }
}
