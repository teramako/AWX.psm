using AWX.Resources;
using System.Management.Automation;

namespace AWX.Cmdlets
{
    [Cmdlet(VerbsCommon.Get, "Host")]
    [OutputType(typeof(Host))]
    public class GetHostCommand : GetCommandBase<Host>
    {
        protected override ResourceType AcceptType => ResourceType.Host;

        protected override void ProcessRecord()
        {
            GatherResourceId();
        }
        protected override void EndProcessing()
        {
            WriteObject(GetResultSet(), true);
        }
    }

    [Cmdlet(VerbsCommon.Find, "Host", DefaultParameterSetName = "All")]
    [OutputType(typeof(Host))]
    public class FindHostCommand : FindCommandBase
    {
        [Parameter(Mandatory = true, ParameterSetName = "AssociatedWith", ValueFromPipelineByPropertyName = true)]
        [ValidateSet(nameof(ResourceType.Inventory),
                     nameof(ResourceType.InventorySource),
                     nameof(ResourceType.Group))]
        public override ResourceType Type { get; set; }
        [Parameter(Mandatory = true, ParameterSetName = "AssociatedWith", ValueFromPipelineByPropertyName = true)]
        public override ulong Id { get; set; }

        /// <summary>
        /// List only directly member group.
        /// Only affected for a Group Type
        /// </summary>
        [Parameter(ParameterSetName = "AssociatedWith")]
        public SwitchParameter OnlyChildren { get; set; }

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
                ResourceType.Inventory => $"{Inventory.PATH}{Id}/hosts/",
                ResourceType.InventorySource => $"{InventorySource.PATH}{Id}/hosts/",
                ResourceType.Group => $"{Group.PATH}{Id}/" + (OnlyChildren ? "hosts/" : "all_hosts/"),
                _ => Host.PATH
            };
            foreach (var resultSet in GetResultSet<Host>(path, Query, All))
            {
                WriteObject(resultSet.Results, true);
            }
        }
    }

    [Cmdlet(VerbsCommon.Get, "HostFactsCache")]
    [OutputType(typeof(Dictionary<string, object?>))]
    public class GetHostFactsCacheCommand : GetCommandBase<Dictionary<string, object?>>
    {
        protected override string ApiPath => Host.PATH;
        protected override ResourceType AcceptType => ResourceType.Host;

        protected override void ProcessRecord()
        {
            WriteObject(GetResource("ansible_facts/"), true);
        }
    }

    [Cmdlet(VerbsCommon.New, "Host", SupportsShouldProcess = true)]
    [OutputType(typeof(Host))]
    public class NewHostCommand : APICmdletBase
    {
        [Parameter(Mandatory = true, Position = 0)]
        [ResourceIdTransformation(AcceptableTypes = [ResourceType.Inventory])]
        public ulong Inventory { get; set; }

        [Parameter(Mandatory = true, Position = 1)]
        public string Name { get; set; } = string.Empty;

        [Parameter()]
        [AllowEmptyString]
        public string? Description { get; set; }

        [Parameter()]
        [AllowEmptyString]
        public string? InstanceId { get; set; }

        [Parameter()]
        [AllowEmptyString]
        [ExtraVarsArgumentTransformation]
        public string? Variables { get; set; }

        [Parameter()]
        public SwitchParameter Disabled { get; set; }

        protected override void ProcessRecord()
        {
            var sendData = new Dictionary<string, object>()
            {
                { "name", Name },
                { "inventory", Inventory },
            };
            if (Description != null)
                sendData.Add("description", Description);
            if (InstanceId != null)
                sendData.Add("instance_id", InstanceId);
            if (Variables != null)
                sendData.Add("variables", Variables);
            if (Disabled)
                sendData.Add("enabled", false);

            var dataDescription = Json.Stringify(sendData, pretty: true);
            if (ShouldProcess(dataDescription))
            {
                try
                {
                    var apiResult = CreateResource<Host>(Host.PATH, sendData);
                    if (apiResult.Contents == null)
                        return;

                    WriteObject(apiResult.Contents, false);
                }
                catch (RestAPIException) { }
            }
        }
    }

    [Cmdlet(VerbsData.Update, "Host", SupportsShouldProcess = true)]
    [OutputType(typeof(Host))]
    public class UpdateHostCommand : UpdateCommandBase<Host>
    {
        [Parameter(Mandatory = true, ValueFromPipeline = true, Position = 0)]
        [ResourceIdTransformation(AcceptableTypes = [ResourceType.Host])]
        public override ulong Id { get; set; }

        [Parameter()]
        public string Name { get; set; } = string.Empty;

        [Parameter()]
        [AllowEmptyString]
        public string? Description { get; set; }

        [Parameter()]
        [AllowEmptyString]
        public bool? Enabled { get; set; } = null;

        [Parameter()]
        [AllowEmptyString]
        public string? InstanceId { get; set; }

        [Parameter()]
        [AllowEmptyString]
        [ExtraVarsArgumentTransformation]
        public string? Variables { get; set; }

        protected override Dictionary<string, object?> CreateSendData()
        {
            var sendData = new Dictionary<string, object?>();
            if (!string.IsNullOrEmpty(Name))
                sendData.Add("name", Name);
            if (Description != null)
                sendData.Add("description", Description);
            if (Enabled != null)
                sendData.Add("enabled", Enabled);
            if (InstanceId != null)
                sendData.Add("instance_id", InstanceId);
            if (Variables != null)
                sendData.Add("variables", Variables);

            return sendData;
        }

        protected override void ProcessRecord()
        {
            if (TryPatch(Id, out var result))
            {
                WriteObject(result, false);
            }
        }
    }

    [Cmdlet(VerbsLifecycle.Register, "Host", SupportsShouldProcess = true)]
    [OutputType(typeof(bool))]
    public class RegisterHostCommand : RegistrationCommandBase<Host>
    {
        [Parameter(Mandatory = true, ValueFromPipeline = true, Position = 0)]
        [ResourceIdTransformation(AcceptableTypes = [ResourceType.Host])]
        public ulong Id { get; set; }

        [Parameter(Mandatory = true, Position = 1)]
        [ResourceIdTransformation(AcceptableTypes = [ResourceType.Group])]
        public ulong To { get; set; }

        protected override void ProcessRecord()
        {
            var parentGroup = new Resource(ResourceType.Group, To);
            var path = $"{Group.PATH}{parentGroup.Id}/hosts/";
            WriteObject(Register(path, Id, parentGroup));
        }
    }

    [Cmdlet(VerbsLifecycle.Unregister, "Host", SupportsShouldProcess = true)]
    [OutputType(typeof(bool))]
    public class UnregisterHostCommand : RegistrationCommandBase<Host>
    {
        [Parameter(Mandatory = true, ValueFromPipeline = true, Position = 0)]
        [ResourceIdTransformation(AcceptableTypes = [ResourceType.Host])]
        public ulong Id { get; set; }

        [Parameter(Mandatory = true, Position = 1)]
        [ResourceIdTransformation(AcceptableTypes = [ResourceType.Group])]
        public ulong From { get; set; }

        protected override void ProcessRecord()
        {
            var parentGroup = new Resource(ResourceType.Group, From);
            var path = $"{Group.PATH}{parentGroup.Id}/hosts/";
            WriteObject(Unregister(path, Id, parentGroup));
        }
    }

    [Cmdlet(VerbsCommon.Remove, "Host", SupportsShouldProcess = true, ConfirmImpact = ConfirmImpact.High)]
    public class RemoveHostCommand : APICmdletBase
    {
        [Parameter(Mandatory = true, ValueFromPipeline = true, Position = 0)]
        [ResourceIdTransformation(AcceptableTypes = [ResourceType.Host])]
        public ulong Id { get; set; }

        [Parameter()]
        public SwitchParameter Force { get; set; }

        protected override void ProcessRecord()
        {
            if (Force || ShouldProcess($"Host [{Id}]", "Delete completely"))
            {
                try
                {
                    var apiResult = DeleteResource($"{Host.PATH}{Id}/");
                    if (apiResult?.IsSuccessStatusCode ?? false)
                    {
                        WriteVerbose($"Host {Id} is deleted.");
                    }
                }
                catch (RestAPIException) { }
            }
        }
    }
}

