using Authi.App.Logic;
using Authi.App.Logic.Services;
using Authi.App.WinUI.Extensions;
using Authi.App.WinUI.UI;
using Authi.Common.Services;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Media;
using System;
using System.Runtime.InteropServices;
using WinUIEx;
using L10n = Authi.App.Logic.Localization;

namespace Authi.App.WinUI
{
    public partial class App
    {
        public static new App Current => (App)Application.Current;

        public WindowEx MainWindow => _mainWindow!;
        private WindowEx? _mainWindow;

        public App()
        {
            ServiceLocator.Init(
                typeof(ServiceLocator).Assembly,    // Authi.Common
                typeof(Config).Assembly,            // Authi.App.Logic
                typeof(App).Assembly);              // Authi.App
            InitializeComponent();
        }

        public T GetResource<T>(string key) => (T)GetResource(key);

        public object GetResource(string key) => Resources[key];

        public bool IsForeground
        {
            get
            {
                var foregroundWindowHandle = GetForegroundWindow();
                var currentWindowHandle = WinRT.Interop.WindowNative.GetWindowHandle(MainWindow);
                return foregroundWindowHandle == currentWindowHandle;
            }
        }

        protected override void OnLaunched(LaunchActivatedEventArgs args)
        {
            _mainWindow = new WindowEx
            {
                WindowContent = new MainPage(),
                Width = 520,
                Height = 640,
                MinWidth = 340,
                MinHeight = 340,
                ExtendsContentIntoTitleBar = true,
                SystemBackdrop = new MicaBackdrop(),
                Title = L10n.Generic.AppName
            }.WithIcon("ms-appx:///Assets/AppIcon.ico");
            _mainWindow.Activated += OnActivated;
            _mainWindow.Activate();
        }

        private void OnActivated(object sender, WindowActivatedEventArgs args)
        {
            if (args.WindowActivationState != WindowActivationState.Deactivated)
            {
                ServiceProvider.Current.Get<IMessenger>().SyncNow.Publish(this);
            }
        }

        [DllImport("user32.dll")]
        private static extern IntPtr GetForegroundWindow();
    }
}
