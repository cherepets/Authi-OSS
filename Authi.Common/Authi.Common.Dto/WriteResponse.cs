using System;
using System.Text.Json.Serialization;

namespace Authi.Common.Dto
{
    public class WriteResponse : OptionalResponse<WriteResponse>
    {
        [JsonIgnore]
        public override WriteResponse? Result => this;

        public required byte[] Body { get; init; }

        public class Payload : PayloadBase
        {
            public required Guid Version { get; init; }
        }
    }
}
