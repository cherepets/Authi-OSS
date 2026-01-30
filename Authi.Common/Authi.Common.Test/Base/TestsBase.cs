using Authi.Common.Services;
using Authi.Common.Test.Mocks;

namespace Authi.Common.Test
{
    public class TestsBase
    {
        private MockServiceProvider? _servicesMock;

        public MockServiceProvider ServicesMock
        {
            get => _servicesMock!;
            protected set => _servicesMock = value;
        }

        [TestInitialize]
        public virtual void Initialize()
        {
            InitServiceProvider();
            InitServiceLocator();
        }

        protected virtual void InitServiceLocator()
        {
            ServiceLocator.Init(
                typeof(ServiceLocator).Assembly);   // Authi.Common
        }

        protected virtual void InitServiceProvider()
        {
            ServicesMock = new MockServiceProvider();
        }

        [TestCleanup]
        public virtual void Cleanup()
        {
            _servicesMock = null;
        }
    }
}
