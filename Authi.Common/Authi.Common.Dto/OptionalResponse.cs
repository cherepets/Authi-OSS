using System.Text.Json.Serialization;

namespace Authi.Common.Dto
{
    public class OptionalResponse<T> where T : class
    {
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public virtual T? Result { get; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? Error { get; }

        public OptionalResponse(string? error)
        {
            Error = error;
        }

        internal OptionalResponse()
        {
        }
    }
}
