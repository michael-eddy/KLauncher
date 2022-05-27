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
        private Context Context { get; }
        private readonly uint thread;
        private readonly double eatpercent;
        private readonly long totalMemory;
        public CleanerThread(Context context, double eatpercent, uint thread)
        {
            Context = context;
            this.thread = thread;
            this.eatpercent = eatpercent;
            totalMemory = context.GetTotalMemory();
        }
        public override void Run()
        {
            try
            {
                Runtime runtime = Runtime.GetRuntime();
                var value = totalMemory * eatpercent / thread;
                runtime.Exec($"{Context.ApplicationInfo.NativeLibraryDir}/libcleaner.so {value}");
            }
            catch (Exception ex)
            {
                Context.ShowToast(ex, ToastLength.Short);
            }
        }
    }
}