using AWX.Resources;
using System.Management.Automation;

namespace AWX.Cmdlets
{
    [Cmdlet(VerbsCommon.Get, "ExecutionEnvironment")]
    [OutputType(typeof(ExecutionEnvironment))]
    public class GetExecutionEnvironmentCommand : GetCmdletBase
    {
        protected override void ProcessRecord()
        {
            if (Type != null && Type != ResourceType.ExecutionEnvironment)
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
                var res = GetResource<ExecutionEnvironment>($"{ExecutionEnvironment.PATH}{IdSet.First()}/");
                WriteObject(res);
            }
            else
            {
                Query.Add("id__in", string.Join(',', IdSet));
                Query.Add("page_size", $"{IdSet.Count}");
                foreach (var resultSet in GetResultSet<ExecutionEnvironment>(ExecutionEnvironment.PATH, Query, true))
                {
                    WriteObject(resultSet.Results, true);
                }
            }
        }
    }

    [Cmdlet(VerbsCommon.Find, "ExecutionEnvironment", DefaultParameterSetName = "All")]
    [OutputType(typeof(ExecutionEnvironment))]
    public class FindExecutionEnvironmentCommand : FindCmdletBase
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
            var path = Id > 0 ? $"{Organization.PATH}{Id}/execution_environments/" : ExecutionEnvironment.PATH;
            foreach (var resultSet in GetResultSet<ExecutionEnvironment>(path, Query, All))
            {
                WriteObject(resultSet.Results, true);
            }
        }
    }

    [Cmdlet(VerbsCommon.New, "ExecutionEnvironment", SupportsShouldProcess = true)]
    [OutputType(typeof(ExecutionEnvironment))]
    public class AddExecutionEnvironmentCommand : APICmdletBase
    {
        [Parameter(Mandatory = true)]
        public string Name { get; set; } = string.Empty;

        [Parameter()]
        [AllowEmptyString]
        public string Description { get; set; } = string.Empty;

        [Parameter(Mandatory = true)]
        public string Image { get; set; } = string.Empty;

        [Parameter()]
        [ResourceIdTransformation(AcceptableTypes = [ResourceType.Organization])]
        public ulong? Organization { get; set; }

        [Parameter()]
        [ResourceIdTransformation(AcceptableTypes = [ResourceType.Credential])]
        public ulong? Credential { get; set; }

        [Parameter()]
        [AllowEmptyString]
        [ValidateSet("", "always", "missing", "never")]
        public string? Pull { get; set; } = string.Empty;

        protected override void ProcessRecord()
        {
            var sendData = new Dictionary<string, object>()
            {
                { "name", Name },
                { "description", Description },
                { "image", Image },
            };
            if (Organization != null)
                sendData.Add("organization", Organization);
            if (Credential != null)
                sendData.Add("credential", Credential);
            if (Pull != null)
                sendData.Add("pull", Pull);

            var dataDescription = Json.Stringify(sendData, pretty: true);
            if (ShouldProcess(dataDescription))
            {
                try
                {
                    var apiResult = CreateResource<ExecutionEnvironment>(ExecutionEnvironment.PATH, sendData);
                    if (apiResult.Contents == null)
                        return;

                    WriteObject(apiResult.Contents, false);
                }
                catch (RestAPIException) { }
            }
        }
    }

    [Cmdlet(VerbsData.Update, "ExecutionEnvironment", SupportsShouldProcess = true)]
    [OutputType(typeof(ExecutionEnvironment))]
    public class UpdateExecutionEnvironmentCommand : APICmdletBase
    {
        [Parameter(Mandatory = true, ValueFromPipeline = true, Position = 0)]
        [ResourceIdTransformation(AcceptableTypes = [ResourceType.ExecutionEnvironment])]
        public ulong Id { get; set; }

        [Parameter()]
        public string? Name { get; set; }

        [Parameter()]
        [AllowEmptyString]
        public string? Description { get; set; }

        [Parameter()]
        public string? Image { get; set; }

        [Parameter()]
        [ResourceIdTransformation(AcceptableTypes = [ResourceType.Organization])]
        public ulong? Organization { get; set; }

        [Parameter()]
        [ResourceIdTransformation(AcceptableTypes = [ResourceType.Credential])]
        public ulong? Credential { get; set; }

        [Parameter()]
        [AllowEmptyString]
        [ValidateSet("", "always", "missing", "never")]
        public string? Pull { get; set; }

        protected override void ProcessRecord()
        {
            var sendData = new Dictionary<string, object?>();
            if (!string.IsNullOrEmpty(Name))
                sendData.Add("name", Name);
            if (Description != null)
                sendData.Add("description", Description);
            if (Image != null)
                sendData.Add("image", Image);
            if (Organization != null)
                sendData.Add("organization", Organization == 0 ? null : Organization);
            if (Credential != null)
                sendData.Add("credential", Credential == 0 ? null : Credential);
            if (Pull != null)
                sendData.Add("pull", Pull);

            if (sendData.Count == 0)
                return;

            var dataDescription = Json.Stringify(sendData, pretty: true);
            if (ShouldProcess($"ExecutionEnvironment [{Id}]", $"Update {dataDescription}"))
            {
                try
                {
                    var after = PatchResource<ExecutionEnvironment>($"{ExecutionEnvironment.PATH}{Id}/", sendData);
                    WriteObject(after, false);
                }
                catch (RestAPIException) { }
            }
        }
    }
}
