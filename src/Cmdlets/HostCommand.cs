using AWX.Resources;
using System.Management.Automation;

namespace AWX.Cmdlets
{
    [Cmdlet(VerbsCommon.Get, "Host")]
    [OutputType(typeof(Host))]
    public class GetHostCommand : GetCommandBase<Host>
    {
        protected override string ApiPath => Host.PATH;
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
    public class UpdateHostCommand : APICmdletBase
    {
        [Parameter(Mandatory = true, ValueFromPipeline = true, Position = 0)]
        [ResourceIdTransformation(AcceptableTypes = [ResourceType.Host])]
        public ulong Id { get; set; }

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

        protected override void ProcessRecord()
        {
            var sendData = new Dictionary<string, object>();
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

            if (sendData.Count == 0)
                return; // do nothing

            var dataDescription = Json.Stringify(sendData, pretty: true);
            if (ShouldProcess($"Host [{Id}]", $"Update {dataDescription}"))
            {
                try
                {
                    var updatedHost = PatchResource<Host>($"{Host.PATH}{Id}/", sendData);
                    WriteObject(updatedHost, false);
                }
                catch (RestAPIException) { }
            }
        }
    }

    [Cmdlet(VerbsCommon.Add, "Host", SupportsShouldProcess = true)]
    public class AddHostCommand : APICmdletBase
    {
        [Parameter(Mandatory = true, ValueFromPipeline = true, Position = 0)]
        [ResourceIdTransformation(AcceptableTypes = [ResourceType.Host])]
        public ulong Id { get; set; }

        [Parameter(Mandatory = true, Position = 1)]
        [ResourceIdTransformation(AcceptableTypes = [ResourceType.Group])]
        public ulong ToGroup { get; set; }

        protected override void ProcessRecord()
        {
            var path = $"{Group.PATH}{ToGroup}/hosts/";

            if (ShouldProcess($"Host [{Id}]", $"Associate to group [{ToGroup}]"))
            {
                var sendData = new Dictionary<string, object>()
                {
                    { "id",  Id },
                };
                try
                {
                    var apiResult = CreateResource<string>(path, sendData);
                    if (apiResult.Response.IsSuccessStatusCode)
                    {
                        WriteVerbose($"Host {Id} is associated to group [{ToGroup}].");
                    }
                }
                catch (RestAPIException) { }
            }
        }
    }

    [Cmdlet(VerbsCommon.Remove, "Host", SupportsShouldProcess = true, ConfirmImpact = ConfirmImpact.High)]
    public class RemoveHostCommand : APICmdletBase
    {
        [Parameter(Mandatory = true, ValueFromPipeline = true, Position = 0)]
        [ResourceIdTransformation(AcceptableTypes = [ResourceType.Host])]
        public ulong Id { get; set; }

        [Parameter()]
        [ResourceIdTransformation(AcceptableTypes = [ResourceType.Group])]
        public ulong FromGroup { get; set; }

        [Parameter()]
        public SwitchParameter Force { get; set; }

        private void Disassociate(ulong hostId, ulong groupId)
        {
            var path = $"{Group.PATH}{groupId}/hosts/";

            if (Force || ShouldProcess($"Host [{hostId}]", $"Disassociate from group [{groupId}]"))
            {
                var sendData = new Dictionary<string, object>()
                {
                    { "id",  hostId },
                    { "disassociate", true }
                };

                try
                {
                    var apiResult = CreateResource<string>(path, sendData);
                    if (apiResult.Response.IsSuccessStatusCode)
                    {
                        WriteVerbose($"Host {hostId} is disassociated from group [{groupId}].");
                    }
                }
                catch (RestAPIException) { }
            }
        }
        private void Delete(ulong id)
        {
            if (Force || ShouldProcess($"Host [{id}]", "Delete completely"))
            {
                try
                {
                    var apiResult = DeleteResource($"{Host.PATH}{id}/");
                    if (apiResult?.IsSuccessStatusCode ?? false)
                    {
                        WriteVerbose($"Host {id} is deleted.");
                    }
                }
                catch (RestAPIException) { }
            }
        }
        protected override void ProcessRecord()
        {
            if (FromGroup > 0) // disassociate
            {
                Disassociate(Id, FromGroup);
            }
            else
            {
                Delete(Id);
            }
        }
    }
}

