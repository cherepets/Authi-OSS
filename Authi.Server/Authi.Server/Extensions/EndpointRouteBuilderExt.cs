using Authi.Server.ApiVersions;
using Microsoft.AspNetCore.Routing;

namespace Authi.Server.Extensions
{
    public static class EndpointRouteBuilderExt
    {
        public static void MapApiVersion(this IEndpointRouteBuilder app, ApiVersionBase apiVersion)
        {
            apiVersion.ConfigureRoutes(app);
        }
    }
}
