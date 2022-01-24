using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Views;
using AndroidX.AppCompat.App;
using Xamarin.Essentials;

namespace KLauncher
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme", MainLauncher = true,
        ConfigurationChanges = ConfigChanges.Locale | ConfigChanges.Navigation | ConfigChanges.Orientation)]
    [IntentFilter(new[] { "android.intent.action.MAIN" }, Categories = new[] { "android.intent.category.HOME", "android.intent.category.DEFAULT", "android.intent.category.LAUNCHER" })]
    public sealed class MainActivity : AppCompatActivity
    {
        private PackageReceiver PackageReceiver { get; set; }
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Platform.Init(this, savedInstanceState);
            SetContentView(Resource.Layout.activity_main);
        }
        public override bool DispatchKeyEvent(KeyEvent e)
        {
            switch (e.KeyCode)
            {
                case Keycode.PageUp:
                    {

                        break;
                    }
                case Keycode.PageDown:
                    {

                        break;
                    }
                case Keycode.Enter:
                    {
                        Intent intent = new Intent(this, typeof(AppActivity));
                        StartActivity(intent);
                        break;
                    }
            }
            return base.DispatchKeyEvent(e);
        }
        protected override void OnStart()
        {
            base.OnStart();
            PackageReceiver = new PackageReceiver();
            IntentFilter filter = new IntentFilter();
            filter.AddAction("android.intent.action.PACKAGE_ADDED");
            filter.AddAction("android.intent.action.PACKAGE_REMOVED");
            filter.AddDataScheme("package");
            RegisterReceiver(PackageReceiver, filter);
        }
        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, Permission[] grantResults)
        {
            Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);
            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }
        protected override void OnDestroy()
        {
            if (PackageReceiver != null)
                UnregisterReceiver(PackageReceiver);
            base.OnDestroy();
        }
    }
}