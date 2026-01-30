using Authi.App.Logic.Services;
using System;
using System.Threading.Tasks;
using Windows.System;

namespace Authi.App.WinUI.Services
{
    internal class LinkOpener : ILinkOpener
    {
        public async Task OpenUriAsync(string uri)
        {
            await Launcher.LaunchUriAsync(new Uri(uri));
        }
    }
}
