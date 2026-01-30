using System.Text.Json.Serialization;

namespace Authi.Common.Dto
{
    public class DeleteResponse : OptionalResponse<DeleteResponse>
    {
        [JsonIgnore]
        public override DeleteResponse? Result => this;

        public required byte[] Body { get; init; }

        public class Payload : PayloadBase
        {
        }
    }
}
