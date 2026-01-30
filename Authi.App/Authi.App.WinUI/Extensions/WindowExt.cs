using Microsoft.UI.Xaml;
using System;
using Windows.Storage;

namespace Authi.App.WinUI.Extensions
{
    public static class WindowExt
    {
        public static T WithIcon<T>(this T window, string iconPath) where T : Window
        {
            window.SetIcon(iconPath);
            return window;
        }

        public static async void SetIcon<T>(this T window, string iconPath) where T : Window
        {
            if (await StorageFile.GetFileFromApplicationUriAsync(new Uri(iconPath)) is StorageFile icon)
            {
                window.AppWindow.SetIcon(icon.Path);
            }
        }
    }
}
