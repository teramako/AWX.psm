using AWX.Resources;
using System.Management.Automation;

namespace AWX.Cmdlets
{
    public class MetricsItem(string name, string description, string type, Dictionary<string, string> labels, double value, string? sampleType)
    {
        public string Name { get; } = name;
        public string Description { get; } = description;
        public string Type { get; } = type;
        public Dictionary<string, string> Labels { get; } = labels;
        public double Value { get; } = value;
        public string? SampleType { get; } = sampleType;
    };

    [Cmdlet(VerbsCommon.Get, "Metrics")]
    [OutputType(typeof(MetricsItem))]
    public class GetMetricsCommand : APICmdletBase
    {
        protected override void ProcessRecord()
        {
            var metrics = GetResource<Metrics>(Metrics.PATH);
            if (metrics == null)
            {
                return;
            }
            foreach (var (key, item) in metrics)
            {
                WriteObject(CreateItem(key, item), true);
            }
        }
        private IEnumerable<MetricsItem> CreateItem(string key, Metrics.Item item)
        {
            foreach (var sample in item.Samples)
            {
                yield return new MetricsItem(key, item.HelpText, item.Type, sample.Labels, sample.Value, sample.SampleType);
            }
        }
    }
}
