using System.Text.Json.Serialization;

namespace AnsibleTower.Resources
{
    [ResourceType(ResourceType.Stdout)]
    public class JobLog(JobLog.JobLogRange range, string content)
    {
        public JobLogRange Range { get; } = range;
        public string Content { get; } = content;
        public class JobLogRange(uint start, uint end, uint absoluteEnd)
        {
            public uint Start { get; } = start;
            public uint End { get; } = end;
            [JsonPropertyName("absolute_end")]
            public uint AbsoluteEnd { get; } = absoluteEnd;
        }
    }
}
