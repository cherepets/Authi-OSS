using System;
using System.Text.Json.Serialization;

namespace Authi.Common.Dto
{
    public class ReadResponse : OptionalResponse<ReadResponse>
    {
        [JsonIgnore]
        public override ReadResponse? Result => this;

        public required byte[] Body { get; init; }

        public class Payload : PayloadBase
        {
            public required byte[] Binary { get; init; }
            public required Guid Version { get; init; }
            public required bool HasChanges { get; init; }
        }
    }
}
