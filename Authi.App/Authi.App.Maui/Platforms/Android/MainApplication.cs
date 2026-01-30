using Android.App;
using Android.Runtime;
using Authi.App.Maui;
using Microsoft.Maui;
using System;

namespace Authi
{
    [Application]
    public class MainApplication(IntPtr handle, JniHandleOwnership ownership) : MauiApplication(handle, ownership)
    {
        protected override Microsoft.Maui.Hosting.MauiApp CreateMauiApp()
        {
            return MauiProgram.CreateMauiApp();
        }
    }
}
