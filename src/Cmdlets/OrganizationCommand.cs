using System.Management.Automation;
using AWX.Resources;

namespace AWX.Cmdlets
{
    [Cmdlet(VerbsCommon.Get, "Organization")]
    [OutputType(typeof(Organization))]
    public class GetOrganizationCommand : GetCommandBase
    {
        protected override void ProcessRecord()
        {
            if (Type != null && Type != ResourceType.Organization)
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
                var res = GetResource<Organization>($"{Organization.PATH}{IdSet.First()}/");
                WriteObject(res);
            }
            else
            {
                Query.Add("id__in", string.Join(',', IdSet));
                Query.Add("page_size", $"{IdSet.Count}");
                foreach (var resultSet in GetResultSet<Organization>(Organization.PATH, Query, true))
                {
                    WriteObject(resultSet.Results, true);
                }
            }
        }
    }

    [Cmdlet(VerbsCommon.Find, "Organization", DefaultParameterSetName = "All")]
    [OutputType(typeof(Organization))]
    public class FindOrganizationCommand : FindCmdletBase
    {
        [Parameter(Mandatory = true, ParameterSetName = "AssociatedWith", ValueFromPipelineByPropertyName = true)]
        [ValidateSet(nameof(ResourceType.User))]
        public override ResourceType Type { get; set; }
        [Parameter(Mandatory = true, ParameterSetName = "AssociatedWith", ValueFromPipelineByPropertyName = true)]
        public override ulong Id { get; set; }
        [Parameter(ParameterSetName = "AssociatedWith")]
        public SwitchParameter Admin { get; set; }

        [Parameter(Position = 0)]
        public string[]? Name { get; set; }

        [Parameter()]
        public override string[] OrderBy { get; set; } = ["id"];

        protected override void BeginProcessing()
        {
            if (Name != null)
            {
                Query.Add("name__in", string.Join(",", Name));
            }
            SetupCommonQuery();
        }
        protected override void ProcessRecord()
        {
            var path = Type switch
            {
                ResourceType.User => $"{User.PATH}{Id}/" + (Admin ? "admin_of_organizations/" : "organizations/"),
                _ => Organization.PATH
            };
            foreach (var resultSet in GetResultSet<Organization>(path, Query, All))
            {
                WriteObject(resultSet.Results, true);
            }
        }
    }

    [Cmdlet(VerbsCommon.New, "Organization", SupportsShouldProcess = true)]
    [OutputType(typeof(Organization))]
    public class NewOrganizationCommand : APICmdletBase
    {
        [Parameter(Mandatory = true, Position = 0)]
        public string Name { get; set; } = string.Empty;

        [Parameter()]
        [AllowEmptyString]
        public string? Description { get; set; }

        [Parameter()]
        public uint MaxHosts { get; set; }

        [Parameter()]
        [ResourceIdTransformation(AcceptableTypes = [ResourceType.ExecutionEnvironment])]
        public ulong DefaultEnvironment { get; set; }

        protected override void ProcessRecord()
        {
            var sendData = new Dictionary<string, object>()
            {
                { "name", Name }
            };
            if (Description != null)
                sendData.Add("description", Description);
            if (MaxHosts > 0)
                sendData.Add("max_hosts", MaxHosts);
            if (DefaultEnvironment > 0)
                sendData.Add("default_environment", DefaultEnvironment);

            var dataDescription = Json.Stringify(sendData, pretty: true);
            if (ShouldProcess(dataDescription))
            {
                try
                {
                    var apiResult = CreateResource<Organization>(Organization.PATH, sendData);
                    if (apiResult.Contents == null)
                        return;

                    WriteObject(apiResult.Contents, false);
                }
                catch (RestAPIException) { }
            }
        }
    }
    
    [Cmdlet(VerbsData.Update, "Organization", SupportsShouldProcess = true)]
    [OutputType(typeof(Organization))]
    public class UpdateOrganizationCommand : APICmdletBase
    {
        [Parameter(Mandatory = true, Position = 0)]
        [ResourceIdTransformation(AcceptableTypes = [ResourceType.Organization])]
        public ulong Id { get; set; }

        [Parameter()]
        public string? Name { get; set; }

        [Parameter()]
        [AllowEmptyString]
        public string? Description { get; set; }

        [Parameter()]
        public uint? MaxHosts { get; set; }

        [Parameter()]
        [ResourceIdTransformation(AcceptableTypes = [ResourceType.ExecutionEnvironment])]
        [AllowNull]
        public ulong? DefaultEnvironment { get; set; }

        protected override void ProcessRecord()
        {
            var sendData = new Dictionary<string, object?>();
            if (!string.IsNullOrEmpty(Name))
                sendData.Add("name", Name);
            if (Description != null)
                sendData.Add("description", Description);
            if (MaxHosts != null)
                sendData.Add("max_hosts", MaxHosts);
            if (DefaultEnvironment != null)
                sendData.Add("default_environment", DefaultEnvironment == 0 ? null : DefaultEnvironment);

            if (sendData.Count == 0)
                return; // do nothing

            var dataDescription = Json.Stringify(sendData, pretty: true);
            if (ShouldProcess($"Organization [{Id}]", $"Update {dataDescription}"))
            {
                try
                {
                    var updatedOrg = PatchResource<Organization>($"{Organization.PATH}{Id}/", sendData);
                    WriteObject(updatedOrg, false);
                }
                catch (RestAPIException) { }
            }
        }
    }

    [Cmdlet(VerbsCommon.Remove, "Organization", SupportsShouldProcess = true, ConfirmImpact = ConfirmImpact.High)]
    public class RemoveOrganizationCommand : APICmdletBase
    {
        [Parameter(Mandatory = true, ValueFromPipeline = true, Position = 0)]
        [ResourceIdTransformation(AcceptableTypes = [ResourceType.Organization])]
        public ulong Id { get; set; }

        [Parameter()]
        public SwitchParameter Force { get; set; }

        private void Delete(ulong id)
        {
            if (Force || ShouldProcess($"Organization [{id}]", "Delete completely"))
            {
                try
                {
                    var apiResult = DeleteResource($"{Organization.PATH}{id}/");
                    if (apiResult?.IsSuccessStatusCode ?? false)
                    {
                        WriteVerbose($"Organization {id} is deleted.");
                    }
                }
                catch (RestAPIException) { }
            }
        }

        protected override void ProcessRecord()
        {
            Delete(Id);
        }
    }
}
