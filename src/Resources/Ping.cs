using System.Text.Json.Serialization;

namespace AnsibleTower.Resources
{
    public class Ping(bool ha,
                string version,
                string activeNode,
                string installUuid,
                PingInstance[] instances,
                PingInstanceGroup[] instanceGroups)
    {
        public bool HA { get; } = ha;
        public string Version { get; } = version;
        [JsonPropertyName("active_node")]
        public string ActiveNode { get; } = activeNode;
        [JsonPropertyName("install_uuid")]
        public string InstallUuid { get; } = installUuid;
        public PingInstance[] Instances { get; } = instances;
        [JsonPropertyName("instance_groups")]
        public PingInstanceGroup[] InstanceGroups { get; } = instanceGroups;
    }

    public class PingInstance(string node,
                              string nodeType,
                              string uuid,
                              string heartbeat,
                              uint capacity,
                              string version)
    {
        public string Node { get; } = node;
        [JsonPropertyName("node_type")]
        public string NodeType { get; } = nodeType;
        public string Uuid { get; } = uuid;
        public string Heartbeat { get; } = heartbeat;
        public uint Capacity { get; } = capacity;
        public string Version { get; } = version;
    }

    public class PingInstanceGroup(string name,
                                   uint capacity,
                                   string[] instances)
    {
        public string Name { get; } = name;
        public uint Capacity { get; } = capacity;
        public string[] Instances { get; } = instances;
    }
}
