using Authi.Common.Services;

namespace Authi.Server.Services
{
    public abstract class ServiceBase : IServiceConsumer
    {
        #region IServiceConsumer
        internal virtual IServiceConsumer Services => this;
        IServiceProvider IServiceConsumer.ServiceProvider => ServiceProvider.Current;
        #endregion
    }
}
