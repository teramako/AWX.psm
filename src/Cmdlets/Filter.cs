using System.Collections;
using System.Management.Automation;
using System.Text;

namespace AWX.Cmdlets
{
    /// <summary>
    /// Lookup Type for <see cref="Filter"/>
    /// <br/>
    /// See: <a href="https://docs.ansible.com/automation-controller/latest/html/controllerapi/filtering.html">
    /// 6. Filtering — Automation Controller API Guide</a>
    /// </summary>
    public enum FilterLookupType
    {
        /// <Summary>
        /// Exact match (default lookup if not specified
        /// </Summary>
        Exact = 0,
        /// <Summary>
        /// Case-insensitive version of Exact
        /// </Summary>
        IExact = 1,
        /// <Summary>
        /// Field contains value
        /// </Summary>
        Contains = 2,
        /// <Summary>
        /// Case-insensitive version of Contains
        /// </Summary>
        IContains = 3,
        /// <Summary>
        /// Field starts with value
        /// </Summary>
        StartsWith = 4,
        /// <Summary>
        /// Case-insensitive version of StartsWith
        /// </Summary>
        IStartsWith = 5,
        /// <Summary>
        /// Field ends with value
        /// </Summary>
        EndsWith = 6,
        /// <Summary>
        /// Case-insensitive version of EndsWith
        /// </Summary>
        IEndsWith = 7,
        /// <Summary>
        /// Field matches the given Regular Expression
        /// </Summary>
        Regex = 8,
        /// <Summary>
        /// Case-insensitive version of  Regex
        /// </Summary>
        IRegex = 9,
        /// <Summary>
        /// Greater than comparsion.
        /// </Summary>
        GreaterThan = 10,
        /// <Summary>
        /// Greater than comparsion. (Alias to <c>GreaterThan</c>)
        /// </Summary>
        GT = 10,
        /// <Summary>
        /// Greater than or equal to comparsion.
        /// </Summary>
        GreaterThanOrEqual = 11,
        /// <Summary>
        /// Greater than or equal to comparsion. (Alias to <c>GreaterThanOrEqual</c>)
        /// </Summary>
        GTE = 11,
        /// <Summary>
        /// Less than comparsion.
        /// </Summary>
        LessThan = 12,
        /// <Summary>
        /// Less than comparsion. (Alias to <c>LessThan</c>)
        /// </Summary>
        LT = 12,
        /// <Summary>
        /// Less than or equal to comparsion.
        /// </Summary>
        LessThanOrEqual = 13,
        /// <Summary>
        /// Less than or equal to comparsion. (Alias to <c>LessThanOrEqual</c>)
        /// </Summary>
        LTE = 13,
        /// <Summary>
        /// Check whether the given field or related object is null: expects a boolean value
        /// </Summary>
        IsNull = 14,
        /// <Summary>
        /// Check whether the given fields's value is present in the list provided: expects a list of items
        /// </Summary>
        In = 15,
    }

