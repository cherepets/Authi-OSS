using System;

namespace Authi.Server.Models
{
    public class Client
    {
        public required Guid ClientId { get; set; }
        public required Guid DataId { get; set; }
        public required KeyPair KeyPair { get; set; }
    }
}
