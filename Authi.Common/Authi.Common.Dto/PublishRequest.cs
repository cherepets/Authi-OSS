using System;

namespace Authi.Common.Dto
{
    public class PublishRequest
    {
        public required Guid ClientId { get; init; }
        public required byte[] Body { get; init; }

        public class Payload : PayloadBase
        {
            public required byte[] OneTimeClientPublicKey { get; init; }
        }
    }
}