    /// <summary>
    /// Filter item.
    /// <br/>
    /// See: <a href="https://docs.ansible.com/automation-controller/latest/html/controllerapi/filtering.html">
    /// 6. Filtering — Automation Controller API Guide</a>
    /// </summary>
    public class Filter
    {
        public Filter()
        { }
        public Filter(string name, object? value, FilterLookupType type = FilterLookupType.Exact, bool or = false, bool not = false)
        {
            Or = or;
            Not = not;
            Type = type;
            Name = name;
            Value = value;
        }
        public static Filter Parse(string str)
        {
            if (string.IsNullOrWhiteSpace(str))
                return new Filter();

            var kv = str.Split('=', 2, StringSplitOptions.TrimEntries);
            if (kv.Length == 1)
                return new Filter(kv[0], null);

            return new Filter(kv[0], kv[1]);
        }
        public static Filter Parse(IDictionary dict)
        {
            var query = new Filter();
            foreach (var keyObj in dict.Keys)
            {
                var key = $"{keyObj}";
                switch (key.ToLowerInvariant())
                {
                    case "name":
                        query.Name = $"{dict[keyObj]}";
                        continue;
                    case "value":
                        query.Value = dict[keyObj];
                        continue;
                    case "type":
                        {
                            var val = $"{dict[keyObj]}";
                            if (Enum.TryParse<FilterLookupType>(val, true, out var type))
                            {
                                query.Type = type;
                            }
                            else
                            {
                                throw new ArgumentException($"Invalid Dictionay Key: \"{key}\" ({val}) is not convertable to FilterLookupType");
                            }
                            continue;
                        }
                    case "or":
                        {
                            var val = $"{dict[keyObj]}";
                            if (Boolean.TryParse(val, out bool isOr))
                            {
                                query.Or = isOr;
                            }
                            else
                            {
                                throw new ArgumentException($"Invalid Dictionay Key: \"{key}\" ({val}) is not convertable to Boolean");
                            }
                            continue;
                        }
                    case "not":
                        {
                            var val = $"{dict[keyObj]}";
                            if (Boolean.TryParse(val, out bool isNot))
                            {
                                query.Not = isNot;
                            }
                            else
                            {
                                throw new ArgumentException($"Invalid Dictionay Key: \"{key}\" ({val}) is not convertable to Boolean");
                            }
                            continue;
                        }
                }
            }
            return query;
        }
        public static string LookupTypeToString(FilterLookupType type)
        {
            switch (type)
            {
                case FilterLookupType.Exact: return "";
                case FilterLookupType.IExact: return "iexact";
                case FilterLookupType.Contains: return "contains";
                case FilterLookupType.IContains: return "icontains";
                case FilterLookupType.StartsWith: return "startswith";
                case FilterLookupType.IStartsWith: return "istartswith";
                case FilterLookupType.EndsWith: return "endswith";
                case FilterLookupType.IEndsWith: return "iendswith";
                case FilterLookupType.Regex: return "regex";
                case FilterLookupType.IRegex: return "iregex";
                case FilterLookupType.GreaterThan: return "gt";
                case FilterLookupType.GreaterThanOrEqual: return "gte";
                case FilterLookupType.LessThan: return "lt";
                case FilterLookupType.LessThanOrEqual: return "lte";
                case FilterLookupType.IsNull: return "isnull";
                case FilterLookupType.In: return "in";
                default: return "";
            }
        }

        public string Name
        {
            get { return _name; }
            set
            {
                var keyFields = value.ToLowerInvariant().Split("__");
                var keyIndex = 0;
                if (keyFields[keyIndex] == "or") { Or = true; keyIndex++; }
                if (keyFields[keyIndex] == "not") { Not = true; keyIndex++; }
                keyFields = keyFields[keyIndex..];
                if (keyFields.Length == 0)
                    throw new ArgumentException($"Invalid Filter key: {value}");
                if (keyFields.Length > 1)
                {
                    if (Enum.TryParse<FilterLookupType>(keyFields.LastOrDefault(), true, out var type))
                    {
                        Type = type;
                        keyFields = keyFields[..^1];
                    }
                }
                _name = string.Join("__", keyFields);
            }
        }
        private string _name = string.Empty;
        public object? Value
        {
            get { return _value; }
            set
            {
                if (value is PSObject pso)
                {
                    value = pso.BaseObject;
                }
                switch (value)
                {
                    case null:
                        _value = string.Empty;
                        return;
                    case string str:
                        _value = str;
                        return;
                    case bool boolean:
                        _value = boolean ? "True" : "False";
                        return;
                    case DateTime datetime:
                        if (datetime.Kind == DateTimeKind.Unspecified)
                            datetime = datetime.ToUniversalTime().ToLocalTime();
                        _value = datetime.ToString("o");
                        return;
                    case IList list:
                        var strList = new string[list.Count];
                        for (var i = 0; i < list.Count; i++)
                        {
                            strList[i] = $"{list[i]}";
                        }
                        _value = string.Join(',', strList);
                        return;
                    default:
                        _value = $"{value}";
                        return;
                }
            }
        }
        private object? _value = null;
        public FilterLookupType Type { get; set; } = FilterLookupType.Exact;
        public bool Or { get; set; } = false;
        public bool Not { get; set; } = false;

        public string GetKey()
        {
            var sb = new StringBuilder();
            if (Or) sb.Append("or__");
            if (Not) sb.Append("not__");
            sb.Append(Name);
            var lookup = LookupTypeToString(Type);
            if (!string.IsNullOrEmpty(lookup))
                sb.Append($"__{lookup}");
            return sb.ToString();
        }

        public string GetValue()
        {
            return $"{Value}";
        }

        public override string ToString()
        {
            return $"{GetKey()}={Value}";
        }
    }
}
