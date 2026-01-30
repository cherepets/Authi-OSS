using Authi.App.Logic;
using Authi.App.Logic.Localization;
using Authi.App.Logic.Services;
using Authi.App.Maui.Converters;
using Authi.App.Maui.Extensions;
using Authi.App.Maui.Styles;
using Authi.App.Maui.UI;
using Authi.Common.Extensions;
using Authi.Common.Services;
using MaterialColorUtilities.Maui;
using Microsoft.Maui;
using Microsoft.Maui.ApplicationModel;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Devices;
using System.Diagnostics;

namespace Authi.App.Maui
{
    public interface IMauiApp
    {
        bool IsDarkMode { get; }
        Thickness SystemInsets { get; set; }
        T GetResource<T>(string key);
        object GetResource(string key);
        T GetThemedResource<T>(string key);
        object GetThemedResource(string key);
        void OpenMainPage();
    }

    public partial class MauiApp : Application, IMauiApp
    {
        public static new IMauiApp Current => (MauiApp)Application.Current;

        public bool IsDarkMode => UserAppTheme == AppTheme.Dark;

        public Thickness SystemInsets { get; set; }

        private readonly Shell _shell;

        public MauiApp()
        {
            UserAppTheme = RequestedTheme == AppTheme.Dark ? AppTheme.Dark : AppTheme.Light;

            ServiceLocator.Init(
                typeof(ServiceLocator).Assembly,    // Authi.Common
                typeof(Config).Assembly,            // Authi.App.Logic
                typeof(MauiApp).Assembly);          // Authi.App

            Resources
                // Colors
                .Merge<XamarinColors>()
                .Merge<CustomColors>()
                .Merge<MaterialColorResourceDictionary>()
                // Styles
                .Merge<XamarinStyles>()
                .Merge<MaterialIcons>()
                .Merge<ButtonStyles>()
                .Merge<LabelStyles>()
                .Merge<SwitchStyles>()
                // Converters
                .Merge<ConvertersDictionary>();

            MainPage = _shell = new Shell { FlyoutBehavior = FlyoutBehavior.Disabled };
            OpenMainPage();
        }

        public T GetResource<T>(string key) => (T)GetResource(key);

        public object GetResource(string key) => Resources[key];

        public T GetThemedResource<T>(string key) => (T)GetThemedResource(key);

        public object GetThemedResource(string key) => GetResource(key + (IsDarkMode ? "Dark" : "Light"));

        public void OpenMainPage()
        {
            _shell.Items.Clear();
            _shell.Items.Add(new MainPage());
        }

        protected override Window CreateWindow(IActivationState activationState)
        {
            return base.CreateWindow(activationState).Customize(window =>
            {
                if (DeviceInfo.Current.Platform == DevicePlatform.WinUI)
                {
                    var displayInfo = DeviceDisplay.Current.MainDisplayInfo;

                    window.Title = Generic.AppName;

                    window.MinimumWidth = 340;
                    window.MinimumHeight = 340;

                    window.Width = 520;
                    window.Height = 640;

                    window.X = (displayInfo.Width / displayInfo.Density - window.Width) / 2;
                    window.Y = (displayInfo.Height / displayInfo.Density - window.Height) / 2;
                }
            });
        }

        protected override void OnResume()
        {
            ServiceProvider.Current.Get<IMessenger>().SyncNow.Publish(this);
        }
    }
}
