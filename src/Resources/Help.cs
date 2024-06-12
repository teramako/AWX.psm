using System.Collections.Specialized;
using System.Text.Json.Serialization;

namespace AnsibleTower.Resources
{
    public class ApiHelp(string name,
                         string description,
                         string[] renders,
                         string[] parses,
                         OrderedDictionary? actions,
                         string[]? types,
                         string[]? searchFields,
                         string[]? relatedSearchFields,
                         string[]? objectRoles,
                         uint? maxPageSize)
    {
        public string Name { get; } = name ?? string.Empty;
        public string Description { get; } = description ?? string.Empty;
        public string[] Renders { get; } = renders;
        public string[] Parses { get; } = parses;
        public OrderedDictionary? Actions { get; } = actions;
        public string[]? Types { get; } = types;

        public string[]? SearchFields { get; } = searchFields;
        public string[]? RelatedSearchFields { get; } = relatedSearchFields;
        public string[]? ObjectRoles { get; } = objectRoles;
        public uint? MaxPageSize { get; } = maxPageSize;
    }
}
