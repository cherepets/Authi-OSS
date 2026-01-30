using System;
using System.Collections.Generic;

namespace Authi.Common.Client.Results
{
    public class ReadResult
    {
        public required IReadOnlyCollection<CredentialDto> Credentials { get; init; }
        public required Guid Version { get; init; }
        public required bool HasChanges { get; init; }
    }
}
