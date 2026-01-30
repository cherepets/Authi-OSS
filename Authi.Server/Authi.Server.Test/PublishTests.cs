using Authi.Common.Dto;
using Authi.Common.Extensions;
using Authi.Common.Services;
using Authi.Common.Test.Mocks;
using Authi.Server.ApiVersions;
using Authi.Server.Models;
using System;

namespace Authi.Server.Test
{
    [TestClass]
    public class PublishTests : ServerTestsBase
    {
        [TestMethod]
        public void PublishHappyTest()
        {
            var api = new ApiV1();

            var clock = new MockClock();
            ServicesMock.Override<IClock>(clock);
            clock.UniversalTime = DateTimeOffset.FromUnixTimeMilliseconds(255);

            var clientKeyPair = Services.Crypto.GenerateX25519KeyPair();
            var serverKeyPair = Services.Crypto.GenerateX25519KeyPair();
            (clientKeyPair, serverKeyPair) = ExchangePublicKeys(
                clientKeyPair,
                serverKeyPair);

            var clientId = Guid.NewGuid();
            var dataId = Guid.NewGuid();
            var version = Guid.NewGuid();
            var dbData = new Data
            {
                DataId = dataId,
                Version = version,
                Binary = [],
                LastAccessedAt = 0
            };

            var dbClient = new Client
            {
                ClientId = clientId,
                DataId = dataId,
                KeyPair = new KeyPair
                {
                    Private = serverKeyPair.Private.ToString(),
                    Public = serverKeyPair.Public.ToString()
                }
            };

            ClientRepository.Create(dbClient);
            DataRepository.Create(dbData);

            var oneTimeKeyPair = Services.Crypto.GenerateX25519KeyPair();
            var requestPayload = new PublishRequest.Payload
            {
                OneTimeClientPublicKey = oneTimeKeyPair.Public,
                Timestamp = Services.Clock.Timestamp
            };
            var requestBody = Services.Crypto.Encrypt(
                requestPayload.ToJson().ToUtfBytes(),
                clientKeyPair);
            var request = new PublishRequest
            {
                ClientId = clientId,
                Body = requestBody
            };

            // Call API
            var response = api.OnPublish(request);

            Assert.IsNull(response.Error);
            Assert.IsNotNull(response.Result);

            var responseBody = Services.Crypto.Decrypt(
                response.Result.Body,
                clientKeyPair);
            var responsePayload = responseBody.ToUtfString().FromJson<PublishResponse.Payload>();

            Assert.IsNotNull(responsePayload);
            Assert.AreEqual(255, responsePayload.Timestamp);
            Assert.AreNotEqual(Guid.Empty, responsePayload.SyncId);

            // Make sure doesn't throw
            _ = new X25519PublicKey(responsePayload.ServerPublicKey);

            Assert.AreEqual(1, SyncRepository.AsDictionary().Count);
            Assert.AreEqual(1, DataRepository.AsDictionary().Count);

            var syncRecord = SyncRepository.Read(responsePayload.SyncId);
            Assert.IsNotNull(syncRecord);
        }

        [TestMethod]
        public void PublishCantFindClientTest()
        {
            var api = new ApiV1();

            var clock = new MockClock();
            ServicesMock.Override<IClock>(clock);
            clock.UniversalTime = DateTimeOffset.FromUnixTimeMilliseconds(255);

            var clientKeyPair = Services.Crypto.GenerateX25519KeyPair();
            var serverKeyPair = Services.Crypto.GenerateX25519KeyPair();
            (clientKeyPair, serverKeyPair) = ExchangePublicKeys(
                clientKeyPair,
                serverKeyPair);

            var clientId = Guid.NewGuid();
            var dataId = Guid.NewGuid();
            var version = Guid.NewGuid();
            var dbData = new Data
            {
                DataId = dataId,
                Version = version,
                Binary = [],
                LastAccessedAt = 0
            };

            // Don't create client
            DataRepository.Create(dbData);

            var oneTimeKeyPair = Services.Crypto.GenerateX25519KeyPair();
            var requestPayload = new PublishRequest.Payload
            {
                OneTimeClientPublicKey = oneTimeKeyPair.Public,
                Timestamp = Services.Clock.Timestamp
            };
            var requestBody = Services.Crypto.Encrypt(
                requestPayload.ToJson().ToUtfBytes(),
                clientKeyPair);
            var request = new PublishRequest
            {
                ClientId = clientId,
                Body = requestBody
            };

            // Call API
            var response = api.OnPublish(request);

            Assert.IsNull(response.Result);
            Assert.IsNotNull(response.Error);

            Assert.AreEqual(ErrorMessages.CantFindClient, response.Error);
        }

