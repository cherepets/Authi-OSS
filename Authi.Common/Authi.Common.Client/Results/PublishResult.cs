using Authi.Common.Services;
using System;

namespace Authi.Common.Client.Results
{
    public class PublishResult
    {
        public required Guid SyncId { get; init; }
        public required AesKey OneTimeKey { get; init; }
    }
}
