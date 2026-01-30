using Authi.App.Logic.Services;
using Microsoft.Maui.ApplicationModel;
using System;
using System.Threading.Tasks;

namespace Authi.App.Maui.Services
{
    internal class LinkOpener : ILinkOpener
    {
        public async Task OpenUriAsync(string uri)
        {
            await Browser.Default.OpenAsync(new Uri(uri), BrowserLaunchMode.External);
        }
    }
}