        [TestMethod]
        public void PublishCantDecryptPayloadTest()
        {
            var api = new ApiV1();

            var clock = new MockClock();
            ServicesMock.Override<IClock>(clock);
            clock.UniversalTime = DateTimeOffset.FromUnixTimeMilliseconds(255);

            var dataKey = Services.Crypto.GenerateAesKey();
            var clientKeyPair = Services.Crypto.GenerateX25519KeyPair();
            var serverKeyPair = Services.Crypto.GenerateX25519KeyPair();
            (clientKeyPair, serverKeyPair) = ExchangePublicKeys(
                clientKeyPair,
                serverKeyPair);

            var clientId = Guid.NewGuid();
            var dataId = Guid.NewGuid();
            var version = Guid.NewGuid();
            var dbData = new Data
            {
                DataId = dataId,
                Version = version,
                Binary = [],
                LastAccessedAt = 0
            };

            var dbClient = new Client
            {
                ClientId = clientId,
                DataId = dataId,
                KeyPair = new KeyPair
                {
                    Private = serverKeyPair.Private.ToString(),
                    Public = serverKeyPair.Public.ToString()
                }
            };

            ClientRepository.Create(dbClient);
            DataRepository.Create(dbData);

            var oneTimeKeyPair = Services.Crypto.GenerateX25519KeyPair();
            var requestPayload = new PublishRequest.Payload
            {
                OneTimeClientPublicKey = oneTimeKeyPair.Public,
                Timestamp = Services.Clock.Timestamp
            };

            // Encrypt with the wrong key
            var requestBody = Services.Crypto.Encrypt(
                requestPayload.ToJson().ToUtfBytes(),
                dataKey);
            var request = new PublishRequest
            {
                ClientId = clientId,
                Body = requestBody
            };

            // Call API
            var response = api.OnPublish(request);

            Assert.IsNull(response.Result);
            Assert.IsNotNull(response.Error);

            Assert.AreEqual(ErrorMessages.CantDecryptPayload, response.Error);
        }

        [TestMethod]
        public void PublishCantVerifyClockTest()
        {
            var api = new ApiV1();

            var clock = new MockClock();
            ServicesMock.Override<IClock>(clock);
            clock.UniversalTime = DateTimeOffset.FromUnixTimeMilliseconds(0);

            var clientKeyPair = Services.Crypto.GenerateX25519KeyPair();
            var serverKeyPair = Services.Crypto.GenerateX25519KeyPair();
            (clientKeyPair, serverKeyPair) = ExchangePublicKeys(
                clientKeyPair,
                serverKeyPair);

            var clientId = Guid.NewGuid();
            var dataId = Guid.NewGuid();
            var version = Guid.NewGuid();
            var dbData = new Data
            {
                DataId = dataId,
                Version = version,
                Binary = [],
                LastAccessedAt = 0
            };

            var dbClient = new Client
            {
                ClientId = clientId,
                DataId = dataId,
                KeyPair = new KeyPair
                {
                    Private = serverKeyPair.Private.ToString(),
                    Public = serverKeyPair.Public.ToString()
                }
            };

            ClientRepository.Create(dbClient);
            DataRepository.Create(dbData);

            var oneTimeKeyPair = Services.Crypto.GenerateX25519KeyPair();
            var requestPayload = new PublishRequest.Payload
            {
                OneTimeClientPublicKey = oneTimeKeyPair.Public,
                Timestamp = Services.Clock.Timestamp
            };
            var requestBody = Services.Crypto.Encrypt(
                requestPayload.ToJson().ToUtfBytes(),
                clientKeyPair);
            var request = new PublishRequest
            {
                ClientId = clientId,
                Body = requestBody
            };

            // Set a later time
            clock.UniversalTime = DateTimeOffset.FromUnixTimeSeconds(31);

            // Call API
            var response = api.OnPublish(request);

            Assert.IsNull(response.Result);
            Assert.IsNotNull(response.Error);

            Assert.AreEqual(ErrorMessages.CantVerifyClock, response.Error);
        }
    }
}
