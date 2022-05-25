using Android.OS;
using Java.Lang;
using KLauncher.Libs.Core;
using System;
using JavaObject = Java.Lang.Object;

namespace KLauncher.Tasks
{
    public sealed class TimeThread : JavaObject, IRunnable
    {
        private Handler Handler { get; }
        public TimeThread(Handler handler)
        {
            Handler = handler;
        }
        public void Run()
        {
            while (true)
            {
                try
                {
                    Message msg = Handler.ObtainMessage();
                    Bundle bundle = new Bundle();
                    var time = DateTime.Now.ToString(SettingHelper.ShowSec ? "HH:mm:ss" : "HH:mm");
                    bundle.PutString("time", time);
                    msg.Data = bundle;
                    Handler.SendMessage(msg);
                    Thread.Sleep(900);
                }
                catch { }
            }
        }
    }
}