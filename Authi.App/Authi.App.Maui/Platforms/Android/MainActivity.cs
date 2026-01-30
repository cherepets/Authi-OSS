using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Views;
using AndroidX.Core.View;
using Authi.App.Maui;
using Microsoft.Maui;
using Microsoft.Maui.ApplicationModel;

namespace Authi
{
    [Activity(Theme = "@style/Maui.SplashTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density)]
    public partial class MainActivity : MauiAppCompatActivity, IOnApplyWindowInsetsListener
    {
        public WindowInsetsCompat OnApplyWindowInsets(View v, WindowInsetsCompat insets)
        {
            var stableInsets = insets.GetInsetsIgnoringVisibility(WindowInsetsCompat.Type.SystemBars());
            MauiApp.Current.SystemInsets = new Thickness
            {
                Bottom = stableInsets.Bottom / Resources.DisplayMetrics.Density,
                Left = stableInsets.Left / Resources.DisplayMetrics.Density,
                Right = stableInsets.Right / Resources.DisplayMetrics.Density,
                Top = stableInsets.Top / Resources.DisplayMetrics.Density
            };
            return insets;
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            MauiHandlers.Initialize();

            if (!MauiApp.Current.IsDarkMode)
            {
                Window.InsetsController?.SetSystemBarsAppearance(
                    (int)WindowInsetsControllerAppearance.LightStatusBars,
                    (int)WindowInsetsControllerAppearance.LightStatusBars);
            }

            WindowCompat.SetDecorFitsSystemWindows(Window, false);
            var rootView = FindViewById(Android.Resource.Id.Content);
            ViewCompat.SetOnApplyWindowInsetsListener(rootView, this);

            Platform.CurrentActivity.Window.AddFlags(WindowManagerFlags.LayoutNoLimits);
            Platform.CurrentActivity.Window.AddFlags(WindowManagerFlags.TranslucentStatus);
            Platform.CurrentActivity.Window.SetStatusBarColor(Android.Graphics.Color.Transparent);
        }
    }
}
