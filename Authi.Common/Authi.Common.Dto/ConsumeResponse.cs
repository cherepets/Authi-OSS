using System;
using System.Text.Json.Serialization;

namespace Authi.Common.Dto
{
    public class ConsumeResponse : OptionalResponse<ConsumeResponse>
    {
        [JsonIgnore]
        public override ConsumeResponse? Result => this;

        public required byte[] Body { get; init; }

        public class Payload : PayloadBase
        {
            public required Guid ClientId { get; init; }
            public required byte[] ServerPublicKey { get; init; }
        }
    }
}
