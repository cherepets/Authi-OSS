using System.Threading.Tasks;
using IClipboard = Authi.App.Logic.Services.IClipboard;

namespace Authi.App.Test.Mocks
{
    internal class Clipboard : IClipboard
    {
        private string? _text;

        public Task<string?> GetTextAsync() => Task.FromResult(_text);

        public Task SetTextAsync(string? text)
        {
            _text = text;
            return Task.CompletedTask;
        }
    }
}
