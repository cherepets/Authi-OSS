using Authi.Common.Dto;
using Authi.Common.Extensions;
using Authi.Common.Services;
using Authi.Server.Models;
using System;

namespace Authi.Server.ApiVersions
{
    public partial class ApiV1 : ApiVersionBase
    {
        private readonly TimeSpan SyncValidFor = TimeSpan.FromMinutes(3);

        public OptionalResponse<ConsumeResponse> OnConsume(ConsumeRequest request)
        {
            var sync = Services.SyncRepository.Read(request.SyncId);
            if (sync == null)
            {
                return new ErrorResponse<ConsumeResponse>(ErrorMessages.CantFindSync);
            }
            Services.SyncRepository.Delete(sync);

            if (!Services.Clock.IsRecent(sync.CreatedAt, SyncValidFor))
            {
                return new ErrorResponse<ConsumeResponse>(ErrorMessages.SyncExpired);
            }

            var oneTimeKeyPair = sync.OneTimeKeyPair.ToX25519KeyPair();

            string requestJson;
            try
            {
                requestJson = Services.Crypto.Decrypt(request.Body, oneTimeKeyPair).ToUtfString();
            }
            catch
            {
                return new ErrorResponse<ConsumeResponse>(ErrorMessages.CantDecryptPayload);
            }

            var requestPayload = requestJson.FromJson<ConsumeRequest.Payload>();
            if (VerifyPayload<ConsumeResponse>(requestPayload) is { } error)
            {
                return error;
            }

            var clientPublicKey = new X25519PrivateKey(requestPayload.ClientPublicKey);
            var serverKey = Services.Crypto.GenerateX25519KeyPair();

            var clientId = Guid.NewGuid();
            var client = new Client
            {
                ClientId = clientId,
                DataId = sync.DataId,
                KeyPair = new KeyPair
                {
                    Private = serverKey.Private.ToString(),
                    Public = clientPublicKey.ToString()
                }
            };

            try
            {
                Services.ClientRepository.Create(client);
            }
            catch
            {
                return new ErrorResponse<ConsumeResponse>(ErrorMessages.CantCreateClient);
            }

            var responsePayload = new ConsumeResponse.Payload
            {
                ClientId = clientId,
                ServerPublicKey = serverKey.Public,
                Timestamp = Services.Clock.Timestamp
            };
            var responseBody = Services.Crypto.Encrypt(
                responsePayload.ToJson().ToUtfBytes(),
                oneTimeKeyPair);
            return new ConsumeResponse
            {
                Body = responseBody
            };
        }
    }
}
