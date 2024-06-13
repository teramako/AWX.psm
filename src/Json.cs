using AnsibleTower.Resources;
using System.Collections;
using System.Collections.Specialized;
using System.Reflection;
using System.Runtime;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Unicode;

namespace AnsibleTower
{
    public class Json
    {
        public static OrderedDictionary ToDict(JsonElement json)
        {
            var dict = new OrderedDictionary();
            foreach (var kv in json.EnumerateObject())
            {
                dict.Add(kv.Name, ObjectToInferredType(kv.Value));
            }
            return dict;
        }
        public static object?[] ToArray(JsonElement json)
        {
            var length = json.GetArrayLength();
            object?[] array = new object?[length];
            for (var i = 0; i < length; i++)
            {
                array[i] = ObjectToInferredType(json[i]);
            }
            return array;
        }
        public static object? ObjectToInferredType(JsonElement val, bool isRoot = false)
        {
            switch (val.ValueKind)
            {
                case JsonValueKind.Object:
                    if (isRoot)
                    {
                        if (val.TryGetProperty("type", out var typeValue))
                        {
                            var key = ToUpperCamelCase(typeValue.GetString() ?? string.Empty);
                            var fieldInfo = typeof(ResourceType).GetField(key);
                            if (fieldInfo != null)
                            {
                                var attr = fieldInfo.GetCustomAttributes<ResourcePathAttribute>(false).First();
                                if (attr != null && attr.Type != null)
                                {
                                    var obj = val.Deserialize(attr.Type, DeserializeOptions);
                                    if (obj != null)
                                    {
                                        return obj;
                                    }
                                }
                            }
                        }
                        var isResultSet = true;
                        foreach (var kv in val.EnumerateObject())
                        {
                            switch (kv.Name)
                            {
                                case "count":
                                case "next":
                                case "previous":
                                    continue;
                                case "results":
                                    if (kv.Value.ValueKind == JsonValueKind.Array)
                                    {
                                        continue;
                                    }
                                    else
                                    {
                                        isResultSet = false;
                                    }
                                    break;
                                default:
                                    isResultSet = false;
                                    break;
                            }
                        }
                        if (isResultSet)
                        {
                            return val.Deserialize<ResultSet>(DeserializeOptions);
                        }
                    }
                    return ToDict(val);
                case JsonValueKind.Array:
                    return ToArray(val);
                case JsonValueKind.String:
                    return val.GetString() ?? string.Empty;
                case JsonValueKind.Number:
                    if (val.GetRawText().IndexOf('.') > 0)
                    {
                        return val.GetDouble();
                    }
                    else
                    {
                        return val.GetInt64();
                    }
                case JsonValueKind.True:
                case JsonValueKind.False:
                    return val.GetBoolean();
                case JsonValueKind.Null:
                default:
                    return null;
                    // throw new ArgumentNullException(nameof(val));
            }
        }
        static string ToUpperCamelCase(string value)
        {
            if (value.Length < 2)
            {
                return value.ToUpperInvariant();
            }
            var sb = new StringBuilder();
            sb.Append(char.ToUpperInvariant(value[0]));
            for (var i = 1; i < value.Length; i++)
            {
                char c = value[i];
                switch (c)
                {
                    case ' ':
                    case '_':
                    case '-':
                        if (i < value.Length - 1)
                        {
                            sb.Append(char.ToUpperInvariant(value[++i]));
                        }
                        else
                        {
                            sb.Append(c);
                        }
                        break;
                    default:
                        sb.Append(char.ToLowerInvariant(c)); break;
                }
            }
            return sb.ToString();

        }
        static string ToSnakeCase(string value)
        {
            if (value.Length < 2)
            {
                return value.ToLowerInvariant();
            }
            var sb = new StringBuilder();
            sb.Append(char.ToLowerInvariant(value[0]));
            for (var i = 1; i < value.Length; i++)
            {
                char c = value[i];
                if (char.IsUpper(c))
                {
                    sb.Append('_');
                    sb.Append(char.ToLowerInvariant(c));
                }
                else
                {
                    sb.Append(c);
                }
            }
            return sb.ToString();
        }
        /// <summary>
        /// "related" property converter.<br/>
        /// See <see cref="RelatedDictionary"/>
        /// </summary>
        public class RelatedResourceConverter : JsonConverter<RelatedDictionary>
        {
            public override RelatedDictionary? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            {
                var dict = new RelatedDictionary();
                while (reader.Read())
                {
                    if (reader.TokenType == JsonTokenType.EndObject)
                    {
                        return dict;
                    }
                    if (reader.TokenType != JsonTokenType.PropertyName)
                    {
                        throw new JsonException($"TokenType is not PropertyName: {reader.TokenType}");
                    }
                    string? propertyName = reader.GetString() ?? throw new JsonException("PropertyName is null");

                    reader.Read();
                    switch (reader.TokenType)
                    {
                        case JsonTokenType.String:
                            var val = reader.GetString();
                            if (val != null)
                                dict.Add(propertyName, val);
                            break;
                        case JsonTokenType.StartArray:
                            var list = new List<string>();
                            reader.Read();
                            while (reader.TokenType != JsonTokenType.EndArray)
                            {
                                if (reader.TokenType != JsonTokenType.String)
                                {
                                    throw new JsonException($"[InArray] value is not String: {reader.TokenType}");
                                }
                                string? arrayVal = reader.GetString();
                                if (!string.IsNullOrEmpty(arrayVal))
                                    list.Add(arrayVal);

                                reader.Read();
                            }
                            dict.Add(propertyName, list.ToArray());
                            break;
                        default:
                            throw new JsonException($"TokenType is not String or StartArray: {reader.TokenType}");
                    }

                }
                throw new JsonException();
            }

