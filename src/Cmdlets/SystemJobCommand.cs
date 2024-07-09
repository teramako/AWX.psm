using AWX.Resources;
using System.Management.Automation;

namespace AWX.Cmdlets
{
    [Cmdlet(VerbsCommon.Get, "SystemJob")]
    [OutputType(typeof(SystemJob.Detail))]
    public class GetSystemJobCommand : GetCmdletBase
    {
        protected override void ProcessRecord()
        {
            foreach (var id in Id)
            {
                if (!IdSet.Add(id))
                {
                    // skip already processed
                    continue;
                }
                var res = GetResource<SystemJob.Detail>($"{SystemJob.PATH}{id}/");
                if (res != null)
                {
                    WriteObject(res);
                }
            }
        }
    }

    [Cmdlet(VerbsCommon.Find, "SystemJob", DefaultParameterSetName = "All")]
    [OutputType(typeof(SystemJob))]
    public class FindSystemJobCommand : FindCmdletBase
    {
        [Parameter(Mandatory = true, ParameterSetName = "AssociatedWith", ValueFromPipelineByPropertyName = true)]
        [ValidateSet(nameof(ResourceType.SystemJobTemplate))]
        public override ResourceType Type { get; set; }
        [Parameter(Mandatory = true, ParameterSetName = "AssociatedWith", ValueFromPipelineByPropertyName = true)]
        public override ulong Id { get; set; }

        [Parameter()]
        [ValidateSet(typeof(EnumValidateSetGenerator<JobStatus>))]
        public string[]? Status { get; set; }

        [Parameter()]
        public override string[] OrderBy { get; set; } = ["!id"];


        protected override void BeginProcessing()
        {
            if (Status != null)
            {
                Query.Add("status__in", string.Join(',', Status));
            }
            SetupCommonQuery();
        }
        protected override void ProcessRecord()
        {
            var path = Type switch
            {
                ResourceType.SystemJobTemplate => $"{SystemJobTemplate.PATH}{Id}/jobs/",
                _ => SystemJob.PATH
            };
            foreach (var resultSet in GetResultSet<SystemJob>(path, Query, All))
            {
                WriteObject(resultSet.Results, true);
            }
        }
    }
}
