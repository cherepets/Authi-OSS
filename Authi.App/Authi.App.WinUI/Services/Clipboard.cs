using Authi.App.Logic.Services;
using System;
using System.Threading.Tasks;
using Windows.ApplicationModel.DataTransfer;
using WindowsClipboard = Windows.ApplicationModel.DataTransfer.Clipboard;

namespace Authi.App.WinUI.Services
{
    internal class Clipboard : IClipboard
    {
        public Task<string?> GetTextAsync()
        {
            var clipboardContent = WindowsClipboard.GetContent();
            return clipboardContent.Contains(StandardDataFormats.Text)
                ? clipboardContent.GetTextAsync().AsTask()
                : Task.FromResult<string?>(null);
        }

        public Task SetTextAsync(string? text)
        {
            var dataPackage = new DataPackage();
            dataPackage.SetText(text);
            WindowsClipboard.SetContent(dataPackage);
            return Task.CompletedTask;
        }
    }
}
