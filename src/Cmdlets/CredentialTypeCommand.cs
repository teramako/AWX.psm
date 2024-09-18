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
            if (IdSet.Count == 1)
            {
                var res = GetResource<CredentialType>($"{CredentialType.PATH}{IdSet.First()}/");
                WriteObject(res);
            }
            else
            {
                Query.Add("id__in", string.Join(',', IdSet));
                Query.Add("page_size", $"{IdSet.Count}");
                foreach (var resultSet in GetResultSet<CredentialType>(CredentialType.PATH, Query, true))
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

    [Cmdlet(VerbsCommon.New, "CredentialType", SupportsShouldProcess = true)]
    [OutputType(typeof(CredentialType))]
    public class NewCredentialTypeCommand : APICmdletBase
    {
        [Parameter(Mandatory = true, Position = 0)]
        public string Name { get; set; } = string.Empty;

        [Parameter()]
        public string Description { get; set; } = string.Empty;

        [Parameter(Mandatory = true, Position = 1)]
        [ValidateSet("net", "cloud")]
        public string Kind { get; set; } = string.Empty;

        [Parameter()]
        public IDictionary Inputs { get; set; } = new Hashtable();

        [Parameter()]
        public IDictionary Injectors { get; set; } = new Hashtable();

        protected override void ProcessRecord()
        {
            var sendData = new Dictionary<string, object>()
            {
                { "name", Name },
                { "description", Description },
                { "kind", Kind },
                { "inputs", Inputs },
                { "injectors", Injectors }
            };
            var dataDescription = JsonSerializer.Serialize(sendData, Json.SerializeOptions);
            if (ShouldProcess(dataDescription))
            {
                try
                {
                    var apiResult = CreateResource<CredentialType>(CredentialType.PATH, sendData);
                    if (apiResult.Contents == null)
                        return;

                    WriteObject(apiResult.Contents, false);
                }
                catch (RestAPIException) { }
            }
        }
    }
}
