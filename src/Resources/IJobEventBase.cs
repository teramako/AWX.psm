using System.Collections.Specialized;
using System.Text.Json.Serialization;

namespace AWX.Resources
{
    public interface IJobEventBase
    {
        DateTime Created { get; }
        DateTime? Modified { get; }
        JobEventEvent Event { get; }
        int Counter { get; }
        [JsonPropertyName("event_display")]
        string EventDisplay { get; }
        [JsonPropertyName("event_data")]
        Dictionary<string, object?> EventData { get; }
        bool Failed { get; }
        bool Changed { get; }
        string UUID { get; }
        string Stdout { get; }
        [JsonPropertyName("start_line")]
        int StartLine { get; }
        [JsonPropertyName("end_line")]
        int EndLine { get; }
        JobVerbosity Verbosity { get; }
    }
}
