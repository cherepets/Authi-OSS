using Authi.App.Logic.Services;
using System;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using Windows.Security.Cryptography.DataProtection;
using Windows.Storage;

namespace Authi.App.WinUI.Services
{
    internal class SecureStorage : ISecureStorage
    {
        public async Task<string?> GetAsync(string key)
        {
            var encBytes = await GetBytesAsync(key);

            if (encBytes == null)
                return null;

            var provider = new DataProtectionProvider();

            var buffer = await provider.UnprotectAsync(encBytes.AsBuffer());

            return Encoding.UTF8.GetString(buffer.ToArray());
        }

        public async Task SetAsync(string key, string value)
        {
            var bytes = Encoding.UTF8.GetBytes(value);

            // LOCAL=user and LOCAL=machine do not require enterprise auth capability
            var provider = new DataProtectionProvider("LOCAL=user");

            var buffer = await provider.ProtectAsync(bytes.AsBuffer());

            var encBytes = buffer.ToArray();

            await SetBytesAsync(key, encBytes);
        }

        public void Remove(string key)
        {
            var settings = GetSettings();
            settings.Values.Remove(key);
        }

        public void RemoveAll()
        {
            var settings = GetSettings();
            settings.Values.Clear();
        }

        static ApplicationDataContainer GetSettings()
        {
            var name = Logic.Localization.Generic.AppName;
            var localSettings = ApplicationData.Current.LocalSettings;
            if (!localSettings.Containers.ContainsKey(name))
                localSettings.CreateContainer(name, ApplicationDataCreateDisposition.Always);
            return localSettings.Containers[name];
        }

        private Task<byte[]?> GetBytesAsync(string key)
        {
            var settings = GetSettings();
            var encBytes = settings.Values[key] as byte[];
            return Task.FromResult(encBytes);
        }

        private Task SetBytesAsync(string key, byte[] data)
        {
            var settings = GetSettings();
            settings.Values[key] = data;
            return Task.CompletedTask;
        }
    }
}
