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
using System.IO;
using System.Linq;
using Xamarin.Essentials;

namespace KLauncher
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.Locale | ConfigChanges.Navigation | ConfigChanges.Orientation)]
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
            RunOnUiThread(InitControls);
        }
        private TextView TextViewWind { get; set; }
        private TextView TextViewTemp { get; set; }
        private TextView TextViewWeather { get; set; }
        public TextView TextViewTime { get; private set; }
        private void InitControls()
        {
            TextViewTime = FindViewById<TextView>(Resource.Id.textViewTime);
            TextViewWind = FindViewById<TextView>(Resource.Id.textViewWind);
            TextViewTemp = FindViewById<TextView>(Resource.Id.textViewTemp);
            TextViewWeather = FindViewById<TextView>(Resource.Id.textViewWeather);
            var handler = new TimeHandler(this);
            TimeThread m = new TimeThread(handler);
            new Java.Lang.Thread(m).Start();
        }
        private void InitWeatherInfo()
        {
            RunOnUiThread(async () =>
            {
                try
                {
                    var ipString = await Weather.GetIpAddress();
                    if (!string.IsNullOrEmpty(ipString))
                    {
                        var cityInfo = await Weather.GetCityCode(ipString);
                        if (cityInfo != null && cityInfo.Data.Status == 1)
                        {
                            var weatherInfo = await Weather.GetWeatherInfo(cityInfo.Data.Adcode);
                            if (weatherInfo != null && weatherInfo.Data.Status == 1 && weatherInfo.Data.Count > 0)
                            {
                                var current = weatherInfo.Data.Lives.FirstOrDefault();
                                TextViewWeather.Text = $"天气 {current.Weather}";
                                TextViewTemp.Text = $"温度 {current.Temperature}°C";
                                TextViewWind.Text = $"风向 {current.WindDirection} {current.WindPower}级";
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    LogManager.Instance.LogError("InitWeatherInfo", ex);
                }
            });
        }
        public override bool DispatchKeyEvent(KeyEvent e)
        {
            switch (e.KeyCode)
            {
                case Keycode.Back:
                    return true;
                case Keycode.Menu:
                    {
                        if (!this.IsFastDoubleClick())
                        {
                            Intent intent = new Intent(this, typeof(AppActivity));
                            StartActivity(intent);
                        }
                        return true;
                    }
                case Keycode.PageUp:
                    {

                        return true;
                    }
                case Keycode.PageDown:
                    {

                        return true;
                    }
                case Keycode.Pound:
                    {

                        return true;
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
            Current.Instance.Init();
            filter.AddAction("android.intent.action.PACKAGE_ADDED");
            filter.AddAction("android.intent.action.PACKAGE_REMOVED");
            filter.AddDataScheme("package");
            RegisterReceiver(PackageReceiver, filter);
            Window.DecorView.SystemUiVisibility = StatusBarVisibility.Hidden;
            InitWeatherInfo();
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