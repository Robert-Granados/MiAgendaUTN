using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Print.Pdf;

namespace MiAgendaUTN
{
    [Activity(Theme = "@style/Maui.SplashTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density)]
    public class MainActivity : MauiAppCompatActivity
    {
#if DEBUG
        protected override void OnCreate(Bundle? savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            if (Intent?.GetBooleanExtra("compatibility-smoke", false) == true)
                CompatibilitySmoke.Run();
        }
#endif
    }
}
