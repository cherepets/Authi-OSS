using Authi.Server.ApiVersions;
using Microsoft.AspNetCore.Routing;

namespace Authi.Server.Extensions
{
    internal static class EndpointRouteBuilderExt
    {
        public static T MapApiVersion<T>(this T builder, ApiVersionBase apiVersion) where T : IEndpointRouteBuilder
        {
            apiVersion.ConfigureRoutes(builder);
            return builder;
        }
    }
}
