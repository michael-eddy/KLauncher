using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Views;
using Android.Widget;
using KLauncher.Libs;
using KLauncher.Libs.Client;
using KLauncher.Tasks;
using System;
using System.Linq;
using Xamarin.Essentials;

namespace KLauncher
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme", MainLauncher = true,
        ConfigurationChanges = ConfigChanges.Locale | ConfigChanges.Navigation | ConfigChanges.Orientation)]
    [IntentFilter(new[] { "android.intent.action.MAIN" }, Categories = new[] { "android.intent.category.HOME", "android.intent.category.DEFAULT", "android.intent.category.LAUNCHER" })]
    public sealed class MainActivity : BaseActivity
    {
        private WeatherClient Weather { get; set; }
        private PackageReceiver PackageReceiver { get; set; }
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Platform.Init(this, savedInstanceState);
            SetContentView(Resource.Layout.activity_main);
            AppCenter.Instance.UpdateList();
            RunOnUiThread(() =>
            {
                InitControls();
                InitWeatherInfo();
            });
        }
        private TextView TextViewTemp { get; set; }
        public TextView TextViewTime { get; private set; }
        private void InitControls()
        {
            TextViewTime = FindViewById<TextView>(Resource.Id.textViewTime);
            TextViewTemp = FindViewById<TextView>(Resource.Id.textViewTemp);
            var handler = new TimeHandler(this);
            TimeThread m = new TimeThread(handler);
            new Java.Lang.Thread(m).Start();
        }
        private async void InitWeatherInfo()
        {
            try
            {
                var cityInfo = await Weather.GetCityCode();
                if (cityInfo != null && cityInfo.Data.Status == 1)
                {
                    var weatherInfo = await Weather.GetWeatherInfo(cityInfo.Data.Adcode);
                    if (weatherInfo != null && weatherInfo.Data.Status == 1 && weatherInfo.Data.Count > 0)
                    {
                        var current = weatherInfo.Data.Lives.FirstOrDefault();
                        TextViewTemp.Text = $"{current.Temperature}°C";
                    }
                }
            }
            catch (Exception ex)
            {
                LogManager.Instance.LogError("InitWeatherInfo", ex);
            }
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
            Weather = new WeatherClient();
            PackageReceiver = new PackageReceiver();
            IntentFilter filter = new IntentFilter();
            filter.AddAction("android.intent.action.PACKAGE_ADDED");
            filter.AddAction("android.intent.action.PACKAGE_REMOVED");
            filter.AddDataScheme("package");
            RegisterReceiver(PackageReceiver, filter);
            Window.DecorView.SystemUiVisibility = StatusBarVisibility.Hidden;
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