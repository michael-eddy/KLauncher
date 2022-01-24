using Android.App;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;
using AndroidX.AppCompat.App;
using KLauncher.Adapters;
using KLauncher.Libs;
using KLauncher.Libs.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using Xamarin.Essentials;

namespace KLauncher
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme")]
    public sealed class AppActivity : AppCompatActivity
    {
        public List<AppItem> Items { get; }
        private ListView AppList { get; set; }
        private AppItemAdapter Adapter { get; set; }
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
            Adapter = new AppItemAdapter(this, Items);
            Adapter.ItemClick += Adapter_ItemClick;
            AppList.Adapter = Adapter;
            AppCenter.Instance.AppUpdate += Instance_AppUpdate;
            RunOnUiThread(() =>
            {
                if (AppCenter.Instance.IsComplete)
                    AppUpdate();
            });
        }
        private void Adapter_ItemClick(object sender)
        {
            var position = sender.ToInt32();
            var item = Items.ElementAt(position);
            if (item != null)
                this.OpenApp(item.PackageName);
        }
        private void Instance_AppUpdate(object sender)
        {
            AppUpdate();
        }
        private void AppUpdate()
        {
            try
            {
                var appItems = AppCenter.Instance.Apps;
                Items.Clear();
                Items.AddRange(appItems);
                Adapter.NotifyDataSetChanged();
            }
            catch (Exception ex)
            {
                LogManager.Instance.LogError("AppUpdate", ex);
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