using AWX.Resources;
using System.Management.Automation;

namespace AWX.Cmdlets
{
    [Cmdlet(VerbsCommon.Get, "CredentialInputSource")]
    [OutputType(typeof(CredentialInputSource))]
    public class GetCredentialInputSourceCommand : GetCmdletBase
    {
        protected override void ProcessRecord()
        {
            if (Type != null && Type != ResourceType.CredentialInputSource)
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
            string path;
            if (IdSet.Count == 1)
            {
                path = $"{CredentialInputSource.PATH}{IdSet.First()}/";
                var res = GetResource<CredentialInputSource>(path);
                WriteObject(res);
            }
            else
            {
                path = CredentialInputSource.PATH;
                Query.Add("id__in", string.Join(',', IdSet));
                Query.Add("page_size", $"{IdSet.Count}");
                foreach (var resultSet in GetResultSet<CredentialInputSource>(path, Query))
                {
                    WriteObject(resultSet.Results, true);
                }
            }
        }
    }

    [Cmdlet(VerbsCommon.Find, "CredentialInputSource", DefaultParameterSetName = "All")]
    [OutputType(typeof(CredentialInputSource))]
    public class FindCredentialInputSourceCommand : FindCmdletBase
    {
        [Parameter(ParameterSetName = "AssociatedWith", ValueFromPipelineByPropertyName = true)]
        [ValidateSet(nameof(ResourceType.Credential))]
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
            var path = Id > 0 ? $"{Credential.PATH}{Id}/input_sources/" : CredentialInputSource.PATH;
            foreach (var resultSet in GetResultSet<CredentialInputSource>(path, Query, All))
            {
                WriteObject(resultSet.Results, true);
            }
        }
    }
}

