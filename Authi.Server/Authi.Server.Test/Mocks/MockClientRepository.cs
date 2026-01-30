using Authi.Server.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Authi.Server.Services
{
    public class MockClientRepository : IClientRepository
    {
        private Dictionary<Guid, Client> _storage = [];

        public void Create(Client client)
        {
            _storage.Add(client.ClientId, client);
        }

        public Client? Read(Guid id)
        {
            return _storage.TryGetValue(id, out var value)
                ? value
                : null;
        }

        public void Update(Client client)
        {
            _storage[client.ClientId] = client;
        }

        public void Delete(Client client)
        {
            _storage.Remove(client.ClientId);
        }

        public void Initialize(params Client[] records)
        {
            _storage = records.ToDictionary(x => x.ClientId);
        }

        public Dictionary<Guid, Client> AsDictionary()
        {
            return _storage;
        }
    }
}
