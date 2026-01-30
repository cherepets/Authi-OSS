using Authi.Common.Services;
using System;

namespace Authi.Common.Test.Mocks
{
    public class MockClock : IClock
    {
        public DateTimeOffset UniversalTime { get; set; }
    }
}
