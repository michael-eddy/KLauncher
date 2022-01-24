using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Widget;
using AndroidX.AppCompat.App;
using Xamarin.Essentials;

namespace KLauncher
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme", MainLauncher = true,
        ConfigurationChanges = ConfigChanges.Locale | ConfigChanges.Navigation | ConfigChanges.Orientation)]
    [IntentFilter(new[] { "android.intent.action.MAIN" }, Categories = new[] { "android.intent.category.HOME", "android.intent.category.DEFAULT", "android.intent.category.LAUNCHER" })]
    public class MainActivity : AppCompatActivity
    {
        private ListView AppList { get; set; }
        private PackageReceiver PackageReceiver { get; set; }
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Platform.Init(this, savedInstanceState);
            SetContentView(Resource.Layout.activity_main);

            AppList = FindViewById<ListView>(Resource.Id.appList);
            InitViewStyle();
        }
        private void InitViewStyle()
        {
            int marginTop = 0, marginBottom = 0;
            int resourceId = ApplicationContext.Resources.GetIdentifier("status_bar_height", "dimen", "android");
            if (resourceId > 0)
                marginTop = ApplicationContext.Resources.GetDimensionPixelSize(resourceId);
            resourceId = ApplicationContext.Resources.GetIdentifier("navigation_bar_height", "dimen", "android");
            if (resourceId > 0)
                marginBottom = ApplicationContext.Resources.GetDimensionPixelSize(resourceId);
            AppList.SetPadding(0, marginTop, 0, marginBottom);
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