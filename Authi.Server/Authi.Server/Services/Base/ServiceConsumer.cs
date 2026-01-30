using Authi.Common.Services;

namespace Authi.Server.Services
{
    internal interface IServiceConsumer
    {
        IServiceProvider ServiceProvider { get; }

        public IAppDbContext AppDbContext => ServiceProvider.Get<IAppDbContext>();
        public IClock Clock => ServiceProvider.Get<IClock>();
        public ICrypto Crypto => ServiceProvider.Get<ICrypto>();
        public IClientRepository ClientRepository => ServiceProvider.Get<IClientRepository>();
        public IDataRepository DataRepository => ServiceProvider.Get<IDataRepository>();
        public ISyncRepository SyncRepository => ServiceProvider.Get<ISyncRepository>();
    }
}
