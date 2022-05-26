using Android.App;
using Android.OS;
using Xamarin.Essentials;

namespace KLauncher
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme")]
    public class AboutActivity : Activity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Platform.Init(this, savedInstanceState);
            SetContentView(Resource.Layout.about_view);
        }
    }
}