            public override void Write(Utf8JsonWriter writer, RelatedDictionary value, JsonSerializerOptions options)
            {
                JsonSerializer.Serialize(writer, value, options);
            }
        }
        /// <summary>
        /// Enum Converter
        /// <list type="bullet">Serialize: to string of "snake_case"</list>
        /// <list type="bullet">Deserialize: to Enum named "UpperCamelCase"</list>
        /// </summary>
        /// <typeparam name="TEnum"></typeparam>
        public class EnumUpperCamelCaseStringConverter<TEnum> : JsonConverter<TEnum> where TEnum : struct, Enum
        {
            public override TEnum Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            {
                var val = reader.GetString() ?? throw new JsonException($"The value of Type {typeToConvert.Name} must not be null");
                string upperCamelCaseName = ToUpperCamelCase(val);
                if (Enum.TryParse(upperCamelCaseName, true, out TEnum enumVal))
                {
                    return enumVal;
                }
                throw new JsonException($"'{upperCamelCaseName}' is not in Enum {typeToConvert.Name}");
            }

            public override void Write(Utf8JsonWriter writer, TEnum value, JsonSerializerOptions options)
            {
                string val = ToSnakeCase(value.ToString());
                writer.WriteStringValue(val);
            }
        }
        /// <summary>
        /// <see cref="DateTime"/> Converter.
        /// <list type="bullet">Serialize to string of UTC</list>
        /// <list type="bullet">Deserialize to Local DateTime</list>
        /// </summary>
        public class LocalDateTimeConverter : JsonConverter<DateTime>
        {
            public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            {
                return reader.GetDateTime().ToLocalTime();
            }

