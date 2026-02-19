using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Authi.Server.Extensions
{
    internal static class CorsExtensions
    {
        private const string PolicyName = "AllowOrigin";

        public static IServiceCollection NoCors(this IServiceCollection services)
        {
            return services.AddCors(options =>
                options.AddPolicy(PolicyName,
                    policy => policy
                        .AllowAnyOrigin()
                        .AllowAnyHeader()
                        .AllowAnyMethod()));
        }

        public static T NoCors<T>(this T builder) where T : IApplicationBuilder
        {
            builder.UseCors(PolicyName);
            return builder;
        }
    }
}
