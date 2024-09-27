using AWX.Resources;
using System.Diagnostics.CodeAnalysis;
using System.Management.Automation;

namespace AWX.Cmdlets
{
    [Cmdlet(VerbsCommon.Get, "WorkflowJobTemplateNode")]
    [OutputType(typeof(WorkflowJobTemplateNode))]
    public class GetWorkflowJobTemplateNodeCommand : GetCmdletBase
    {
        protected override void ProcessRecord()
        {
            if (Type != null && Type != ResourceType.WorkflowJobTemplateNode)
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
                var res = GetResource<WorkflowJobTemplateNode>($"{WorkflowJobTemplateNode.PATH}{IdSet.First()}/");
                WriteObject(res);
            }
            else
            {
                Query.Add("id__in", string.Join(',', IdSet));
                Query.Add("page_size", $"{IdSet.Count}");
                foreach (var resultSet in GetResultSet<WorkflowJobTemplateNode>(WorkflowJobTemplateNode.PATH, Query, true))
                {
                    WriteObject(resultSet.Results, true);
                }
            }
        }
    }

    [Cmdlet(VerbsCommon.Find, "WorkflowJobTemplateNode", DefaultParameterSetName = "All")]
    [OutputType(typeof(WorkflowJobTemplateNode))]
    public class FindWorkflowJobTemplateNodeCommand : FindCmdletBase
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
    public class FindWorkflowJobTemplateNodeForCommand : FindCmdletBase
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
}