            public override void Write(Utf8JsonWriter writer, DateTime dateTime, JsonSerializerOptions options)
            {
                writer.WriteStringValue(dateTime.ToUniversalTime().ToString("o"));
            }
        }
        /// <summary>
        /// Json Serialize / Deserialize OPTIONS for this API.
        /// <list type="bullet">Property PathName to snake_case for serialization</list>
        /// <list type="bullet">Property PathName In case sensitive for deserialization</list>
        /// <list type="bullet">DateTime to Local time zone for deserialization, to Utc for serialization</list>
        /// <list type="bullet">Non classed object to <see cref="OrderedDictionary"/> for deserialization</list>
        /// <list type="bullet">Non classed array to <see cref="object"/>?[] for deserialization</list>
        /// </summary>
        public static readonly JsonSerializerOptions DeserializeOptions = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
            PropertyNameCaseInsensitive = true,
            Converters =
            {
                new LocalDateTimeConverter(),
                // new DictConverter(),
                // new ArrayConverter()
            }
        };
        public static readonly JsonSerializerOptions SerializeOptions = new() {
            WriteIndented = true,
            PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
            Encoder = JavaScriptEncoder.Create(UnicodeRanges.All),
            Converters =
            {
                new LocalDateTimeConverter(),
            }
        };
        /// <summary>
        /// Deserialize JSON to <see cref="OrderedDictionary"/> and serialize
        /// </summary>
        private class DictConverter : JsonConverter<Dictionary<string, object?>>
        {
            private static readonly ArrayConverter arrayConverter = new();
            public override Dictionary<string, object?> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            {
                var dict = new Dictionary<string, object?>();
                while (reader.Read())
                {
                    if (reader.TokenType == JsonTokenType.EndObject)
                    {
                        break;
                    }
                    if (reader.TokenType != JsonTokenType.PropertyName)
                    {
                        throw new JsonException($"TokenType is not PropertyName: {reader.TokenType}");
                    }
                    string? propertyName = reader.GetString() ?? throw new JsonException("PropertyName is null");
                    reader.Read();
                    switch (reader.TokenType)
                    {
                        case JsonTokenType.Null:
                            dict.Add(propertyName, null); break;
                        case JsonTokenType.String:
                            if (reader.TryGetDateTime(out DateTime datetime))
                            {
                                dict.Add(propertyName, datetime.ToLocalTime());
                            }
                            else
                            {
                                dict.Add(propertyName, reader.GetString());
                            }
                            break;
                        case JsonTokenType.Number:
                            if (reader.TryGetInt32(out int int32val))
                            {
                                dict.Add(propertyName, int32val);
                            }
                            else if (reader.TryGetDouble(out double doubleVal))
                            {
                                dict.Add(propertyName, doubleVal);
                            }
                            else if (reader.TryGetInt64(out long int64val))
                            {
                                dict.Add(propertyName, int64val);
                            }
                            else if (reader.TryGetUInt64(out ulong uint64val))
                            {
                                dict.Add(propertyName, uint64val);
                            }
                            break;
                        case JsonTokenType.True:
                        case JsonTokenType.False:
                            dict.Add(propertyName, reader.GetBoolean()); break;
                        case JsonTokenType.StartArray:
                            /*
                            Type arrayType = typeof(object?[]);
                            var arrayConverter = (JsonConverter<object?[]>)options.GetConverter(arrayType);
                            dict.Add(propertyName, arrayConverter.Read(ref reader, arrayType, options));
                            */
                            dict.Add(propertyName, arrayConverter.Read(ref reader, typeof(IList), options));
                            break;
                        case JsonTokenType.StartObject:
                            dict.Add(propertyName, Read(ref reader, typeToConvert, options));
                            break;
                        default:
                            throw new JsonException($"Invalid TokenType: {reader.TokenType}");
                    }
                }
                return dict;
            }

            public override void Write(Utf8JsonWriter writer, Dictionary<string, object?> dict, JsonSerializerOptions options)
            {
                throw new NotImplementedException();
            }
        }
        /// <summary>
        /// Deserialize JSON to object[] and serialize
        /// </summary>
        private class ArrayConverter : JsonConverter<object?[]>
        {
            private static readonly DictConverter dictConverter = new();
            public override object?[] Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            {
                ArrayList array = [];
                while (reader.Read())
                {
                    switch (reader.TokenType)
                    {
                        case JsonTokenType.EndArray:
                            return array.ToArray();
                        case JsonTokenType.String:
                            if (reader.TryGetDateTime(out DateTime datetime))
                            {
                                array.Add(datetime);
                            }
                            else
                            {
                                array.Add(reader.GetString());
                            }
                            break;
                        case JsonTokenType.Number:
                            if (reader.TryGetInt32(out int int32val))
                            {
                                array.Add(int32val);
                            }
                            else if (reader.TryGetDouble(out double doubleVal))
                            {
                                array.Add(doubleVal);
                            }
                            else if (reader.TryGetInt64(out long int64val))
                            {
                                array.Add(int64val);
                            }
                            else if (reader.TryGetUInt64(out ulong uint64val))
                            {
                                array.Add(uint64val);
                            }
                            break;
                        case JsonTokenType.True:
                        case JsonTokenType.False:
                            array.Add(reader.GetBoolean()); break;
                        case JsonTokenType.Null:
                            array.Add(null); break;
                        case JsonTokenType.StartArray:
                            array.Add(Read(ref reader, typeToConvert, options));
                            break;
                        case JsonTokenType.StartObject:
                            /*
                            Type dictType = typeof(OrderedDictionary);
                            var dictConverter = (JsonConverter<OrderedDictionary>)options.GetConverter(dictType);
                            array.Add(dictConverter.Read(ref reader, dictType, options));
                            */
                            array.Add(dictConverter.Read(ref reader, typeof(OrderedDictionary), options));
                            break;
                        default:
                            throw new JsonException($"Invalid TokenType: {reader.TokenType}");
                    }
                }
                throw new JsonException($"End unexpectly: {reader.TokenType}");
            }

