using AWX.Resources;
using System.Management.Automation;

namespace AWX.Cmdlets
{
    [Cmdlet(VerbsCommon.Get, "JobHostSummary")]
    [OutputType(typeof(JobHostSummary))]
    public class GetJobHostSummaryCommand : GetCommandBase
    {
        protected override void ProcessRecord()
        {
            if (Type != null && Type != ResourceType.JobHostSummary)
            {
                return;
            }
            foreach (var id in Id)
            {
                if (!IdSet.Add(id))
                {
                    // skip already processed
                    continue;
                }
                var res = GetResource<JobHostSummary>($"{JobHostSummary.PATH}{id}/");
                if (res != null)
                {
                    WriteObject(res);
                }
            }
        }
    }

    [Cmdlet(VerbsCommon.Find, "JobHostSummary")]
    [OutputType(typeof(JobHostSummary))]
    public class FindJobHostSummaryCommand : FindCmdletBase
    {
        [Parameter(Mandatory = true, ValueFromPipelineByPropertyName = true, Position = 0)]
        [ValidateSet(nameof(ResourceType.Job),
                     nameof(ResourceType.Host),
                     nameof(ResourceType.Group))]
        public override ResourceType Type { get; set; }
        [Parameter(Mandatory = true, ValueFromPipelineByPropertyName = true, Position = 1)]
        public override ulong Id { get; set; }

        [Parameter()]
        public override string[] OrderBy { get; set; } = ["!id"];

        protected override void EndProcessing()
        {
            var path = Type switch
            {
                ResourceType.Job => $"{JobTemplateJob.PATH}{Id}/job_host_summaries/",
                ResourceType.Host => $"{Host.PATH}{Id}/job_host_summaries/",
                ResourceType.Group => $"{Group.PATH}{Id}/job_host_summaries/",
                _ => throw new ArgumentException()
            };
            foreach (var resultSet in GetResultSet<JobHostSummary>(path, Query, All))
            {
                WriteObject(resultSet.Results, true);
            }
        }
    }
}
