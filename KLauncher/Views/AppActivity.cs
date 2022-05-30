using Android.App;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;
using KLauncher.Adapters;
using KLauncher.Libs;
using KLauncher.Libs.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Xamarin.Essentials;

namespace KLauncher
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme")]
    public sealed class AppActivity : BaseActivity
    {
        public List<AppItem> Items { get; }
        private ListView AppList { get; set; }
        private TextView TextViewBack { get; set; }
        private TextView TextViewMenu { get; set; }
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
            TextViewBack = FindViewById<TextView>(Resource.Id.textViewBack);
            TextViewMenu = FindViewById<TextView>(Resource.Id.textViewMenu);
            Adapter = new AppItemAdapter(this, Items);
            Adapter.ItemClick += Adapter_ItemClick;
            Adapter.ItemLongClick += Adapter_ItemLongClick;
            TextViewBack.Click += TextViewBack_Click;
            TextViewMenu.Click += TextViewMenu_Click;
            AppList.Adapter = Adapter;
            AppCenter.Instance.AppUpdate += Instance_AppUpdate;
        }
        protected override void OnResume()
        {
            base.OnResume();
            AppList.SetSelection(0);
            RunOnUiThread(() =>
            {
                if (AppCenter.Instance.IsComplete)
                    AppUpdate();
            });
        }
        private void TextViewBack_Click(object sender, EventArgs e)
        {
            if (!this.IsFastDoubleClick())
                Finish();
        }
        private void TextViewMenu_Click(object sender, EventArgs e)
        {
            if (!this.IsFastDoubleClick())
            {
                menu = new PopupMenu(this, TextViewMenu);
                menu.MenuInflater.Inflate(Resource.Menu.view_menu, menu.Menu);
                menu.MenuItemClick += Menu_MenuItemClick;
                menu.DismissEvent += Menu_DismissEvent;
                menu.Show();
            }
        }
        private int position;
        private PopupMenu menu;
        private void Adapter_ItemLongClick(View sender, int position)
        {
            this.position = position;
            menu = new PopupMenu(this, sender);
            menu.MenuInflater.Inflate(Resource.Menu.app_menu, menu.Menu);
            menu.MenuItemClick += Menu_MenuItemClick;
            menu.DismissEvent += Menu_DismissEvent;
            menu.Show();
        }
        private void Menu_DismissEvent(object sender, PopupMenu.DismissEventArgs e)
        {
            menu.MenuItemClick -= Menu_MenuItemClick;
            menu.DismissEvent -= Menu_DismissEvent;
            menu.Dispose();
        }
        private void Adapter_ItemClick(object sender)
        {
            var position = sender.ToInt32();
            var item = Items.ElementAt(position);
            if (item != null)
                this.OpenApp(item);
        }
        private void Instance_AppUpdate(object sender) => AppUpdate();
        private void AppUpdate()
        {
            try
            {
                var appItems = AppCenter.Instance.Take();
                Items.Clear();
                Items.AddRange(appItems);
                Adapter.NotifyDataSetChanged();
                RunOnUiThread(() =>
                {
                    AppList.SetSelection(0);
                });
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
                case Keycode.Num5:
                    {
                        Adapter_ItemLongClick(TextViewMenu, AppList.SelectedItemPosition);
                        break;
                    }
                case Keycode.DpadCenter:
                    {
                        if (!this.IsFastDoubleClick())
                        {
                            var index = AppList.SelectedItemPosition;
                            Adapter_ItemClick(index);
                        }
                        return true;
                    }
                case Keycode.SoftRight:
                    {
                        if (!this.IsFastDoubleClick())
                            Finish();
                        return true;
                    }
                case Keycode.Menu:
                    {
                        if (!this.IsFastDoubleClick())
                        {
                            Thread.Sleep(200);
                            menu = new PopupMenu(this, TextViewMenu);
                            menu.MenuInflater.Inflate(Resource.Menu.view_menu, menu.Menu);
                            menu.MenuItemClick += Menu_MenuItemClick;
                            menu.DismissEvent += Menu_DismissEvent;
                            menu.Show();
                        }
                        return true;
                    }
            }
            return base.DispatchKeyEvent(e);
        }
        private void Menu_MenuItemClick(object sender, PopupMenu.MenuItemClickEventArgs e)
        {
            switch (e.Item.ItemId)
            {
                case Resource.Id.appmanage:
                    {
                        Intent intent = new Intent(Android.Provider.Settings.ActionManageAllApplicationsSettings);
                        StartActivity(intent);
                        break;
                    }
                case Resource.Id.setting:
                    {
                        if (!this.IsFastDoubleClick())
                        {
                            Intent intent = new Intent(this, typeof(SettingActivity));
                            StartActivity(intent);
                        }
                        break;
                    }
                case Resource.Id.runing:
                    {
                        if (!this.IsFastDoubleClick())
                        {
                            Intent intent = new Intent(this, typeof(RunningActivity));
                            StartActivity(intent);
                        }
                        break;
                    }
                case Resource.Id.open:
                    {
                        if (!this.IsFastDoubleClick())
                            Adapter_ItemClick(position);
                        break;
                    }
                case Resource.Id.remove:
                    {
                        if (!this.IsFastDoubleClick())
                        {
                            var item = Items.ElementAt(position);
                            var uri = Android.Net.Uri.FromParts("package", item?.PackageName, null);
                            Intent intent = new Intent(Intent.ActionDelete, uri);
                            StartActivity(intent);
                        }
                        break;
                    }
                case Resource.Id.info:
                    {
                        var packageName = Items.ElementAt(position)?.PackageName;
                        Intent intent = new Intent(Android.Provider.Settings.ActionApplicationDetailsSettings);
                        intent.AddFlags(ActivityFlags.NewTask);
                        intent.SetData(Android.Net.Uri.Parse($"package:{packageName}"));
                        StartActivity(intent);
                        break;
                    }
                case Resource.Id.hidden:
                    {
                        if (!this.IsFastDoubleClick())
                        {
                            var item = Items.ElementAt(position);
                            item.IsVisable = false;
                            AppCenter.Instance.SaveAsync(item);
                            AppList.SetSelection(0);
                        }
                        break;
                    }
                default:
                    {
                        if (!this.IsFastDoubleClick())
                        {
                            Intent intent = new Intent(this, typeof(AboutActivity));
                            StartActivity(intent);
                        }
                        break;
                    }
            }
        }
    }
}