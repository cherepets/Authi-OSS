using Authi.Common.Services;
using Authi.Server.Models;
using System;

namespace Authi.Server.Services
{
    [Service]
    internal interface IClientRepository
    {
        void Create(Client client);
        void Delete(Client client);
        Client? Read(Guid id);
        void Update(Client client);
    }

    internal class ClientRepository : ServiceBase, IClientRepository
    {
        public void Create(Client client)
        {
            Services.AppDbContext.Insert(client);
        }

        public Client? Read(Guid id)
        {
            return Services.AppDbContext.Find<Client>(id);
        }

        public void Update(Client client)
        {
            Services.AppDbContext.Update(client);
        }

        public void Delete(Client client)
        {
            Services.AppDbContext.Delete(client);
        }
    }
}
