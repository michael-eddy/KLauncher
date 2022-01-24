using Android.App;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;
using AndroidX.AppCompat.App;
using KLauncher.Libs;
using KLauncher.Libs.Models;
using System;
using System.Collections.Generic;
using Xamarin.Essentials;

namespace KLauncher
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme")]
    public sealed class AppActivity : AppCompatActivity
    {
        public List<AppItem> Items { get; }
        private ListView AppList { get; set; }
        public AppActivity()
        {
            Items = new List<AppItem>();
        }
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Platform.Init(this, savedInstanceState);
            SetContentView(Resource.Layout.activity_applist);

            AppList = FindViewById<ListView>(Resource.Id.appList);
            InitViewStyle();
            AppCenter.Instance.AppUpdate += Instance_AppUpdate;
        }
        private void Instance_AppUpdate(object sender)
        {
            try
            {
                var appItems = AppCenter.Instance.Apps;
                Items.Clear();
                Items.AddRange(appItems);
            }
            catch(Exception ex)
            {
                LogManager.Instance.LogError("AppUpdate", ex);
            }
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
                        AppList.SetSelection(1);
                        break;
                    }
                case Keycode.Enter:
                    {

                        break;
                    }
                case Keycode.SoftRight:
                    {
                        Intent intent = new Intent(this, typeof(MainActivity));
                        StartActivity(intent);
                        break;
                    }
            }
            return base.DispatchKeyEvent(e);
        }
    }
}