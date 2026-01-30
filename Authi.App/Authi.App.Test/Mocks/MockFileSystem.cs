using System.IO;
using System.Threading.Tasks;
using IFileSystem = Authi.App.Logic.Services.IFileSystem;

namespace Authi.App.Test.Mocks
{
    internal class FileSystem : IFileSystem
    {
        public string AppDataDirectory => Path.GetTempPath();

        public Task<Stream?> ReadFromPickerAsync() => Task.FromResult<Stream?>(null);

        public Task<bool> WriteToPickerAsync(Stream stream, string? suggestedFileName = null) => Task.FromResult(false);
    }
}
