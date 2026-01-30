using Authi.Common.Services;
using System.Collections.Generic;

namespace Authi.Common.Test.Mocks
{
    public class MockServiceProvider : ServiceProvider
    {
        private readonly Dictionary<string, object> _overrides = [];

        public MockServiceProvider()
        {
            Current = this;
        }

        public MockServiceProvider Override<T>(T mock) where T : class
        {
            _overrides[typeof(T).Name] = mock;
            return this;
        }

        public override T Get<T>()
        {
            if (_overrides.TryGetValue(typeof(T).Name, out var mock))
            {
                return (T)mock;
            }
            return base.Get<T>();
        }

        public override object GetNewInstance(string typeName)
        {
            if (_overrides.TryGetValue(typeName, out var mock))
            {
                return mock;
            }
            return base.GetNewInstance(typeName);
        }
    }
}
