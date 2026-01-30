using System;

namespace Authi.Common.Dto
{
    public class ConsumeRequest
    {
        public required Guid SyncId { get; init; }
        public required byte[] Body { get; init; }

        public class Payload : PayloadBase
        {
            public required byte[] ClientPublicKey { get; init; }
        }
    }
}
