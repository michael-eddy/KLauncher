using Android.App;
using Android.Content;
using Android.Content.PM;
using KLauncher.Libs.Models;
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
        public void UpdateList()
        {
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
                        var drawable = packageInfo.ApplicationInfo.LoadIcon(Context.PackageManager).ToBas64Code();
                        var displayName = packageInfo.ApplicationInfo.LoadLabel(Context.PackageManager);
                        var app = Apps.FirstOrDefault(x => x.PackageName == packageName);
                        if (app != null)
                        {
                            if (versionCode != app.VersionCode)
                            {
                                app.Icon = drawable;
                                app.VersionCode = versionCode;
                                app.DisplayName = displayName;
                                Save(app);
                            }
                        }
                        else
                        {
                            app = new AppItem
                            {
                                IsVisable = true,
                                Icon = drawable,
                                VersionCode = versionCode,
                                DisplayName = displayName,
                                PackageName = packageName,
                                IsSystem = packageInfo.ApplicationInfo.IsSystem()
                            };
                            Apps.Add(app);
                            Save(app);
                        }
                    }
                }
                IsComplete = true;
            });
        }
        private void Save(AppItem app)
        {
            var appItem = DB.Connection.Table<AppItem>().FirstOrDefault(x => x.PackageName == app.PackageName);
            if (appItem != null)
            {
                appItem.Icon = app.Icon;
                appItem.IsSystem = app.IsSystem;
                appItem.IsVisable = app.IsVisable;
                appItem.VersionCode = app.VersionCode;
                appItem.DisplayName = app.DisplayName;
                DB.Connection.Update(appItem);
            }
            else
                DB.Connection.Insert(app);
        }
        private void Remove(string packageName)
        {
            Task.Factory.StartNew(() =>
            {
                var appItem = DB.Connection.Table<AppItem>().FirstOrDefault(x => x.PackageName == packageName);
                if (appItem != null)
                    DB.Connection.Delete(appItem);
                AppUpdate?.Invoke(this);
            });
        }
        public void SaveAsync(AppItem app)
        {
            Task.Factory.StartNew(() =>
            {
                var appItem = DB.Connection.Table<AppItem>().FirstOrDefault(x => x.PackageName == app.PackageName);
                if (appItem != null)
                {
                    appItem.Icon = app.Icon;
                    appItem.IsSystem = app.IsSystem;
                    appItem.IsVisable = app.IsVisable;
                    appItem.VersionCode = app.VersionCode;
                    appItem.DisplayName = app.DisplayName;
                    DB.Connection.Update(appItem);
                }
                else
                    DB.Connection.Insert(app);
                AppUpdate?.Invoke(this);
            });
        }
        public void UpdateOne(string packageName, UpdateType updateType)
        {
            var packageInfo = Context.PackageManager.GetPackageInfo(packageName, PackageInfoFlags.Activities);
            if (packageInfo != null)
            {
                switch (updateType)
                {
                    case UpdateType.Add:
                        {
                            var versionCode = packageInfo.LongVersionCode;
                            var drawable = packageInfo.ApplicationInfo.LoadIcon(Context.PackageManager);
                            var app = new AppItem
                            {
                                IsVisable = true,
                                VersionCode = versionCode,
                                PackageName = packageName,
                                Icon = drawable.ToBas64Code(),
                                IsSystem = packageInfo.ApplicationInfo.IsSystem(),
                                DisplayName = packageInfo.ApplicationInfo.LoadLabel(Context.PackageManager)
                            };
                            var appItem = Apps.FirstOrDefault(x => x.PackageName == packageName);
                            if (appItem == null)
                                Apps.Add(app);
                            else
                            {
                                appItem.Icon = app.Icon;
                                appItem.VersionCode = app.VersionCode;
                                appItem.DisplayName = app.DisplayName;
                            }
                            SaveAsync(app);
                            break;
                        }
                    case UpdateType.Remove:
                        {
                            Remove(packageName);
                            break;
                        }
                }
            }
        }
    }
}