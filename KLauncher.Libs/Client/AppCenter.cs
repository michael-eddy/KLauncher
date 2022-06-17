using Android.App;
using Android.Content;
using Android.Content.PM;
using KLauncher.Libs.Core;
using KLauncher.Libs.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KLauncher.Libs
{
    public sealed class AppCenter
    {
        private DB DB { get; }
        private Intent Intent { get; }
        private Context Context { get; }
        public bool IsComplete { get; set; }
        public List<AppItem> Apps { get; }
        private static AppCenter _Instance;
        public event CallbackObject AppUpdate;
        public static AppCenter Instance
        {
            get
            {
                if (_Instance == null)
                    _Instance = new AppCenter();
                return _Instance;
            }
        }
        private AppCenter()
        {
            DB = new DB();
            Context = Application.Context;
            Apps = new List<AppItem>();
            Intent = new Intent(Intent.ActionMain, null);
            Intent.AddCategory(Intent.CategoryLauncher);
        }
        public IEnumerable<AppItem> Take() => Apps.Where(x => !SettingHelper.ShowHidden || x.IsVisable);
        public IEnumerable<AppItem> Take(IEnumerable<string> packages) => Apps.Where(x => packages.Contains(x.PackageName));
        public void UpdateList()
        {
            if (IsComplete) return;
            Task.Factory.StartNew(() =>
            {
                var apps = DB.Connection.Table<AppItem>().ToList();
                Apps.Clear();
                Apps.AddRange(apps);
                var resolveInfos = Context.PackageManager.QueryIntentActivities(Intent, PackageInfoFlags.Activities);
                if (resolveInfos != null && resolveInfos.Count > 0)
                {
                    foreach (var resolveInfo in resolveInfos)
                    {
                        var packageInfo = Context.PackageManager.GetPackageInfo(resolveInfo.ActivityInfo.ApplicationInfo.PackageName, PackageInfoFlags.Activities);
                        var versionCode = packageInfo.LongVersionCode;
                        var className = packageInfo.ApplicationInfo.ClassName;
                        var packageName = packageInfo.ApplicationInfo.PackageName;
                        if (packageName.Equals(Context.PackageName, StringComparison.CurrentCultureIgnoreCase))
                            continue;
                        var drawable = packageInfo.ApplicationInfo.LoadIcon(Context.PackageManager).ToBas64Code();
                        var displayName = packageInfo.ApplicationInfo.LoadLabel(Context.PackageManager);
                        var app = Apps.FirstOrDefault(x => x.PackageName == packageName);
                        if (app != null)
                        {
                            if (versionCode != app.VersionCode)
                            {
                                app.Icon = drawable;
                                app.ClassName = className;
                                app.VersionCode = versionCode;
                                app.DisplayName = displayName;
                                SaveAsync(app);
                            }
                        }
                        else
                        {
                            app = new AppItem
                            {
                                IsVisable = true,
                                Icon = drawable,
                                ClassName = className,
                                VersionCode = versionCode,
                                DisplayName = displayName,
                                PackageName = packageName,
                                IsSystem = packageInfo.ApplicationInfo.IsSystem()
                            };
                            Apps.Add(app);
                            SaveAsync(app);
                        }
                    }
                    AppUpdate?.Invoke(this);
                }
                OrderResult();
                IsComplete = true;
            });
        }
        private void OrderResult()
        {
            try
            {
                foreach (var app in Apps)
                {
                    var fistChar = app.DisplayName.ToCharArray().FirstOrDefault();
                    app.OrderChar = fistChar > 127 ? fistChar.GetPinyin() : fistChar.ToString();
                }
                var items = Apps.OrderBy(x => x.OrderChar).ToList();
                Apps.Clear();
                Apps.AddRange(items);
            }
            catch (Exception ex)
            {
                LogManager.Instance.LogError("OrderResult", ex);
            }
        }
        private void Remove(string packageName)
        {
            try
            {
                var appItem = DB.Connection.Table<AppItem>().FirstOrDefault(x => x.PackageName == packageName);
                if (appItem != null)
                {
                    var removeIdx = Apps.FindIndex(x => x.PackageName == packageName);
                    Apps.RemoveAt(removeIdx);
                    DB.Connection.Delete(appItem);
                }
                AppUpdate?.Invoke(this);
            }
            catch (Exception ex)
            {
                LogManager.Instance.LogError("Remove", ex);
            }
        }
        public void SaveAsync(AppItem app)
        {
            try
            {
                var appItem = DB.Connection.Table<AppItem>().FirstOrDefault(x => x.PackageName == app.PackageName);
                if (appItem != null)
                {
                    appItem.Icon = app.Icon;
                    appItem.IsSystem = app.IsSystem;
                    appItem.IsVisable = app.IsVisable;
                    appItem.ClassName = app.ClassName;
                    appItem.VersionCode = app.VersionCode;
                    appItem.DisplayName = app.DisplayName;
                    DB.Connection.Update(appItem);
                }
                else
                    DB.Connection.Insert(app);
                AppUpdate?.Invoke(this);
            }
            catch (Exception ex)
            {
                LogManager.Instance.LogError("SaveAsync", ex);
            }
        }
        public void UpdateOne(string packageName, UpdateType updateType)
        {
            try
            {
                switch (updateType)
                {
                    case UpdateType.Add:
                        {
                            var packageInfo = Context.PackageManager.GetPackageInfo(packageName, PackageInfoFlags.Activities);
                            if (packageInfo != null)
                            {
                                var versionCode = packageInfo.LongVersionCode;
                                var className = packageInfo.ApplicationInfo.ClassName;
                                var drawable = packageInfo.ApplicationInfo.LoadIcon(Context.PackageManager);
                                var app = new AppItem
                                {
                                    IsVisable = true,
                                    ClassName = className,
                                    VersionCode = versionCode,
                                    PackageName = packageName,
                                    Icon = drawable.ToBas64Code(),
                                    IsSystem = packageInfo.ApplicationInfo.IsSystem(),
                                    DisplayName = packageInfo.ApplicationInfo.LoadLabel(Context.PackageManager)
                                };
                                var appItem = Apps.FirstOrDefault(x => x.PackageName == packageName);
                                if (appItem == null)
                                {
                                    Apps.Add(app);
                                    OrderResult();
                                }
                                else
                                {
                                    appItem.Icon = app.Icon;
                                    appItem.ClassName = app.ClassName;
                                    appItem.VersionCode = app.VersionCode;
                                    appItem.DisplayName = app.DisplayName;
                                }
                                SaveAsync(app);
                            }
                            break;
                        }
                    case UpdateType.Remove:
                        {
                            Remove(packageName);
                            break;
                        }
                }
            }
            catch (Exception ex)
            {
                LogManager.Instance.LogError("UpdateOne", ex);
            }
        }
    }
}