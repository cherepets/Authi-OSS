using Authi.Common.Dto;
using Authi.Common.Extensions;
using Authi.Common.Services;
using Authi.Common.Test.Mocks;
using Authi.Server.ApiVersions;
using System;

namespace Authi.Server.Test
{
    [TestClass]
    public class InitTests : ServerTestsBase
    {
        [TestMethod]
        public void InitHappyTest()
        {
            var api = new ApiV1();

            var clock = new MockClock();
            ServicesMock.Override<IClock>(clock);
            clock.UniversalTime = DateTimeOffset.FromUnixTimeMilliseconds(255);

            var clientKeyPair = Services.Crypto.GenerateX25519KeyPair();
            var requestPayload = new InitRequest.Payload
            {
                Timestamp = Services.Clock.Timestamp
            };
            var requestBody = Services.Crypto.Encrypt(
                requestPayload.ToJson().ToUtfBytes(),
                new AesKey(clientKeyPair.Public));
            var request = new InitRequest
            {
                ClientPublicKey = clientKeyPair.Public,
                Body = requestBody
            };

            // Call API
            var response = api.OnInit(request);

            Assert.IsNull(response.Error);
            Assert.IsNotNull(response.Result);

            var keyPair = new X25519KeyPair(
                clientKeyPair.Private,
                new X25519PublicKey(response.Result.ServerPublicKey));
            var responseBody = Services.Crypto.Decrypt(
                response.Result.Body,
                keyPair);
            var responsePayload = responseBody.ToUtfString().FromJson<InitResponse.Payload>();

            Assert.IsNotNull(responsePayload);
            Assert.AreEqual(255, responsePayload.Timestamp);
            Assert.AreNotEqual(Guid.Empty, responsePayload.ClientId);

            Assert.AreEqual(1, ClientRepository.AsDictionary().Count);
            Assert.AreEqual(1, DataRepository.AsDictionary().Count);

            var clientRecord = ClientRepository.Read(responsePayload.ClientId);
            Assert.IsNotNull(clientRecord);

            var dataRecord = DataRepository.Read(clientRecord.DataId);
            Assert.IsNotNull(dataRecord);

            Assert.AreEqual(clientRecord.DataId, dataRecord.DataId);
        }

        [TestMethod]
        public void InitCantParseClientPublicKeyTest()
        {
            var api = new ApiV1();

            var clientKeyPair = Services.Crypto.GenerateX25519KeyPair();
            var requestPayload = new InitRequest.Payload
            {
                Timestamp = Services.Clock.Timestamp
            };
            var requestBody = Services.Crypto.Encrypt(
                requestPayload.ToJson().ToUtfBytes(),
                new AesKey(clientKeyPair.Public));

            // Break the public key
            var request = new InitRequest
            {
                ClientPublicKey = clientKeyPair.Public.Bytes[16..].ToArray(),
                Body = requestBody
            };

            // Call API
            var response = api.OnInit(request);

            Assert.IsNull(response.Result);
            Assert.IsNotNull(response.Error);

            Assert.AreEqual(ErrorMessages.CantParseClientPublicKey, response.Error);
        }

        [TestMethod]
        public void InitCantDecryptPayloadTest()
        {
            var api = new ApiV1();

            var clientKeyPair = Services.Crypto.GenerateX25519KeyPair();
            var requestPayload = new InitRequest.Payload
            {
                Timestamp = Services.Clock.Timestamp
            };

            // Encrypt with the wrong key
            var requestBody = Services.Crypto.Encrypt(
                requestPayload.ToJson().ToUtfBytes(),
                new AesKey(clientKeyPair.Private));
            var request = new InitRequest
            {
                ClientPublicKey = clientKeyPair.Public,
                Body = requestBody
            };

            // Call API
            var response = api.OnInit(request);

            Assert.IsNull(response.Result);
            Assert.IsNotNull(response.Error);

            Assert.AreEqual(ErrorMessages.CantDecryptPayload, response.Error);
        }

        [TestMethod]
        public void InitCantVerifyClockTest()
        {
            var api = new ApiV1();

            var clock = new MockClock();
            ServicesMock.Override<IClock>(clock);
            clock.UniversalTime = DateTimeOffset.FromUnixTimeSeconds(0);

            var clientKeyPair = Services.Crypto.GenerateX25519KeyPair();
            var requestPayload = new InitRequest.Payload
            {
                Timestamp = Services.Clock.Timestamp
            };
            var requestBody = Services.Crypto.Encrypt(
                requestPayload.ToJson().ToUtfBytes(),
                new AesKey(clientKeyPair.Public));
            var request = new InitRequest
            {
                ClientPublicKey = clientKeyPair.Public,
                Body = requestBody
            };

            // Set a later time
            clock.UniversalTime = DateTimeOffset.FromUnixTimeSeconds(31);

            // Call API
            var response = api.OnInit(request);

            Assert.IsNull(response.Result);
            Assert.IsNotNull(response.Error);

            Assert.AreEqual(ErrorMessages.CantVerifyClock, response.Error);
        }
    }
}
