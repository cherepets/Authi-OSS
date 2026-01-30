using Authi.Common.Dto;
using Authi.Common.Extensions;
using System;

namespace Authi.Server.ApiVersions
{
    public partial class ApiV1 : ApiVersionBase
    {
        public OptionalResponse<ReadResponse> OnRead(ReadRequest request)
        {
            var client = Services.ClientRepository.Read(request.ClientId);
            if (client == null)
            {
                return new ErrorResponse<ReadResponse>(ErrorMessages.CantFindClient);
            }

            var keyPair = client.KeyPair.ToX25519KeyPair();

            string requestJson;
            try
            {
                requestJson = Services.Crypto.Decrypt(request.Body, keyPair).ToUtfString();
            }
            catch
            {
                return new ErrorResponse<ReadResponse>(ErrorMessages.CantDecryptPayload);
            }

            var requestPayload = requestJson.FromJson<ReadRequest.Payload>();
            if (VerifyPayload<ReadResponse>(requestPayload) is { } error)
            {
                return error;
            }

            var data = Services.DataRepository.Read(client.DataId);
            if (data == null)
            {
                return new ErrorResponse<ReadResponse>(ErrorMessages.CantFindData);
            }
            if (data.Version == Guid.Empty)
            {
                return new ErrorResponse<ReadResponse>(ErrorMessages.CantReadDataVersion);
            }

            data.LastAccessedAt = Services.Clock.Timestamp;
            Services.DataRepository.Update(data);

            var hasChanges = data.Version != requestPayload.Version;

            var responsePayload = new ReadResponse.Payload
            {
                Binary = hasChanges ? data.Binary : [],
                Version = data.Version,
                HasChanges = hasChanges,
                Timestamp = Services.Clock.Timestamp
            };
            var responseBody = Services.Crypto.Encrypt(
                responsePayload.ToJson().ToUtfBytes(),
                keyPair);

            return new ReadResponse
            {
                Body = responseBody
            };
        }
    }
}
