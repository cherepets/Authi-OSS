using Authi.Common.Client.Exceptions;
using Authi.Common.Client.Results;
using Authi.Common.Dto;
using Authi.Common.Extensions;
using Authi.Common.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;

namespace Authi.Common.Client
{
    public class ApiClient
    {
        private readonly TimeSpan ResponseValidFor = TimeSpan.FromSeconds(30);

        private IClock Clock { get; }
        private ICrypto Crypto { get; }

        private readonly Api _api = new();

        public ApiClient(IClock clock, ICrypto crypto)
        {
            Clock = clock;
            Crypto = crypto;
        }

        public async Task<ConsumeResult> ConsumeAsync(SyncCode syncCode)
        {
            var clientKeyPair = Crypto.GenerateX25519KeyPair();
            var requestPayload = new ConsumeRequest.Payload
            {
                ClientPublicKey = clientKeyPair.Public,
                Timestamp = Clock.Timestamp
            };
            var requestBody = Crypto.Encrypt(
                requestPayload.ToJson().ToUtfBytes(),
                syncCode.OneTimeKey);
            var request = new ConsumeRequest
            {
                SyncId = syncCode.SyncId,
                Body = requestBody
            };

            var response = await _api.ConsumeAsync(request);

            var responseJson = Crypto.Decrypt(response.Body, syncCode.OneTimeKey).ToUtfString();
            var responsePayload = responseJson.FromJson<ConsumeResponse.Payload>();
            VerifyPayload(responsePayload);

            var serverPublicKey = new X25519PublicKey(responsePayload.ServerPublicKey);
            return new ConsumeResult
            {
                ClientId = responsePayload.ClientId,
                DataKey = syncCode.DataKey,
                SyncKeyPair = new X25519KeyPair(
                    clientKeyPair.Private,
                    serverPublicKey)
            };
        }

        public async Task<InitResult> InitAsync()
        {
            var clientKeyPair = Crypto.GenerateX25519KeyPair();
            var requestPayload = new InitRequest.Payload
            {
                Timestamp = Clock.Timestamp
            };
            var requestBody = Crypto.Encrypt(
                requestPayload.ToJson().ToUtfBytes(),
                new AesKey(clientKeyPair.Public));
            var request = new InitRequest
            {
                ClientPublicKey = clientKeyPair.Public,
                Body = requestBody
            };

            var response = await _api.InitAsync(request);

            var syncKeyPair = new X25519KeyPair(
                new X25519PrivateKey(clientKeyPair.Private),
                new X25519PublicKey(response.ServerPublicKey));

            var responseJson = Crypto.Decrypt(response.Body, syncKeyPair).ToUtfString();
            var responsePayload = responseJson.FromJson<InitResponse.Payload>();
            VerifyPayload(responsePayload);

            var serverPublicKey = new X25519PublicKey(response.ServerPublicKey);
            var dataKey = Crypto.GenerateAesKey();
            return new InitResult
            {
                ClientId = responsePayload.ClientId,
                DataKey = dataKey,
                SyncKeyPair = new X25519KeyPair(
                    clientKeyPair.Private,
                    serverPublicKey)
            };
        }

        public async Task<PublishResult> PublishAsync(Guid clientId, X25519KeyPair syncKeyPair)
        {
            var oneTimeKey = Crypto.GenerateX25519KeyPair();
            var requestPayload = new PublishRequest.Payload
            {
                OneTimeClientPublicKey = oneTimeKey.Public,
                Timestamp = Clock.Timestamp
            };
            var requestBody = Crypto.Encrypt(
                requestPayload.ToJson().ToUtfBytes(),
                syncKeyPair);
            var request = new PublishRequest
            {
                ClientId = clientId,
                Body = requestBody
            };

            var response = await _api.PublishAsync(request);

            var responseJson = Crypto.Decrypt(response.Body, syncKeyPair).ToUtfString();
            var responsePayload = responseJson.FromJson<PublishResponse.Payload>();
            VerifyPayload(responsePayload);

            return new PublishResult
            {
                SyncId = responsePayload.SyncId,
                OneTimeKey = new X25519KeyPair(oneTimeKey.Private, new X25519PublicKey(responsePayload.ServerPublicKey)).DeriveAesKey()
            };
        }

        public async Task<ReadResult> ReadAsync(Guid clientId, Guid version, AesKey dataKey, X25519KeyPair syncKeyPair)
        {
            var requestPayload = new ReadRequest.Payload
            {
                Timestamp = Clock.Timestamp,
                Version = version
            };
            var requestBody = Crypto.Encrypt(
                requestPayload.ToJson().ToUtfBytes(),
                syncKeyPair);
            var request = new ReadRequest
            {
                ClientId = clientId,
                Body = requestBody
            };

            var response = await _api.ReadAsync(request);

            var responseJson = Crypto.Decrypt(response.Body, syncKeyPair).ToUtfString();
            var responsePayload = responseJson.FromJson<ReadResponse.Payload>();
            VerifyPayload(responsePayload);

            var credentials = responsePayload.Binary.Length > 0
                ? Crypto
                    .Decrypt(responsePayload.Binary, dataKey)
                    .ToUtfString()
                    .FromJson<CredentialDto[]>()
                : null;

            return new ReadResult
            {
                Credentials = credentials ?? [],
                Version = responsePayload.Version,
                HasChanges = responsePayload.HasChanges
            };
        }

        public async Task<WriteResult> WriteAsync(IReadOnlyCollection<CredentialDto> credentials, Guid clientId, AesKey dataKey, X25519KeyPair syncKeyPair)
        {
            var credentialsJson = credentials.ToJson();
            var dataBinary = credentialsJson.ToUtfBytes();
            var requestPayload = new WriteRequest.Payload
            {
                Binary = Crypto.Encrypt(dataBinary, dataKey),
                Timestamp = Clock.Timestamp
            };
            var requestBody = Crypto.Encrypt(
                requestPayload.ToJson().ToUtfBytes(),
                syncKeyPair);
            var request = new WriteRequest
            {
                ClientId = clientId,
                Body = requestBody
            };
            var response = await _api.WriteAsync(request);

            var responseJson = Crypto.Decrypt(response.Body, syncKeyPair).ToUtfString();
            var responsePayload = responseJson.FromJson<WriteResponse.Payload>();
            VerifyPayload(responsePayload);

            return new WriteResult
            {
                Version = responsePayload.Version
            };
        }

        private void VerifyPayload([NotNull] PayloadBase? payload)
        {
            if (payload == null)
            {
                payload = new PayloadBase { Timestamp = 0 };
                throw new ApiException("Can't parse payload.");
            }

            if (!Clock.IsRecent(payload.Timestamp, ResponseValidFor))
            {
                throw new ApiException("Can't verify clock.");
            }
        }
    }
}
