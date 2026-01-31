using Authi.App.Logic.Services;
using System.Threading.Tasks;

namespace Authi.App.Test.Mocks
{
    internal class MockLinkOpener : ILinkOpener
    {
        public Task OpenUriAsync(string uri)
        {
            return Task.CompletedTask;
        }
    }
}
