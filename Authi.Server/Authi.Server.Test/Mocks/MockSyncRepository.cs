using Authi.Common.Services;
using Authi.Server.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Authi.Server.Services
{
    public class MockSyncRepository : ISyncRepository
    {
        private Dictionary<Guid, Sync> _storage = [];

        public void Create(Sync sync)
        {
            if (ServiceProvider.Current.Get<IDataRepository>().Read(sync.DataId) is null)
            {
                throw new Exception($"Data with id {sync.DataId} not found.");
            }

            _storage.Add(sync.SyncId, sync);
        }

        public Sync? Read(Guid id)
        {
            return _storage.TryGetValue(id, out var value)
                ? value
                : null;
        }

        public void Update(Sync sync)
        {
            _storage[sync.SyncId] = sync;
        }

        public void Delete(Sync sync)
        {
            _storage.Remove(sync.SyncId);
        }

        public void Initialize(params Sync[] records)
        {
            _storage = records.ToDictionary(x => x.SyncId);
        }

        public Dictionary<Guid, Sync> AsDictionary()
        {
            return _storage;
        }
    }
}
