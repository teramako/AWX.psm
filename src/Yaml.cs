using YamlDotNet.Core;
using YamlDotNet.Core.Events;
using YamlDotNet.Serialization;
using System.Text;
using System.Collections;

namespace AWX
{
    public class Yaml
    {
        public static IDeserializer Deserializer {
            get {
                if (_deserializer != null)
                    return _deserializer;

                _deserializer = new DeserializerBuilder().Build();
                return _deserializer;
            }
        }
        private static IDeserializer? _deserializer = null;

        /// <summary>
        /// Deserialize YAML string to Dictionary
        /// </summary>
        /// <param name="yaml">YAML or JSON string</param>
        /// <returns>Dictionary object</returns>
        public static Dictionary<string, object?> DeserializeToDict(string yaml)
        {
            var result = new Dictionary<string, object?>();
            var parser = new Parser(new StringReader(yaml));
            parser.Consume<StreamStart>();
            if (!parser.TryConsume<DocumentStart>(out _))
            {
                return result;
            }
            if (!parser.TryConsume<MappingStart>(out _))
            {
                return result;
            }
            return ParseDict(parser, result);
        }

        private static object? ParseScalar(Scalar scalar)
        {
            if (scalar.IsQuotedImplicit)
                return scalar.Value;
            var stringValue = scalar.Value;
            switch (stringValue.ToLowerInvariant())
            {
                case "null":
                    return null;
                case "true":
                case "yes":
                    return true;
                case "false":
                case "no":
                    return false;
            }
            if (int.TryParse(stringValue, out var intVal))
                return intVal;
            if (long.TryParse(stringValue, out var longVal))
                return longVal;
            if (double.TryParse(stringValue, out var doubleVal))
                return doubleVal;

            return stringValue;
        }

        private static Dictionary<string, object?> ParseDict(IParser parser, Dictionary<string, object?> dict)
        {
            while (!parser.TryConsume<MappingEnd>(out _))
            {
                var key = parser.Consume<Scalar>();
                if (parser.TryConsume<MappingStart>(out _))
                {
                    dict.Add(key.Value, ParseDict(parser, new Dictionary<string, object?>()));
                }
                else if (parser.TryConsume<SequenceStart>(out _))
                {
                    dict.Add(key.Value, ParseArray(parser, new ArrayList()));
                }
                else if (parser.TryConsume<Scalar>(out var scalar))
                {
                    dict.Add(key.Value, ParseScalar(scalar));
                }
            }
            return dict;
        }

        private static object?[] ParseArray(IParser parser, ArrayList array)
        {
            while (!parser.TryConsume<SequenceEnd>(out _))
            {
                if (parser.TryConsume<MappingStart>(out _))
                {
                    array.Add(ParseDict(parser, new Dictionary<string, object?>()));
                }
                else if (parser.TryConsume<SequenceStart>(out _))
                {
                    array.Add(ParseArray(parser, new ArrayList()));
                }
                else if (parser.TryConsume<Scalar>(out var scalar))
                {
                    array.Add(ParseScalar(scalar));
                }
            }
            return array.ToArray();
        }

    }
}
