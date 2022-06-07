using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Views;
using Android.Widget;
using KLauncher.Adapters;
using KLauncher.Libs;
using KLauncher.Libs.Extensions;
using KLauncher.Libs.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using Xamarin.Essentials;
using Thread = System.Threading.Thread;

namespace KLauncher
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme")]
    public sealed class RunningActivity : ShizukuActivity
    {
        public List<AppItem> Items { get; }
        private ListView AppList { get; set; }
        private TextView TextViewBack { get; set; }
        private TextView TextViewMenu { get; set; }
        private AppItemAdapter Adapter { get; set; }
        public RunningActivity()
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
            Adapter.ItemLongClick += Adapter_ItemLongClick;
            AppList.Adapter = Adapter;
            TextViewBack.Click += TextViewBack_Click;
            TextViewMenu.Click += TextViewMenu_Click;
            RunOnUiThread(CheckShizuku);
        }
        private int position;
        private PopupMenu menu;
        private void Adapter_ItemLongClick(View view, int position)
        {
            if (!this.IsFastDoubleClick())
            {
                this.position = position;
                menu = new PopupMenu(this, view);
                menu.MenuInflater.Inflate(Resource.Menu.runing_menu, menu.Menu);
                menu.MenuItemClick += Menu_MenuItemClick;
                menu.DismissEvent += Menu_DismissEvent;
                menu.Show();
            }
        }
        private void Menu_DismissEvent(object sender, PopupMenu.DismissEventArgs e)
        {
            menu.MenuItemClick -= Menu_MenuItemClick;
            menu.DismissEvent -= Menu_DismissEvent;
            menu.Dispose();
        }
        private void Menu_MenuItemClick(object sender, PopupMenu.MenuItemClickEventArgs e)
        {
            switch (e.Item.ItemId)
            {
                case Resource.Id.open:
                    {
                        if (!this.IsFastDoubleClick())
                            OpenApp(position);
                        break;
                    }
                case Resource.Id.end:
                    {
                        if (!this.IsFastDoubleClick())
                            CleanApp(position);
                        break;
                    }
                case Resource.Id.info:
                    {
                        if (!this.IsFastDoubleClick())
                        {
                            var packageName = Items.ElementAt(position)?.PackageName;
                            if (!string.IsNullOrEmpty(packageName))
                            {
                                Intent intent = new Intent(Android.Provider.Settings.ActionApplicationDetailsSettings);
                                intent.AddFlags(ActivityFlags.NewTask);
                                intent.SetData(Android.Net.Uri.Parse($"package:{packageName}"));
                                StartActivity(intent);
                            }
                        }
                        break;
                    }
            }
        }
        private void TextViewBack_Click(object sender, EventArgs e)
        {
            if (!this.IsFastDoubleClick())
                Finish();
        }
        private void TextViewMenu_Click(object sender, EventArgs e)
        {
            if (!this.IsFastDoubleClick())
                CleanApp(AppList.SelectedItemPosition);
        }
        public override bool DispatchKeyEvent(KeyEvent e)
        {
            switch (e.KeyCode)
            {
                case Keycode.DpadCenter:
                    {
                        if (!this.IsFastDoubleClick())
                            OpenApp(position);
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
                            position = AppList.SelectedItemPosition;
                            menu = new PopupMenu(this, TextViewMenu);
                            menu.MenuInflater.Inflate(Resource.Menu.runing_menu, menu.Menu);
                            menu.MenuItemClick += Menu_MenuItemClick;
                            menu.DismissEvent += Menu_DismissEvent;
                            menu.Show();
                        }
                        return true;
                    }
            }
            return base.DispatchKeyEvent(e);
        }
        private void OpenApp(int position)
        {
            if (position > -1)
            {
                var item = Items.ElementAt(position);
                this.OpenApp(item);
            }
        }
        private void CleanApp(int index)
        {
            if (index > -1)
            {
                var packageName = Items.ElementAt(index).PackageName;
                ((ActivityManager)GetSystemService(ActivityService)).KillBackgroundProcesses(packageName);
                this.KillApp(packageName);
                ShizukuExec(string.Format(ShizukuCommand.FORCE_KILL, packageName));
                Items.RemoveAt(index);
                Adapter.NotifyDataSetChanged();
                AppList.SetSelection(0);
            }
        }
        protected override void OnResume()
        {
            base.OnResume();
            Items.Clear();
            Items.AddRange(GetRunningList());
            Adapter.NotifyDataSetChanged();
            AppList.SetSelection(0);
        }
        private IEnumerable<AppItem> GetRunningList()
        {
            List<string> packages = new List<string>();
            var localList = PackageManager.GetInstalledPackages(0);
            for (int i = 0; i < localList.Count; i++)
            {
                var localPackageInfo = localList.ElementAt(i);
                if ((ApplicationInfoFlags.Stopped & localPackageInfo.ApplicationInfo.Flags) == 0)
                    packages.Add(localPackageInfo.PackageName.Split(":").FirstOrDefault());
            }
            return AppCenter.Instance.Take(packages);
        }
    }
}