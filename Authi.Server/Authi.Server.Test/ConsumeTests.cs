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
    public class ConsumeTests : ServerTestsBase
    {
        [TestMethod]
        public void ConsumeHappyTest()
        {
            var api = new ApiV1();

            var clock = new MockClock();
            ServicesMock.Override<IClock>(clock);
            clock.UniversalTime = DateTimeOffset.FromUnixTimeMilliseconds(255);

            var oneTimeClientKeyPair = Services.Crypto.GenerateX25519KeyPair();
            var oneTimeServerKeyPair = Services.Crypto.GenerateX25519KeyPair();
            (oneTimeClientKeyPair, oneTimeServerKeyPair) = ExchangePublicKeys(
                oneTimeClientKeyPair,
                oneTimeServerKeyPair);

            var clientKeyPair = Services.Crypto.GenerateX25519KeyPair();
            var serverKeyPair = Services.Crypto.GenerateX25519KeyPair();
            (clientKeyPair, serverKeyPair) = ExchangePublicKeys(
                clientKeyPair,
                serverKeyPair);

            var dataId = Guid.NewGuid();
            var syncId = Guid.NewGuid();
            var version = Guid.NewGuid();
            var dbData = new Data
            {
                DataId = dataId,
                Version = version,
                Binary = [],
                LastAccessedAt = 0
            };

            var dbSync = new Sync
            {
                SyncId = syncId,
                DataId = dataId,
                CreatedAt = ServicesMock.Get<IClock>().Timestamp,
                OneTimeKeyPair = new KeyPair
                {
                    Private = oneTimeServerKeyPair.Private.ToString(),
                    Public = oneTimeServerKeyPair.Public.ToString()
                }
            };

            DataRepository.Create(dbData);
            SyncRepository.Create(dbSync);

            var requestPayload = new ConsumeRequest.Payload
            {
                ClientPublicKey = clientKeyPair.Public,
                Timestamp = Services.Clock.Timestamp
            };
            var requestBody = Services.Crypto.Encrypt(
                requestPayload.ToJson().ToUtfBytes(),
                oneTimeClientKeyPair);
            var request = new ConsumeRequest
            {
                SyncId = syncId,
                Body = requestBody
            };

            // Call API
            var response = api.OnConsume(request);

            Assert.IsNull(response.Error);
            Assert.IsNotNull(response.Result);

            var responseBody = Services.Crypto.Decrypt(
                response.Result.Body,
                oneTimeClientKeyPair);
            var responsePayload = responseBody.ToUtfString().FromJson<ConsumeResponse.Payload>();

            Assert.IsNotNull(responsePayload);
            Assert.AreEqual(255, responsePayload.Timestamp);
            Assert.AreNotEqual(Guid.Empty, responsePayload.ClientId);

            Assert.AreEqual(1, ClientRepository.AsDictionary().Count);
            Assert.AreEqual(1, DataRepository.AsDictionary().Count);
            Assert.AreEqual(0, SyncRepository.AsDictionary().Count);

            var clientRecord = ClientRepository.Read(responsePayload.ClientId);
            Assert.IsNotNull(clientRecord);

            var dataRecord = DataRepository.Read(clientRecord.DataId);
            Assert.IsNotNull(dataRecord);

            Assert.AreEqual(clientRecord.DataId, dataRecord.DataId);
        }

        [TestMethod]
        public void ConsumeSyncExpiredTest()
        {
            var api = new ApiV1();

            var clock = new MockClock();
            ServicesMock.Override<IClock>(clock);
            clock.UniversalTime = DateTimeOffset.FromUnixTimeMilliseconds(300000);

            var oneTimeClientKeyPair = Services.Crypto.GenerateX25519KeyPair();
            var oneTimeServerKeyPair = Services.Crypto.GenerateX25519KeyPair();
            (oneTimeClientKeyPair, oneTimeServerKeyPair) = ExchangePublicKeys(
                oneTimeClientKeyPair,
                oneTimeServerKeyPair);

            var clientKeyPair = Services.Crypto.GenerateX25519KeyPair();
            var serverKeyPair = Services.Crypto.GenerateX25519KeyPair();
            (clientKeyPair, serverKeyPair) = ExchangePublicKeys(
                clientKeyPair,
                serverKeyPair);

            var dataId = Guid.NewGuid();
            var syncId = Guid.NewGuid();
            var version = Guid.NewGuid();
            var dbData = new Data
            {
                DataId = dataId,
                Version = version,
                Binary = [],
                LastAccessedAt = 0
            };

            var dbSync = new Sync
            {
                SyncId = syncId,
                DataId = dataId,
                CreatedAt = 0,
                OneTimeKeyPair = new KeyPair
                {
                    Private = oneTimeServerKeyPair.Private.ToString(),
                    Public = oneTimeServerKeyPair.Public.ToString()
                }
            };

            DataRepository.Create(dbData);
            SyncRepository.Create(dbSync);

            var requestPayload = new ConsumeRequest.Payload
            {
                ClientPublicKey = clientKeyPair.Public,
                Timestamp = Services.Clock.Timestamp
            };
            var requestBody = Services.Crypto.Encrypt(
                requestPayload.ToJson().ToUtfBytes(),
                oneTimeClientKeyPair);
            var request = new ConsumeRequest
            {
                SyncId = syncId,
                Body = requestBody
            };

            // Call API
            var response = api.OnConsume(request);

            Assert.IsNull(response.Result);
            Assert.IsNotNull(response.Error);

            Assert.AreEqual(ErrorMessages.SyncExpired, response.Error);
        }

        [TestMethod]
        public void ConsumeCantFindSyncTest()
        {
            var api = new ApiV1();

            var clock = new MockClock();
            ServicesMock.Override<IClock>(clock);
            clock.UniversalTime = DateTimeOffset.FromUnixTimeMilliseconds(255);

            var oneTimeClientKeyPair = Services.Crypto.GenerateX25519KeyPair();
            var oneTimeServerKeyPair = Services.Crypto.GenerateX25519KeyPair();
            (oneTimeClientKeyPair, oneTimeServerKeyPair) = ExchangePublicKeys(
                oneTimeClientKeyPair,
                oneTimeServerKeyPair);

            var clientKeyPair = Services.Crypto.GenerateX25519KeyPair();
            var serverKeyPair = Services.Crypto.GenerateX25519KeyPair();
            (clientKeyPair, serverKeyPair) = ExchangePublicKeys(
                clientKeyPair,
                serverKeyPair);

            var dataId = Guid.NewGuid();
            var syncId = Guid.NewGuid();
            var version = Guid.NewGuid();
            var dbData = new Data
            {
                DataId = dataId,
                Version = version,
                Binary = [],
                LastAccessedAt = 0
            };

            // Don't create sync
            DataRepository.Create(dbData);

            var requestPayload = new ConsumeRequest.Payload
            {
                ClientPublicKey = clientKeyPair.Public,
                Timestamp = Services.Clock.Timestamp
            };
            var requestBody = Services.Crypto.Encrypt(
                requestPayload.ToJson().ToUtfBytes(),
                oneTimeClientKeyPair);
            var request = new ConsumeRequest
            {
                SyncId = syncId,
                Body = requestBody
            };

            // Call API
            var response = api.OnConsume(request);

            Assert.IsNull(response.Result);
            Assert.IsNotNull(response.Error);

            Assert.AreEqual(ErrorMessages.CantFindSync, response.Error);
        }

        [TestMethod]
        public void ConsumeCantDecryptPayloadTest()
        {
            var api = new ApiV1();

            var clock = new MockClock();
            ServicesMock.Override<IClock>(clock);
            clock.UniversalTime = DateTimeOffset.FromUnixTimeMilliseconds(255);

            var oneTimeClientKeyPair = Services.Crypto.GenerateX25519KeyPair();
            var oneTimeServerKeyPair = Services.Crypto.GenerateX25519KeyPair();
            (oneTimeClientKeyPair, oneTimeServerKeyPair) = ExchangePublicKeys(
                oneTimeClientKeyPair,
                oneTimeServerKeyPair);

            var clientKeyPair = Services.Crypto.GenerateX25519KeyPair();
            var serverKeyPair = Services.Crypto.GenerateX25519KeyPair();
            (clientKeyPair, serverKeyPair) = ExchangePublicKeys(
                clientKeyPair,
                serverKeyPair);

            var dataId = Guid.NewGuid();
            var syncId = Guid.NewGuid();
            var version = Guid.NewGuid();
            var dbData = new Data
            {
                DataId = dataId,
                Version = version,
                Binary = [],
                LastAccessedAt = 0
            };

            var dbSync = new Sync
            {
                SyncId = syncId,
                DataId = dataId,
                CreatedAt = ServicesMock.Get<IClock>().Timestamp,
                OneTimeKeyPair = new KeyPair
                {
                    Private = oneTimeServerKeyPair.Private.ToString(),
                    Public = oneTimeServerKeyPair.Public.ToString()
                }
            };

            DataRepository.Create(dbData);
            SyncRepository.Create(dbSync);

            var requestPayload = new ConsumeRequest.Payload
            {
                ClientPublicKey = clientKeyPair.Public,
                Timestamp = Services.Clock.Timestamp
            };

            // Encrypt with the wrong key
            var requestBody = Services.Crypto.Encrypt(
                requestPayload.ToJson().ToUtfBytes(),
                clientKeyPair);
            var request = new ConsumeRequest
            {
                SyncId = syncId,
                Body = requestBody
            };

            // Call API
            var response = api.OnConsume(request);

            Assert.IsNull(response.Result);
            Assert.IsNotNull(response.Error);

            Assert.AreEqual(ErrorMessages.CantDecryptPayload, response.Error);
        }

        [TestMethod]
        public void ConsumeCantVerifyClockTest()
        {
            var api = new ApiV1();

            var clock = new MockClock();
            ServicesMock.Override<IClock>(clock);
            clock.UniversalTime = DateTimeOffset.FromUnixTimeMilliseconds(0);

            var oneTimeClientKeyPair = Services.Crypto.GenerateX25519KeyPair();
            var oneTimeServerKeyPair = Services.Crypto.GenerateX25519KeyPair();
            (oneTimeClientKeyPair, oneTimeServerKeyPair) = ExchangePublicKeys(
                oneTimeClientKeyPair,
                oneTimeServerKeyPair);

            var clientKeyPair = Services.Crypto.GenerateX25519KeyPair();
            var serverKeyPair = Services.Crypto.GenerateX25519KeyPair();
            (clientKeyPair, serverKeyPair) = ExchangePublicKeys(
                clientKeyPair,
                serverKeyPair);

            var dataId = Guid.NewGuid();
            var syncId = Guid.NewGuid();
            var version = Guid.NewGuid();
            var dbData = new Data
            {
                DataId = dataId,
                Version = version,
                Binary = [],
                LastAccessedAt = 0
            };

            var dbSync = new Sync
            {
                SyncId = syncId,
                DataId = dataId,
                CreatedAt = ServicesMock.Get<IClock>().Timestamp,
                OneTimeKeyPair = new KeyPair
                {
                    Private = oneTimeServerKeyPair.Private.ToString(),
                    Public = oneTimeServerKeyPair.Public.ToString()
                }
            };

            DataRepository.Create(dbData);
            SyncRepository.Create(dbSync);

            var requestPayload = new ConsumeRequest.Payload
            {
                ClientPublicKey = clientKeyPair.Public,
                Timestamp = Services.Clock.Timestamp
            };
            var requestBody = Services.Crypto.Encrypt(
                requestPayload.ToJson().ToUtfBytes(),
                oneTimeClientKeyPair);
            var request = new ConsumeRequest
            {
                SyncId = syncId,
                Body = requestBody
            };

            // Set a later time
            clock.UniversalTime = DateTimeOffset.FromUnixTimeSeconds(31);

            // Call API
            var response = api.OnConsume(request);

            Assert.IsNull(response.Result);
            Assert.IsNotNull(response.Error);

            Assert.AreEqual(ErrorMessages.CantVerifyClock, response.Error);
        }
    }
}
