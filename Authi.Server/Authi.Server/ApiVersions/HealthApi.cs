using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using System;
using System.IO;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace Authi.Server.ApiVersions
{
    public class HealthApi : ApiVersionBase
    {
        /* OMITTED IN OSS BUILD */
        public string SecretToken => "input_your_token";

        public override void ConfigureRoutes(IEndpointRouteBuilder app)
        {
            app.MapGet("/health", async (HttpContext context) =>
            {
                if (IsAuthorized(context))
                {
                    var events = Services.AppHealthMonitor.GetEvents();
                    var html = GetHtmlFileContent("health-dashboard");
                    return Results.Content(
                        content: html.Replace("{{DATA}}", JsonSerializer.Serialize(events)),
                        contentType: "text/html; charset=utf-8");
                }
                else
                {
                    var html = GetHtmlFileContent("health-login");
                    return Results.Content(
                        content: html,
                        contentType: "text/html; charset=utf-8");
                }
            });
        }

        private bool IsAuthorized(HttpContext context)
        {
            var authCookie = context.Request.Cookies["health-auth"];
            if (authCookie == null)
            {
                return false;
            }

            return authCookie == SecretToken;
        }

        private static string GetHtmlFileContent(string fileName)
        {
            var assembly = Assembly.GetExecutingAssembly();
            using var stream = assembly.GetManifestResourceStream($"Authi.Server.Resources.{fileName}.html");
            using var reader = new StreamReader(stream!);
            return reader.ReadToEnd();
        }
    }
}
