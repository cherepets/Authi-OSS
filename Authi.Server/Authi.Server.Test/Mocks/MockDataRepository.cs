using Authi.Server.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Authi.Server.Services
{
    public class MockDataRepository : IDataRepository
    {
        private Dictionary<Guid, Data> _storage = [];

        public void Create(Data data)
        {
            _storage.Add(data.DataId, data);
        }

        public Data? Read(Guid id)
        {
            return _storage.TryGetValue(id, out var value)
                ? value
                : null;
        }

        public void Update(Data data)
        {
            _storage[data.DataId] = data;
        }

        public void Delete(Data data)
        {
            _storage.Remove(data.DataId);
        }

        public void Initialize(params Data[] records)
        {
            _storage = records.ToDictionary(x => x.DataId);
        }

        public Dictionary<Guid, Data> AsDictionary()
        {
            return _storage;
        }
    }
}
