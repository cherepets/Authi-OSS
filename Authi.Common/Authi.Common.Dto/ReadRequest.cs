using System;

namespace Authi.Common.Dto
{
    public class ReadRequest
    {
        public required Guid ClientId { get; init; }
        public required byte[] Body { get; init; }

        public class Payload : PayloadBase
        {
            public required Guid Version { get; init; }
        }
    }
}
