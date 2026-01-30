using System.Text.Json;
using System.Text.Json.Serialization;

namespace Authi.Common.Extensions
{
    public static class JsonConverter
    {
        private static readonly JsonSerializerOptions DefaultOptions = new()
        {
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            PropertyNameCaseInsensitive = true
        };

        public static string ToJson<T>(this T obj)
        {
            return JsonSerializer.Serialize(obj, DefaultOptions);
        }

        public static T? FromJson<T>(this string json)
        {
            return JsonSerializer.Deserialize<T>(json, DefaultOptions);
        }
    }
}
