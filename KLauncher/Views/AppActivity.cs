using Android.App;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;
using KLauncher.Adapters;
using KLauncher.Libs;
using KLauncher.Libs.Core;
using KLauncher.Libs.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using Xamarin.Essentials;

namespace KLauncher
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme")]
    public sealed class AppActivity : BaseActivity
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
                IEnumerable<AppItem> appItems = AppCenter.Instance.Apps;
                if (!SettingHelper.ShowHidden)
                    appItems = appItems.Where(x => x.IsVisable);
                Items.Clear();
                Items.AddRange(appItems);
                Adapter.NotifyDataSetChanged();
                AppList.SetSelection(0);
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
                        var index = AppList.SelectedItemPosition;
                        if (index > 0)
                            AppList.SetSelection(index - 1);
                        break;
                    }
                case Keycode.PageDown:
                    {
                        var index = AppList.SelectedItemPosition;
                        if (index < Items.Count - 1)
                            AppList.SetSelection(index + 1);
                        break;
                    }
                case Keycode.Enter:
                    {
                        var index = AppList.SelectedItemPosition;
                        Adapter_ItemClick(index);
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