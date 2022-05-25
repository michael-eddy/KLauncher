using Android.Content;
using Android.Widget;
using Java.Lang;
using KLauncher.Libs;
using KLauncher.Libs.Core;
using System;
using System.Runtime.InteropServices;
using Exception = System.Exception;

namespace KLauncher.Tasks
{
    public sealed class CleanerThread : Thread
    {
        private const string DllName = "libcleaner.so";
        [DllImport(DllName, EntryPoint = "cleaner")]
        private static extern int cleaner(long l);
        private Context Context { get; }
        private readonly double eatpercent;
        private readonly long totalMemory;
        public CleanerThread(Context context, double eatpercent)
        {
            Context = context;
            this.eatpercent = eatpercent;
            totalMemory = context.GetTotalMemory();
        }
        public override void Run()
        {
            try
            {
                if (SettingHelper.UseShell)
                {
                    Runtime runtime = Runtime.GetRuntime();
                    runtime.Exec($"{Context.ApplicationInfo.NativeLibraryDir}/libcleaner.so {totalMemory * eatpercent}");
                }
                else
                    cleaner(Convert.ToInt64(totalMemory * eatpercent));
            }
            catch (Exception ex)
            {
                Context.ShowToast(ex, ToastLength.Short);
            }
        }
    }
}