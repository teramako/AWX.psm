﻿namespace AnsibleTower.Resources
{
    public class LaunchedBy(ulong? id, string type, string name, string url)
    {
        public ulong? Id { get; } = id;
        public string Type { get; } = type;
        public string Name { get; } = name;
        public string Url { get; } = url;
        public override string ToString()
        {
            return $"[{Type}]{Name}";
        }
    }
}

