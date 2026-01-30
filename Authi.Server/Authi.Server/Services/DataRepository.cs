using Authi.Common.Services;
using Authi.Server.Models;
using System;

namespace Authi.Server.Services
{
    [Service]
    internal interface IDataRepository
    {
        void Create(Data data);
        void Delete(Data data);
        Data? Read(Guid id);
        void Update(Data data);
    }

    internal class DataRepository : ServiceBase, IDataRepository
    {
        public void Create(Data data)
        {
            Services.AppDbContext.Insert(data);
        }

        public Data? Read(Guid id)
        {
            return Services.AppDbContext.Find<Data>(id);
        }

        public void Update(Data data)
        {
            Services.AppDbContext.Update(data);
        }

        public void Delete(Data data)
        {
            Services.AppDbContext.Delete(data);
        }
    }
}
