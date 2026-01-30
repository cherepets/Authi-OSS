using Authi.Common.Services;
using System;

namespace Authi.Common.Client
{
    public class SyncCode
    {
        public required Guid SyncId { get; init; }
        public required AesKey DataKey { get; init; }
        public required AesKey OneTimeKey { get; init; }

        public byte[] Serialize()
        {
            return [.. SyncId.ToByteArray(), .. DataKey.Bytes.ToArray(), .. OneTimeKey.Bytes.ToArray()];
        }

        public static SyncCode Deserialize(byte[] bytes)
        {
            const int guidLength = 16;

            var syncId = new Guid(bytes[..guidLength]);

            var keys = bytes.AsMemory()[guidLength..];
            int keySize = keys.Length / 2;
            var dataKeyBytes = keys[..keySize];
            var oneTimeKeyBytes = keys[keySize..];

            return new SyncCode
            {
                SyncId = syncId,
                DataKey = new AesKey(dataKeyBytes),
                OneTimeKey = new AesKey(oneTimeKeyBytes)
            };
        }
    }
}
