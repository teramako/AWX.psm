using AWX.Resources;
using System.Management.Automation;

namespace AWX.Cmdlets
{
    [Cmdlet(VerbsCommon.Get, "HostMetrics")]
    [OutputType(typeof(HostMetrics))]
    public class GetHostMetricsCommand : GetCmdletBase
    {
        protected override void ProcessRecord()
        {
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
                path = $"{HostMetrics.PATH}{IdSet.First()}/";
                var res = GetResource<HostMetrics>(path);
                WriteObject(res);
            }
            else
            {
                path = HostMetrics.PATH;
                Query.Add("id__in", string.Join(',', IdSet));
                Query.Add("page_size", $"{IdSet.Count}");
                foreach (var resultSet in GetResultSet<HostMetrics>(path, Query))
                {
                    WriteObject(resultSet.Results, true);
                }
            }
        }
    }

    [Cmdlet(VerbsCommon.Find, "HostMetrics", DefaultParameterSetName = "All")]
    [OutputType(typeof(HostMetrics))]
    public class FindHostMetricsCommand : FindCmdletBase
    {
        public override ResourceType Type { get; set; }
        public override ulong Id { get; set; }

        [Parameter()]
        public override string[] OrderBy { get; set; } = ["id"];

        protected override void BeginProcessing()
        {
            SetupCommonQuery();
        }
        protected override void ProcessRecord()
        {
            foreach (var resultSet in GetResultSet<HostMetrics>(HostMetrics.PATH, Query, All))
            {
                WriteObject(resultSet.Results, true);
            }
        }
    }
}
