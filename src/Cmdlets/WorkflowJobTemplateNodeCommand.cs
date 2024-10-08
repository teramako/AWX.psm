using AWX.Resources;
using System.Diagnostics.CodeAnalysis;
using System.Management.Automation;

namespace AWX.Cmdlets
{
    [Cmdlet(VerbsCommon.Get, "WorkflowJobTemplateNode")]
    [OutputType(typeof(WorkflowJobTemplateNode))]
    public class GetWorkflowJobTemplateNodeCommand : GetCommandBase<WorkflowJobTemplateNode>
    {
        protected override ResourceType AcceptType => ResourceType.WorkflowJobTemplateNode;

        protected override void ProcessRecord()
        {
            GatherResourceId();
        }
        protected override void EndProcessing()
        {
            WriteObject(GetResultSet(), true);
        }
    }

    [Cmdlet(VerbsCommon.Find, "WorkflowJobTemplateNode", DefaultParameterSetName = "All")]
    [OutputType(typeof(WorkflowJobTemplateNode))]
    public class FindWorkflowJobTemplateNodeCommand : FindCommandBase
    {
        [Parameter(ParameterSetName = "AssociatedWith", ValueFromPipelineByPropertyName = true)]
        [ValidateSet(nameof(ResourceType.WorkflowJobTemplate))]
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
            var path = Id switch
            {
                > 0 => $"{WorkflowJobTemplate.PATH}{Id}/workflow_nodes/",
                _ => WorkflowJobTemplateNode.PATH
            };
            foreach (var resultSet in GetResultSet<WorkflowJobTemplateNode>(path, Query, All))
            {
                WriteObject(resultSet.Results, true);
            }
        }
    }

    [Cmdlet(VerbsCommon.Find, "WorkflowJobTemplateNodeFor")]
    [OutputType(typeof(WorkflowJobTemplateNode))]
    public class FindWorkflowJobTemplateNodeForCommand : FindCommandBase
    {
        [Parameter(ValueFromPipelineByPropertyName = true)]
        [ValidateSet(nameof(ResourceType.WorkflowJobTemplateNode))]
        public override ResourceType Type { get; set; }
        [Parameter(Mandatory = true, ValueFromPipelineByPropertyName = true)]
        public override ulong Id { get; set; }

        [Parameter(Mandatory = true, Position = 0)]
        public NodeType For { get; set; }

        [Parameter()]
        public override string[] OrderBy { get; set; } = ["!id"];

        public enum NodeType
        {
            Always, Failure, Success
        }

        protected override void BeginProcessing()
        {
            SetupCommonQuery();
        }
        protected override void ProcessRecord()
        {
            var path = For switch
            {
                NodeType.Always => $"{WorkflowJobTemplateNode.PATH}{Id}/always_nodes/",
                NodeType.Failure => $"{WorkflowJobTemplateNode.PATH}{Id}/failure_nodes/",
                NodeType.Success => $"{WorkflowJobTemplateNode.PATH}{Id}/success_nodes/",
                _ => throw new ArgumentException()
            };
            foreach (var resultSet in GetResultSet<WorkflowJobTemplateNode>(path, Query, All))
            {
                WriteObject(resultSet.Results, true);
            }
        }
    }

    [Cmdlet(VerbsCommon.New, "WorkflowJobTemplateNode", DefaultParameterSetName = "UnifiedJobTemplate", SupportsShouldProcess = true)]
    [OutputType(typeof(WorkflowJobTemplateNode))]
    public class NewWorkflowJobTemplateNodeCommand : APICmdletBase
    {
        [Parameter(Mandatory = true, ValueFromPipeline = true, Position = 0)]
        [ResourceIdTransformation(AcceptableTypes = [ResourceType.WorkflowJobTemplate])]
        public ulong WorkflowJobtemplate { get; set; }

        [Parameter(Position = 1)]
        [ResourceIdTransformation(AcceptableTypes = [ResourceType.WorkflowJobTemplateNode])]
        public ulong? ParentNode { get; set; }

        [Parameter(Position = 2)]
        [Alias("Upon")]
        [ValidateSet("success", "failure", "always")]
        public string RunUpon { get; set; } = "success";

        [Parameter(Mandatory = true, ParameterSetName = "UnifiedJobTemplate", Position = 3)]
        [Alias("Template")]
        [ResourceIdTransformation(AcceptableTypes = [
                ResourceType.Project,
                ResourceType.InventorySource,
                ResourceType.JobTemplate,
                ResourceType.SystemJobTemplate,
                ResourceType.WorkflowJobTemplate,
        ])]
        public ulong UnifiedJobTemplate { get; set; }

        [Parameter(Mandatory = true, ParameterSetName = "WorkflowApproval")]
        public string ApprovalName { get; set; } = string.Empty;

        [Parameter(ParameterSetName = "WorkflowApproval")]
        [AllowEmptyString]
        public string? Description { get; set; }

        [Parameter(ParameterSetName = "UnifiedJobTemplate")]
        [AllowEmptyString]
        [ExtraVarsArgumentTransformation]
        public string? ExtraData { get; set; }

        [Parameter(ParameterSetName = "UnifiedJobTemplate")]
        [ResourceIdTransformation(AcceptableTypes = [ResourceType.Inventory])]
        public ulong? Inventory { get; set; }

        [Parameter(ParameterSetName = "UnifiedJobTemplate")]
        [AllowEmptyString]
        public string? ScmBranch { get; set; }

        [Parameter(ParameterSetName = "UnifiedJobTemplate")]
        [AllowEmptyString]
        [ValidateSet("run", "check", "")]
        public string? JobType { get; set; }

        [Parameter(ParameterSetName = "UnifiedJobTemplate")]
        [AllowEmptyString]
        public string? Tags { get; set; }

        [Parameter(ParameterSetName = "UnifiedJobTemplate")]
        [AllowEmptyString]
        public string? SkipTags { get; set; }

        [Parameter(ParameterSetName = "UnifiedJobTemplate")]
        [AllowEmptyString]
        public string? Limit { get; set; }

        [Parameter(ParameterSetName = "UnifiedJobTemplate")]
        public SwitchParameter DiffMode { get; set; }

        [Parameter(ParameterSetName = "UnifiedJobTemplate")]
        public JobVerbosity? Verbosity { get; set; }

        [Parameter(ParameterSetName = "UnifiedJobTemplate")]
        [ResourceIdTransformation(AcceptableTypes = [ResourceType.ExecutionEnvironment])]
        public ulong? ExecutionEnvironment { get; set; }

        [Parameter(ParameterSetName = "UnifiedJobTemplate")]
        [ValidateRange(0, int.MaxValue)]
        public int? Forks { get; set; }

        [Parameter(ParameterSetName = "UnifiedJobTemplate")]
        [ValidateRange(0, int.MaxValue)]
        public int? JobSliceCount { get; set; }

        [Parameter()]
        public int? Timeout { get; set; }

        [Parameter()]
        public SwitchParameter AllParentsMustConverge { get; set; }

        [Parameter()]
        public string? Identifier { get; set; }

        private bool TryCreateNode([MaybeNullWhen(false)] out WorkflowJobTemplateNode node)
        {
            var sendData = new Dictionary<string, object>()
            {
                { "all_parents_must_converge", AllParentsMustConverge ? true : false }
            };
            if (Identifier != null)
                sendData.Add("identifier", Identifier);
            try
            {
                var apiResponse = CreateResource<WorkflowJobTemplateNode>($"{WorkflowJobTemplate.PATH}{WorkflowJobtemplate}/workflow_nodes/", sendData);
                node = apiResponse.Contents;
                return apiResponse.Response.IsSuccessStatusCode;
            }
            catch (RestAPIException)
            {
                node = null;
                return false;
            }
        }
        private bool TryCreateApprovalTemplate(WorkflowJobTemplateNode node,
                                               IDictionary<string, object> sendData,
                                               [MaybeNullWhen(false)] out WorkflowApprovalTemplate result)
        {
            try
            {
                var apiResponse = CreateResource<WorkflowApprovalTemplate>($"{WorkflowJobTemplateNode.PATH}{node.Id}/create_approval_template/", sendData);
                result = apiResponse.Contents;
                return apiResponse.Response.IsSuccessStatusCode;
            }
            catch (RestAPIException)
            {
                result = null;
                return false;
            }
        }
        private bool TryAddNode(WorkflowJobTemplateNode node, WorkflowApprovalTemplate template)
        {
            if (ParentNode == null)
                return true;
            try
            {
                var sendData = new Dictionary<string, object>()
                {
                    {"id", template.Id }
                };
                var apiResponse = CreateResource<string>($"{WorkflowJobTemplateNode.PATH}{ParentNode}/{RunUpon}_nodes/", sendData);
                return apiResponse.Response.IsSuccessStatusCode;
            }
            catch (RestAPIException)
            {
                return false;
            }
        }
        protected void CreateWorkflowApprovalNode()
        {
            var sendData = new Dictionary<string, object>()
            {
                { "name", ApprovalName }
            };
            if (Description != null)
                sendData.Add("description", Description);
            if (Timeout != null)
                sendData.Add("timeout", Timeout);

            var dataDescription = Json.Stringify(sendData, pretty: true);
            if (ShouldProcess($"WorkflowJobTemplate [{WorkflowJobtemplate}]", $"Create WorkflowApprovalTemplate {dataDescription}"))
            {
                if (TryCreateNode(out var node) &&
                    TryCreateApprovalTemplate(node, sendData, out var template) &&
                    TryAddNode(node, template))
                {
                    WriteObject(node, false);
                }
            }
        }

        protected void CreateWorkflowNode()
        {
            var path = ParentNode == null
                ? $"{WorkflowJobTemplate.PATH}{WorkflowJobtemplate}/workflow_nodes/"
                : $"{WorkflowJobTemplateNode.PATH}{ParentNode}/{RunUpon}_nodes/";
            var sendData = new Dictionary<string, object>()
            {
                { "unified_job_template", UnifiedJobTemplate }
            };
            if (ExtraData != null)
                sendData.Add("extra_data", Yaml.DeserializeToDict(ExtraData));
            if (Inventory != null)
                sendData.Add("inventory", Inventory);
            if (ScmBranch != null)
                sendData.Add("scm_branch", ScmBranch);
            if (JobType != null)
                sendData.Add("job_type", JobType);
            if (Tags != null)
                sendData.Add("job_tags", Tags);
            if (SkipTags != null)
                sendData.Add("skip_tags", SkipTags);
            if (Limit != null)
                sendData.Add("limit", Limit);
            if (DiffMode)
                sendData.Add("diff_mode", true);
            if (Verbosity != null)
                sendData.Add("verbosity", (int)Verbosity);
            if (ExecutionEnvironment != null)
                sendData.Add("execution_environment", ExecutionEnvironment);
            if (Forks != null)
                sendData.Add("forks", Forks);
            if (JobSliceCount != null)
                sendData.Add("job_slice_count", JobSliceCount);
            if (Timeout != null)
                sendData.Add("timeout", Timeout);
            if (AllParentsMustConverge)
                sendData.Add("all_parents_must_converge", true);
            if (Identifier != null)
                sendData.Add("identifier", Identifier);

            var dataDescription = Json.Stringify(sendData, pretty: true);
            if (ShouldProcess($"WorkflowJobTemplate [{WorkflowJobtemplate}]", $"Create WorkflowTemplateNode {dataDescription}"))
            {
                try
                {
                    var apiResponse = CreateResource<WorkflowJobTemplateNode>(path, sendData);
                    if (apiResponse.Response.IsSuccessStatusCode)
                    {
                        WriteObject(apiResponse.Contents, false);
                    }
                }
                catch (RestAPIException) { }
            }
        }

        protected override void ProcessRecord()
        {
            if (!string.IsNullOrEmpty(ApprovalName))
            {
                CreateWorkflowApprovalNode();
            }
            else
            {
                CreateWorkflowNode();
            }
        }
    }

    [Cmdlet(VerbsData.Update, "WorkflowJobTemplateNode", SupportsShouldProcess = true)]
    [OutputType(typeof(WorkflowJobTemplateNode))]
    public class UpdateWorkflowJobTemplateNodeCommand : APICmdletBase
    {
        [Parameter(Mandatory = true, ValueFromPipeline = true, Position = 0)]
        [ResourceIdTransformation(AcceptableTypes = [ResourceType.WorkflowJobTemplateNode])]
        public ulong Id { get; set; }

        [Parameter()]
        [Alias("Template")]
        [ResourceIdTransformation(AcceptableTypes = [
                ResourceType.Project,
                ResourceType.InventorySource,
                ResourceType.JobTemplate,
                ResourceType.SystemJobTemplate,
                ResourceType.WorkflowJobTemplate,
        ])]
        public ulong? UnifiedJobTemplate { get; set; }

        [Parameter()]
        [AllowEmptyString]
        [ExtraVarsArgumentTransformation]
        public string? ExtraData { get; set; }

        [Parameter()]
        [ResourceIdTransformation(AcceptableTypes = [ResourceType.Inventory])]
        public ulong? Inventory { get; set; }

        [Parameter()]
        [AllowEmptyString]
        public string? ScmBranch { get; set; }

        [Parameter()]
        [AllowEmptyString]
        [ValidateSet("run", "check", "")]
        public string? JobType { get; set; }

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
        [ResourceIdTransformation(AcceptableTypes = [ResourceType.ExecutionEnvironment])]
        public ulong? ExecutionEnvironment { get; set; }

        [Parameter()]
        [ValidateRange(0, int.MaxValue)]
        public int? Forks { get; set; }

        [Parameter()]
        [ValidateRange(0, int.MaxValue)]
        public int? JobSliceCount { get; set; }

        [Parameter()]
        public int? Timeout { get; set; }

        [Parameter()]
        public bool? AllParentsMustConverge { get; set; }

        [Parameter()]
        public string? Identifier { get; set; }

        private Dictionary<string, object?> CreateSendData()
        {
            var sendData = new Dictionary<string, object?>();
            if (UnifiedJobTemplate != null)
                sendData.Add("unified_job_template", UnifiedJobTemplate == 0 ? null : UnifiedJobTemplate);
            if (ExtraData != null)
                sendData.Add("extra_data", Yaml.DeserializeToDict(ExtraData));
            if (Inventory != null)
                sendData.Add("inventory", Inventory == 0 ? null : Inventory);
            if (ScmBranch != null)
                sendData.Add("scm_branch", ScmBranch);
            if (JobType != null)
                sendData.Add("job_type", JobType);
            if (Tags != null)
                sendData.Add("job_tags", Tags);
            if (SkipTags != null)
                sendData.Add("skip_tags", SkipTags);
            if (Limit != null)
                sendData.Add("limit", Limit);
            if (DiffMode != null)
                sendData.Add("diff_mode", DiffMode);
            if (Verbosity != null)
                sendData.Add("verbosity", (int)Verbosity);
            if (ExecutionEnvironment != null)
                sendData.Add("execution_environment", ExecutionEnvironment == 0 ? null : ExecutionEnvironment);
            if (Forks != null)
                sendData.Add("forks", Forks);
            if (JobSliceCount != null)
                sendData.Add("job_slice_count", JobSliceCount);
            if (Timeout != null)
                sendData.Add("timeout", Timeout);
            if (AllParentsMustConverge != null)
                sendData.Add("all_parents_must_converge", AllParentsMustConverge);
            if (Identifier != null)
                sendData.Add("identifier", Identifier);
            return sendData;
        }

        protected override void ProcessRecord()
        {
            var path = $"{WorkflowJobTemplateNode.PATH}{Id}/";
            var sendData = CreateSendData();
            if (sendData.Count == 0)
                return;

            var dataDescription = Json.Stringify(sendData, pretty: true);
            if (ShouldProcess($"WorkflowJobTemplateNode [{Id}]", $"Update {dataDescription}"))
            {
                try
                {
                    var after = PatchResource<WorkflowJobTemplateNode>($"{WorkflowJobTemplateNode.PATH}{Id}/", sendData);
                    WriteObject(after, false);
                }
                catch (RestAPIException) { }
            }
        }
    }

    [Cmdlet(VerbsLifecycle.Register, "WorkflowJobTemplateNode", SupportsShouldProcess = true)]
    [OutputType(typeof(void))]
    public class RegisterWorkflowJobTemplateNodeCommand : APICmdletBase
    {
        [Parameter(Mandatory = true, ValueFromPipeline = true, Position = 0)]
        [ResourceIdTransformation(AcceptableTypes = [ResourceType.WorkflowJobTemplateNode])]
        public ulong Id { get; set; }

        [Parameter(Mandatory = true)]
        [ResourceIdTransformation(AcceptableTypes = [ResourceType.WorkflowJobTemplateNode])]
        public ulong To { get; set; }

        [Parameter()]
        [Alias("Upon")]
        [ValidateSet("success", "failure", "always")]
        public string RunUpon { get; set; } = "success";

        protected override void ProcessRecord()
        {
            var path = $"{WorkflowJobTemplateNode.PATH}{To}/{RunUpon}_nodes/";
            var sendData = new Dictionary<string, object>()
            {
                { "id", Id }
            };
            if (ShouldProcess($"Link Node[{Id}] to Node[{To}] Upon {RunUpon}"))
            {
                try
                {
                    var apiResponse = CreateResource<string>(path, sendData);
                    if (apiResponse.Response.IsSuccessStatusCode)
                    {
                        WriteVerbose($"Node {Id} is linked to Node[{To}] upon {RunUpon}.");
                    }
                }
                catch (RestAPIException) { }
            }
        }
    }

    [Cmdlet(VerbsLifecycle.Unregister, "WorkflowJobTemplateNode", SupportsShouldProcess = true)]
    [OutputType(typeof(void))]
    public class UnregisterWorkflowJobTemplateNodeCommand : APICmdletBase
    {
        [Parameter(Mandatory = true, ValueFromPipeline = true, Position = 0)]
        [ResourceIdTransformation(AcceptableTypes = [ResourceType.WorkflowJobTemplateNode])]
        public ulong Id { get; set; }

        [Parameter(Mandatory = true)]
        [ResourceIdTransformation(AcceptableTypes = [ResourceType.WorkflowJobTemplateNode])]
        public ulong From { get; set; }

        private WorkflowJobTemplateNode? _parentNode;
        private string GetUpon()
        {
            if (_parentNode == null)
                return string.Empty;

            if (_parentNode.SuccessNodes.Any(id => id == Id))
                return "success";
            else if (_parentNode.FailureNodes.Any(id => id == Id))
                return "failure";
            else if (_parentNode.AlwaysNodes.Any(id => id == Id))
                return "always";

            return string.Empty;
        }

        protected override void BeginProcessing()
        {
            var node = GetResource<WorkflowJobTemplateNode>($"{WorkflowJobTemplateNode.PATH}{From}/");
            if (node == null)
                StopProcessing();

            _parentNode = node;
        }

        protected override void ProcessRecord()
        {
            var upon = GetUpon();
            if (string.IsNullOrEmpty(upon))
            {
                WriteVerbose($"Not found Node[{Id}] in the ParentNode [{From}]");
                return;
            }

            var path = $"{WorkflowJobTemplateNode.PATH}{From}/{upon}_nodes/";
            var sendData = new Dictionary<string, object>()
            {
                { "id", Id },
                { "disassociate", true }
            };
            if (ShouldProcess($"Link Node[{Id}] from Node[{From}] upon {upon}"))
            {
                try
                {
                    var apiResponse = CreateResource<string>(path, sendData);
                    if (apiResponse.Response.IsSuccessStatusCode)
                    {
                        WriteVerbose($"Node {Id} is unlinked from Node[{From}] upon {upon}.");
                    }
                }
                catch (RestAPIException) { }
            }
        }
    }

    [Cmdlet(VerbsCommon.Remove, "WorkflowJobTemplateNode", SupportsShouldProcess = true, ConfirmImpact = ConfirmImpact.High)]
    [OutputType(typeof(void))]
    public class RemoveWorkflowJobTemplateNodeCommand : APICmdletBase
    {
        [Parameter(Mandatory = true, ValueFromPipeline = true, Position = 0)]
        [ResourceIdTransformation(AcceptableTypes = [ResourceType.WorkflowJobTemplateNode])]
        public ulong Id { get; set; }

        [Parameter()]
        public SwitchParameter Force { get; set; }

        protected override void ProcessRecord()
        {
            if (Force || ShouldProcess($"WorkflowJobTemplateNode [{Id}]", "Delete completely"))
            {
                try
                {
                    var apiResult = DeleteResource($"{WorkflowJobTemplateNode.PATH}{Id}/");
                    if (apiResult?.IsSuccessStatusCode ?? false)
                    {
                        WriteVerbose($"WorkflowJobTemplateNode {Id} is removed.");
                    }
                }
                catch (RestAPIException) { }
            }
        }
    }
}
