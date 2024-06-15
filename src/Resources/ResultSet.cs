using System.Text.Json;

namespace AWX.Resources
{
    public class ResultSet(int count, string? next, string? previous, object[] results)
    {
        public int Count { get; } = count;
        public string? Next { get; } = next;
        public string? Previous { get; } = previous;

        public object?[] Results { get; } = results.OfType<JsonElement>()
                                                   .Select(json => Json.ObjectToInferredType(json, true))
                                                   .ToArray();
    }

    public class ResultSet<T>(int count,
                              string? next,
                              string? previous,
                              T[] results)
    {
        public int Count { get; } = count;
        public string? Next { get; } = next;
        public string? Previous { get; } = previous;
        public T[] Results { get; } = results;
    }
}
