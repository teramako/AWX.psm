using AWX.Resources;
using System.Management.Automation;

namespace AWX.Cmdlets
{
    [Cmdlet(VerbsCommon.Get, "CredentialType")]
    [OutputType(typeof(CredentialType))]
    public class GetCredentialTypeCommand : GetCmdletBase
    {
        protected override void ProcessRecord()
        {
            if (Type != null && Type != ResourceType.CredentialType)
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
                path = $"{CredentialType.PATH}{IdSet.First()}/";
                var res = GetResource<CredentialType>(path);
                WriteObject(res);
            }
            else
            {
                path = CredentialType.PATH;
                Query.Add("id__in", string.Join(',', IdSet));
                Query.Add("page_size", $"{IdSet.Count}");
                foreach (var resultSet in GetResultSet<CredentialType>(path, Query))
                {
                    WriteObject(resultSet.Results, true);
                }
            }
        }
    }

    [Cmdlet(VerbsCommon.Find, "CredentialType")]
    [OutputType(typeof(CredentialType))]
    public class FindCredentialTypeCommand : FindCmdletBase
    {
        public override ResourceType Type { get; set; }
        public override ulong Id { get; set; }

        [Parameter()]
        public CredentialTypeKind[]? Kind { get; set; }

        [Parameter()]
        public override string[] OrderBy { get; set; } = ["id"];

        protected override void BeginProcessing()
        {
            if (Kind != null)
            {
                Query.Add("kind__in", string.Join(',', Kind));
            }
            SetupCommonQuery();
        }
        protected override void ProcessRecord()
        {
            foreach (var resultSet in GetResultSet<CredentialType>(CredentialType.PATH, Query, All))
            {
                WriteObject(resultSet.Results, true);
            }
        }
    }
}