            public override void Write(Utf8JsonWriter writer, object?[] list, JsonSerializerOptions options)
            {
                JsonSerializer.Serialize(writer, list, options);
            }
        }

/*        public class SummaryFieldsConverter : JsonConverter<Dictionary<string, object>>
        {
            public override Dictionary<string, object>? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            {
                var dict = new Dictionary<string, object>();
                while (reader.Read())
                {
                    if (reader.TokenType == JsonTokenType.EndObject)
                    {
                        return dict;
                    }
                    if (reader.TokenType != JsonTokenType.PropertyName)
                    {
                        throw new JsonException($"TokenType is not PropertyName: {reader.TokenType}");
                    }
                    string? propertyName = reader.GetString() ?? throw new JsonException("PropertyName is null");
                    reader.Read();
                    object? val = null;
                    switch (propertyName)
                    {
                        case "user":
                        case "actor":
                        case "created_by":
                        case "modifed_by":
                            val = reader.TokenType == JsonTokenType.StartArray ?
                                    JsonSerializer.Deserialize<UserSummary[]>(ref reader, options) :
                                    JsonSerializer.Deserialize<UserSummary>(ref reader, options);
                            break;
                        case "o_auth2_access_token":
                            val = JsonSerializer.Deserialize<OAuth2AccessTokenSummary>(ref reader, options);
                            break;
                        case "user_capabilities":
                            val = JsonSerializer.Deserialize<UserCapability>(ref reader, options);
                            break;
                        case "organization":
                        case "job_template":
                        case "credential_type":
                        case "notification_template":
                            val = JsonSerializer.Deserialize<NameDescriptionSummary>(ref reader, options);
                            break;
                        case "object_roles":
                            val = JsonSerializer.Deserialize<Dictionary<string, RoleSummary>>(ref reader, options);
                            break;
                        case "related_field_counts":
                            val = JsonSerializer.Deserialize<Dictionary<string, int>>(ref reader, options);
                            break;
                        case "tokens":
                            val = JsonSerializer.Deserialize<ListSummary<TokenSummary>>(ref reader, options);
                            break;
                        case "credential":
                            val = JsonSerializer.Deserialize<CredentialSummary>(ref reader, options);
                            break;
                        case "credentials":
                            val = JsonSerializer.Deserialize<CredentialSummary[]>(ref reader, options);
                            break;
                        case "last_job":
                            val = JsonSerializer.Deserialize<LastJobSummary>(ref reader, options);
                            break;
                        case "last_update":
                            val = JsonSerializer.Deserialize<LastUpdateSummary>(ref reader, options);
                            break;
                        case "inventory":
                            val = JsonSerializer.Deserialize<InventorySummary>(ref reader, options);
                            break;
                        case "project":
                        case "source_project":
                            val = JsonSerializer.Deserialize<ProjectSummary>(ref reader, options);
                            break;
                        case "recent_jobs":
                            val = JsonSerializer.Deserialize<HostRecentJobSummary>(ref reader, options);
                            break;
                        case "labels":
                            val = JsonSerializer.Deserialize<ListSummary<NameSummary>>(ref reader, options);
                            break;
                        case "unified_job_template":
                            val = JsonSerializer.Deserialize<UnifiedJobTemplateSummary>(ref reader, options);
                            break;
                        case "instance_group":
                            val = JsonSerializer.Deserialize<InstanceGroupSummary>(ref reader, options);
                            break;
                        case "resource_name":
                        case "resource_type":
                        case "resource_type_display_name":
                            val = reader.GetString();
                            break;
                        case "resource_id":
                            val = reader.GetUInt64();
                            break;
                        default:
                            break;
                    }
                    if (val != null)
                        dict.Add(propertyName, val);

                }
                throw new JsonException($"End unexpectly: {reader.TokenType}");
            }

            public override void Write(Utf8JsonWriter writer, Dictionary<string, object> value, JsonSerializerOptions options)
            {
                JsonSerializer.Serialize(writer, value, options);
            }
        }
*/
        public class CapabilityConverter : JsonConverter<Capability>
        {
            public override Capability Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            {
                var cap = Capability.None;
                while (reader.Read())
                {
                    switch (reader.TokenType)
                    {
                        case JsonTokenType.EndObject:
                            return cap;
                        case JsonTokenType.PropertyName:
                            var propertyName = reader.GetString();
                            reader.Read();
                            var flag = reader.GetBoolean();
                            if (flag && propertyName != null)
                            {
                                cap |= Enum.Parse<Capability>(propertyName, true);
                            }
                            break;
                        default:
                            throw new JsonException();
                    }
                }
                throw new JsonException();
            }

            public override void Write(Utf8JsonWriter writer, Capability value, JsonSerializerOptions options)
            {
                writer.WriteStartObject();
                foreach (var capName in value.ToString("g").Split(", "))
                {
                    writer.WritePropertyName(capName.ToLowerInvariant());
                    writer.WriteBooleanValue(true);
                }
                writer.WriteEndObject();
            }
        }
    }
}
