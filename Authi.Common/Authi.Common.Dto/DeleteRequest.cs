using System;

namespace Authi.Common.Dto
{
    public class DeleteRequest
    {
        public required Guid ClientId { get; init; }
        public required byte[] Body { get; init; }

        public class Payload : PayloadBase
        {
        }
    }
}
