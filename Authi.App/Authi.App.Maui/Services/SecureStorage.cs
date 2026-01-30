using Authi.App.Logic.Services;
using System.Threading.Tasks;
using MauiSecureStorage = Microsoft.Maui.Storage.SecureStorage;

namespace Authi.App.Maui.Services
{
    internal class SecureStorage : ISecureStorage
    {
        public Task<string> GetAsync(string key)
            => MauiSecureStorage.GetAsync(key);

        public void Remove(string key)
            => MauiSecureStorage.Remove(key);

        public void RemoveAll()
            => MauiSecureStorage.RemoveAll();

        public Task SetAsync(string key, string value)
            => MauiSecureStorage.SetAsync(key, value);
    }
}
