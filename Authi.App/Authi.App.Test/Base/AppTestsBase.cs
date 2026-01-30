using Authi.App.Logic;
using Authi.App.Logic.Services;
using Authi.Common.Services;
using Authi.Common.Test;

namespace Authi.App.Test
{
    public class AppTestsBase : TestsBase, IServiceConsumer
    {
        protected override void InitServiceLocator()
        {
            ServiceLocator.Init(
                typeof(AppTestsBase).Assembly,      // Authi.Common
                typeof(ServiceLocator).Assembly,    // Authi.Common
                typeof(Config).Assembly);           // Authi.App.Logic
        }

        #region IServiceConsumer
        internal virtual IServiceConsumer Services => this;
        IServiceProvider IServiceConsumer.ServiceProvider => ServiceProvider.Current;
        #endregion
    }
}
