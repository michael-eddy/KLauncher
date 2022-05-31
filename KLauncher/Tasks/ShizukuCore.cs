using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Rikka.Shizuku;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KLauncher.Tasks
{
    public sealed class ShizukuCore
    {
        public object S
        {
            get
            {
                var wrapper = new ShizukuBinderWrapper(SystemServiceHelper.GetSystemService("package"));
                return wrapper;
            }
        }
    }
}