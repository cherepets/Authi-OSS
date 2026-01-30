using Authi.Common.Dto;
using Authi.Common.Extensions;

namespace Authi.Server.ApiVersions
{
    public partial class ApiV1 : ApiVersionBase
    {
        public OptionalResponse<DeleteResponse> OnDelete(DeleteRequest request)
        {
            var client = Services.ClientRepository.Read(request.ClientId);
            if (client == null)
            {
                return new ErrorResponse<DeleteResponse>(ErrorMessages.CantFindClient);
            }

            var keyPair = client.KeyPair.ToX25519KeyPair();

            string requestJson;
            try
            {
                requestJson = Services.Crypto.Decrypt(request.Body, keyPair).ToUtfString();
            }
            catch
            {
                return new ErrorResponse<DeleteResponse>(ErrorMessages.CantDecryptPayload);
            }

            var requestPayload = requestJson.FromJson<DeleteRequest.Payload>();
            if (VerifyPayload<DeleteResponse>(requestPayload) is { } error)
            {
                return error;
            }

            var data = Services.DataRepository.Read(client.DataId);
            if (data == null)
            {
                return new ErrorResponse<DeleteResponse>(ErrorMessages.CantFindData);
            }

            var responsePayload = new DeleteResponse.Payload
            {
                Timestamp = Services.Clock.Timestamp
            };
            var responseBody = Services.Crypto.Encrypt(
                responsePayload.ToJson().ToUtfBytes(),
                keyPair);

            Services.ClientRepository.Delete(client);
            Services.DataRepository.Delete(data);
            return new DeleteResponse
            {
                Body = responseBody
            };
        }
    }
}
