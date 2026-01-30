using Authi.App.Maui;

namespace Authi.WinUI
{
    public partial class App
    {
        public App()
        {
            InitializeComponent();
        }

        protected override Microsoft.Maui.Hosting.MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();
    }
}
