using Authi.Common.Client;
using Authi.Common.Client.Results;
using Authi.Common.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Client = Authi.Common.Client.ApiClient;

namespace Authi.App.Logic.Services
{
    [Service]
    internal interface IApiClient
    {
        Task<ConsumeResult> ConsumeAsync(SyncCode syncCode);
        Task<InitResult> InitAsync();
        Task<PublishResult> PublishAsync(Guid clientId, X25519KeyPair syncKeyPair);
        Task<ReadResult> ReadAsync(Guid clientId, Guid version, AesKey dataKey, X25519KeyPair syncKeyPair);
        Task<WriteResult> WriteAsync(IReadOnlyCollection<CredentialDto> credentials, Guid clientId, AesKey dataKey, X25519KeyPair syncKeyPair);
    }

    internal class ApiClient : ServiceBase, IApiClient
    {
        private readonly Lazy<Client> _client;

        public ApiClient()
        {
            _client = new Lazy<Client>(() => new Client(
                Services.Clock,
                Services.Crypto));
        }

        public Task<ConsumeResult> ConsumeAsync(SyncCode syncCode)
            => _client.Value.ConsumeAsync(syncCode);

        public Task<InitResult> InitAsync()
            => _client.Value.InitAsync();

        public Task<PublishResult> PublishAsync(Guid clientId, X25519KeyPair syncKeyPair)
            => _client.Value.PublishAsync(clientId, syncKeyPair);

        public Task<ReadResult> ReadAsync(Guid clientId, Guid version, AesKey dataKey, X25519KeyPair syncKeyPair)
            => _client.Value.ReadAsync(clientId, version, dataKey, syncKeyPair);

        public Task<WriteResult> WriteAsync(IReadOnlyCollection<CredentialDto> credentials, Guid clientId, AesKey dataKey, X25519KeyPair syncKeyPair)
            => _client.Value.WriteAsync(credentials, clientId, dataKey, syncKeyPair);
    }
}
