using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using System.IO;
using System.Reflection;
using System.Text.Json;

namespace Authi.Server.ApiVersions
{
    public class HealthApi : ApiVersionBase
    {
        public override void ConfigureRoutes(IEndpointRouteBuilder app)
        {
            app.MapGet("/health", async () =>
            {
                var events = Services.AppHealthMonitor.GetEvents();

                var assembly = Assembly.GetExecutingAssembly();
                using var stream = assembly.GetManifestResourceStream("Authi.Server.Resources.health.html");
                using var reader = new StreamReader(stream!);
                var html = await reader.ReadToEndAsync();

                return Results.Content(
                    content: html.Replace("{{DATA}}", JsonSerializer.Serialize(events)),
                    contentType: "text/html; charset=utf-8");
            });
        }
    }
}
