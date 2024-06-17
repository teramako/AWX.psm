using System.Text.Json.Serialization;

namespace AWX.Resources
{
    public interface IJobDetail
    {
        [JsonPropertyName("job_args")]
        string JobArgs { get; }
        [JsonPropertyName("job_cwd")]
        string JobCwd { get; }
        [JsonPropertyName("job_env")]
        Dictionary<string, string> JobEnv { get; }
        [JsonPropertyName("result_traceback")]
        string ResultTraceback { get; }
        [JsonPropertyName("event_processing_finished")]
        bool EventProcessingFinished { get; }
    }
}

