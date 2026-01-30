using System;
using System.Text.Json.Serialization;

namespace Authi.Common.Dto
{
    public class PublishResponse : OptionalResponse<PublishResponse>
    {
        [JsonIgnore]
        public override PublishResponse? Result => this;

        public required byte[] Body { get; init; }

        public class Payload : PayloadBase
        {
            public required Guid SyncId { get; init; }
            public required byte[] ServerPublicKey { get; init; }
        }
    }
}
