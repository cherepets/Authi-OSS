using System;

namespace Authi.Server.Models
{
    public class Sync
    {
        public required Guid SyncId { get; set; }
        public required Guid DataId { get; set; }
        public required long CreatedAt { get; set; }
        public required KeyPair OneTimeKeyPair { get; set; }
    }
}
