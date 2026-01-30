using System;

namespace Authi.Common.Dto
{
    public class WriteRequest
    {
        public required Guid ClientId { get; init; }
        public required byte[] Body { get; init; }

        public class Payload : PayloadBase
        {
            public required byte[] Binary { get; init; }
        }
    }
}
