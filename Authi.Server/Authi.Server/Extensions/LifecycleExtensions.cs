using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;

namespace Authi.Server.Extensions
{
    internal static class LifecycleExtensions
    {
        public static T OnApplicationStopping<T>(this T host, Action handler) where T : IHost
        {
            host.Services
                .GetRequiredService<IHostApplicationLifetime>()
                .ApplicationStopping
                .Register(handler);
            return host;
        }
    }
}
