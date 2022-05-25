using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Views;
using Android.Widget;
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
    public sealed class CleanActivity : BaseActivity
    {
        public List<AppItem> Items { get; }
        private ListView AppList { get; set; }
        private TextView TextViewBack { get; set; }
        private TextView TextViewMenu { get; set; }
        private AppItemAdapter Adapter { get; set; }
        public CleanActivity()
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
            TextViewMenu.Text = "清理";
            Adapter = new AppItemAdapter(this, Items, false);
            AppList.Adapter = Adapter;
            TextViewBack.Click += TextViewBack_Click;
            TextViewMenu.Click += TextViewMenu_Click;
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
                        {
                            var index = AppList.SelectedItemPosition;
                            var item = Items.ElementAt(index);
                            this.OpenApp(item.PackageName);
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
                            CleanApp(AppList.SelectedItemPosition);
                        return true;
                    }
            }
            return base.DispatchKeyEvent(e);
        }
        private void CleanApp(int index)
        {
            var packageName = Items.ElementAt(index).PackageName;
            ActivityManager.FromContext(this).KillBackgroundProcesses(packageName);
            Items.RemoveAt(index);
            Adapter.NotifyDataSetChanged();
            AppList.SetSelection(0);
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
                var localPackageInfo1 = localList.ElementAt(i);
                if (((ApplicationInfoFlags.System & localPackageInfo1.ApplicationInfo.Flags) == 0)
                        && ((ApplicationInfoFlags.UpdatedSystemApp & localPackageInfo1.ApplicationInfo.Flags) == 0)
                        && ((ApplicationInfoFlags.Stopped & localPackageInfo1.ApplicationInfo.Flags) == 0))
                    packages.Add(localPackageInfo1.PackageName.Split(":").FirstOrDefault());
            }
            return AppCenter.Instance.Take(packages);
        }
    }
}