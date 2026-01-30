using Camera.MAUI;
using CommunityToolkit.Maui;
using MaterialColorUtilities.Maui;
using Microsoft.Maui.Controls.Hosting;
using Microsoft.Maui.Hosting;
using SkiaSharp.Views.Maui.Controls.Hosting;
using System;

namespace Authi.App.Maui
{
    public static class MauiProgram
    {
        public static Microsoft.Maui.Hosting.MauiApp CreateMauiApp()
        {
            AppContext.SetSwitch("System.Reflection.NullabilityInfoContext.IsSupported", true);
            var builder = Microsoft.Maui.Hosting.MauiApp.CreateBuilder();
            builder
                .UseMaterialColors()
                .UseMauiApp<MauiApp>()
                .UseMauiCommunityToolkit()
                .UseSkiaSharp()
                .UseMauiCameraView()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("MajorMonoDisplay.ttf", "MajorMonoDisplay");
                    fonts.AddFont("NotoSans.ttf", "DefaultFont");
                    fonts.AddFont("MaterialSymbols.ttf", "IconFont");
                });
            return builder.Build();
        }
    }
}
