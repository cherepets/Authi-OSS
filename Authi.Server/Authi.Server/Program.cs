using Authi.Common.Services;
using Authi.Server.ApiVersions;
using Authi.Server.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Authi.Server
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddCors(options =>
            {
                options.AddPolicy(name: "AllowOrigin",
                    policy => policy
                        .AllowAnyOrigin()
                        .AllowAnyHeader()
                        .AllowAnyMethod());
            });

            var app = builder.Build();
            app.UseDeveloperExceptionPage();
            app.UseCors("AllowOrigin");

            ServiceLocator.Init(
                typeof(ServiceLocator).Assembly,    // Authi.Common
                typeof(Program).Assembly);          // Authi.Server

            app.MapApiVersion(new ApiV1());

#if DEBUG
            app.MapApiVersion(new DebugApi());
#endif

            app.Run();
        }
    }
}