using Authi.Common.Services;
using Authi.Server.Models;
using System;

namespace Authi.Server.Services
{
    [Service]
    internal interface ISyncRepository
    {
        void Create(Sync sync);
        void Delete(Sync sync);
        Sync? Read(Guid id);
        void Update(Sync sync);
    }

    internal class SyncRepository : ServiceBase, ISyncRepository
    {
        public void Create(Sync sync)
        {
            Services.AppDbContext.Insert(sync);
        }

        public Sync? Read(Guid id)
        {
            return Services.AppDbContext.Find<Sync>(id);
        }

        public void Update(Sync sync)
        {
            Services.AppDbContext.Update(sync);
        }

        public void Delete(Sync sync)
        {
            Services.AppDbContext.Delete(sync);
        }
    }
}
