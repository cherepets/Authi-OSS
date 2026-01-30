using Authi.Common.Services;
using Authi.Server.Services;
using Microsoft.AspNetCore.Routing;

namespace Authi.Server.ApiVersions
{
    public abstract class ApiVersionBase : IServiceConsumer
    {
        public abstract void ConfigureRoutes(IEndpointRouteBuilder app);

        #region IServiceConsumer
        internal virtual IServiceConsumer Services => this;
        IServiceProvider IServiceConsumer.ServiceProvider => ServiceProvider.Current;
        #endregion
    }
}
