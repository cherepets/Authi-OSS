using Authi.App.Logic.Data;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Authi.App.Logic.Services
{
    internal interface ICredentialStorage
    {
        Task InsertAsync(Credential data);
        Task UpdateAsync(Credential data);
        Task DeleteAsync(Credential data);
        Task<IReadOnlyCollection<Credential>> GetAllAsync();
    }
}
