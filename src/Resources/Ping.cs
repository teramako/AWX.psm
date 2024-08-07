using System.Text;
using System.Text.Json.Serialization;

namespace AWX.Resources
{
    public class Ping(bool ha,
                      string version,
                      string activeNode,
                      string installUuid,
                      Ping.Instance[] instances,
                      Ping.Group[] instanceGroups)
    {
        public bool HA { get; } = ha;
        public string Version { get; } = version;
        [JsonPropertyName("active_node")]
        public string ActiveNode { get; } = activeNode;
        [JsonPropertyName("install_uuid")]
        public string InstallUuid { get; } = installUuid;
        public Instance[] Instances { get; } = instances;
        [JsonPropertyName("instance_groups")]
        public Group[] InstanceGroups { get; } = instanceGroups;

        public record Instance(string Node, [property: JsonPropertyName("node_type")] string NodeType, string Uuid,
                               string Heartbeat, uint Capacity, string Version)
        {
            public override string ToString()
            {
                StringBuilder sb = new();
                sb.Append("{ ");
                if (PrintMembers(sb))
                {
                    sb.Append(' ');
                }
                sb.Append('}');
                return sb.ToString();
            }
        };
        public record Group(string Name, uint Capacity, string[] Instances)
        {
            public override string ToString()
            {
                return $"{{ Name = {Name}, Capacity = {Capacity}, Instances: [{string.Join(',', Instances)}] }}";
            }
        };
    }
}
