using System;
using System.Text.Json.Serialization;

namespace Authi.Common.Dto
{
    public class InitResponse : OptionalResponse<InitResponse>
    {
        [JsonIgnore]
        public override InitResponse? Result => this;

        public required byte[] ServerPublicKey { get; init; }
        public required byte[] Body { get; init; }

        public class Payload : PayloadBase
        {
            public required Guid ClientId { get; init; }
            public required Guid Version { get; init; }
        }
    }
}
