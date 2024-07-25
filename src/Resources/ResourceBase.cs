using System.Text.Json.Serialization;

namespace AWX.Resources
{
    public interface IResource<TSummary>
    {
        /// <summary>
        /// Database ID for the resource
        /// </summary>
        ulong Id { get; }
        /// <summary>
        /// Data type for the resource
        /// </summary>
        ResourceType Type { get; }
        string Url { get; }
        /// <summary>
        /// Data structure with URLs of related resources.
        /// </summary>
        RelatedDictionary Related { get; }
        /// <summary>
        /// Data structure with name/description for related resources.
        /// The output for some objects may be limited for performance reasons.
        /// </summary>
        [JsonPropertyName("summary_fields")]
        TSummary SummaryFields { get; }
    }
}
