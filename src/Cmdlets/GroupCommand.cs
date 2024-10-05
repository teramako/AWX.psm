using AWX.Resources;
using System.Management.Automation;

namespace AWX.Cmdlets
{
    [Cmdlet(VerbsCommon.Get, "Group")]
    [OutputType(typeof(Group))]
    public class GetGroupCommand : GetCmdletBase
    {
        protected override void ProcessRecord()
        {
            if (Type != null && Type != ResourceType.Group)
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
                var res = GetResource<Group>($"{Group.PATH}{IdSet.First()}/");
                WriteObject(res);
            }
            else
            {
                Query.Add("id__in", string.Join(',', IdSet));
                Query.Add("page_size", $"{IdSet.Count}");
                foreach (var resultSet in GetResultSet<Group>(Group.PATH, Query, true))
                {
                    WriteObject(resultSet.Results, true);
                }
            }
        }
    }

    [Cmdlet(VerbsCommon.Find, "Group", DefaultParameterSetName = "All")]
    [OutputType(typeof(Group))]
    public class FindGroupCommand : FindCmdletBase
    {
        [Parameter(Mandatory = true, ParameterSetName = "AssociatedWith", ValueFromPipelineByPropertyName = true)]
        [ValidateSet(nameof(ResourceType.Inventory),
                     nameof(ResourceType.Group),
                     nameof(ResourceType.InventorySource),
                     nameof(ResourceType.Host))]
        public override ResourceType Type { get; set; }
        [Parameter(Mandatory = true, ParameterSetName = "AssociatedWith", ValueFromPipelineByPropertyName = true)]
        public override ulong Id { get; set; }

        /// <summary>
        /// List only root(Top-level) groups.
        /// Only affected for an Inventory Type
        /// </summary>
        [Parameter(ParameterSetName = "AssociatedWith")]
        public SwitchParameter OnlyRoot { get; set; }

        /// <summary>
        /// List only directly member groups.
        /// Only affected for a Host Type
        /// </summary>
        [Parameter(ParameterSetName = "AssociatedWith")]
        public SwitchParameter OnlyParnets { get; set; }

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
                ResourceType.Inventory => $"{Inventory.PATH}{Id}/" + (OnlyRoot ? "root_groups/" : "groups/"),
                ResourceType.Group => $"{Group.PATH}{Id}/children/",
                ResourceType.InventorySource => $"{InventorySource.PATH}{Id}/groups/",
                ResourceType.Host => $"{Host.PATH}{Id}/" + (OnlyParnets ? "groups/" : "all_groups/"),
                _ => Group.PATH
            };
            foreach (var resultSet in GetResultSet<Group>(path, Query, All))
            {
                WriteObject(resultSet.Results, true);
            }
        }
    }

    [Cmdlet(VerbsCommon.New, "Group", SupportsShouldProcess = true)]
    [OutputType(typeof(Group))]
    public class NewGroupCommand : APICmdletBase
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
        [ExtraVarsArgumentTransformation]
        public string? Variables { get; set; }

        protected override void ProcessRecord()
        {
            var sendData = new Dictionary<string, object>()
            {
                { "name", Name },
                { "inventory", Inventory },
            };
            if (Description != null)
                sendData.Add("description", Description);
            if (Variables != null)
                sendData.Add("variables", Variables);

            var dataDescription = Json.Stringify(sendData, pretty: true);
            if (ShouldProcess(dataDescription))
            {
                try
                {
                    var apiResult = CreateResource<Group>(Group.PATH, sendData);
                    if (apiResult.Contents == null)
                        return;

                    WriteObject(apiResult.Contents, false);
                }
                catch (RestAPIException) { }
            }
        }
    }

    [Cmdlet(VerbsData.Update, "Group", SupportsShouldProcess = true)]
    [OutputType(typeof(Group))]
    public class UpdateGroupCommand : APICmdletBase
    {
        [Parameter(Mandatory = true, ValueFromPipeline = true, Position = 0)]
        [ResourceIdTransformation(AcceptableTypes = [ResourceType.Group])]
        public ulong Id { get; set; }

        [Parameter()]
        public string Name { get; set; } = string.Empty;

        [Parameter()]
        [AllowEmptyString]
        public string? Description { get; set; }

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
            if (Variables != null)
                sendData.Add("variables", Variables);

            if (sendData.Count == 0)
                return; // do nothing

            var dataDescription = Json.Stringify(sendData, pretty: true);
            if (ShouldProcess($"Group [{Id}]", $"Update {dataDescription}"))
            {
                try
                {
                    var updatedGroup = PatchResource<Group>($"{Group.PATH}{Id}/", sendData);
                    WriteObject(updatedGroup, false);
                }
                catch (RestAPIException) { }
            }
        }
    }

    [Cmdlet(VerbsCommon.Add, "Group", SupportsShouldProcess = true)]
    public class AddGroupCommand : APICmdletBase
    {
        [Parameter(Mandatory = true, ValueFromPipeline = true, Position = 0)]
        [ResourceIdTransformation(AcceptableTypes = [ResourceType.Group])]
        public ulong Id { get; set; }

        [Parameter(Mandatory = true, Position = 1)]
        [ResourceIdTransformation(AcceptableTypes = [ResourceType.Group])]
        public ulong ToGroup { get; set; }

        protected override void ProcessRecord()
        {
            var path = $"{Group.PATH}{ToGroup}/children/";

            if (ShouldProcess($"Group [{Id}]", $"Associate to group [{ToGroup}]"))
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
                        WriteVerbose($"Group {Id} is associated to group [{ToGroup}].");
                    }
                }
                catch (RestAPIException) { }
            }
        }
    }

    [Cmdlet(VerbsCommon.Remove, "Group", SupportsShouldProcess = true, ConfirmImpact = ConfirmImpact.High)]
    public class RemoveGroupCommand : APICmdletBase
    {
        [Parameter(Mandatory = true, ValueFromPipeline = true, Position = 0)]
        [ResourceIdTransformation(AcceptableTypes = [ResourceType.Group])]
        public ulong Id { get; set; }

        [Parameter()]
        [ResourceIdTransformation(AcceptableTypes = [ResourceType.Group])]
        public ulong FromGroup { get; set; }

        [Parameter()]
        public SwitchParameter Force { get; set; }

        private void Disassociate(ulong childGroupId, ulong parentGroupId)
        {
            var path = $"{Group.PATH}{parentGroupId}/children/";

            if (Force || ShouldProcess($"Group [{childGroupId}]", $"Disassociate from group [{parentGroupId}]"))
            {
                var sendData = new Dictionary<string, object>()
                {
                    { "id",  childGroupId },
                    { "disassociate", true }
                };

                try
                {
                    var apiResult = CreateResource<string>(path, sendData);
                    if (apiResult.Response.IsSuccessStatusCode)
                    {
                        WriteVerbose($"Group {childGroupId} is disassociated from group [{parentGroupId}].");
                    }
                }
                catch (RestAPIException) { }
            }
        }
        private void Delete(ulong id)
        {
            if (Force || ShouldProcess($"Group [{id}]", "Delete completely"))
            {
                try
                {
                    var apiResult = DeleteResource($"{Group.PATH}{id}/");
                    if (apiResult?.IsSuccessStatusCode ?? false)
                    {
                        WriteVerbose($"Group {id} is deleted.");
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

