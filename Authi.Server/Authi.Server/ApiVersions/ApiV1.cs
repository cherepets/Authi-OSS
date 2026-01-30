using Authi.Common.Dto;
using Authi.Server.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Authi.Server.ApiVersions
{
    public partial class ApiV1 : ApiVersionBase
    {
        private readonly TimeSpan RequestValidFor = TimeSpan.FromSeconds(30);

        public override void ConfigureRoutes(IEndpointRouteBuilder app)
        {
            var api = app.MapGroup("/api/v1");
            api.MapPost("/init", OnInit);
            api.MapPost("/read", OnRead);
            api.MapPost("/write", OnWrite);
            api.MapPost("/delete", OnDelete);
            api.MapPost("/publish", OnPublish);
            api.MapPost("/consume", OnConsume);
            CleanUp();
        }

        private void CleanUp()
        {
            var syncTimeStamp = Services.Clock.UniversalTime.AddDays(-1).ToUnixTimeMilliseconds();
            var dataTimeStamp = Services.Clock.UniversalTime.AddDays(-365).ToUnixTimeMilliseconds();

            var outdatedSync = Services.AppDbContext.Find<Sync>(x => x.CreatedAt < syncTimeStamp);
            var outdatedData = Services.AppDbContext.Find<Data>(x => x.LastAccessedAt < dataTimeStamp);
            var orphanedUsers = outdatedData.SelectMany(data => Services.AppDbContext.Find<Client>(x => x.DataId == data.DataId)).ToArray();

            Services.AppDbContext.Delete(outdatedSync);
            Services.AppDbContext.Delete(outdatedData);
            Services.AppDbContext.Delete(orphanedUsers);
        }

        private ErrorResponse<T>? VerifyPayload<T>([NotNull] PayloadBase? payload) where T : class
        {
            if (payload == null)
            {
                payload = new PayloadBase { Timestamp = 0 };
                return new ErrorResponse<T>(ErrorMessages.CantParsePayload);
            }

            if (!Services.Clock.IsRecent(payload.Timestamp, RequestValidFor))
            {
                return new ErrorResponse<T>(ErrorMessages.CantVerifyClock);
            }

            return null;
        }
    }
}
