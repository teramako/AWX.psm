using System.Text.Json.Serialization;

namespace AWX.Resources
{
    public class Metrics
        : Dictionary<string, Metrics.Item>
    {
        public const string PATH = "/api/v2/metrics/";

        public record Item(
            [property: JsonPropertyName("help_text")] string HelpText,
            string Type,
            SampleItem[] Samples
        );
        public record SampleItem(
            Dictionary<string, string> Labels,
            double Value,
            [property: JsonPropertyName("sample_type")] string? SampleType
        );
    }
}
