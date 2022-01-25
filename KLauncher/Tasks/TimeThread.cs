﻿using Android.OS;
using Java.Lang;
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
            Message msg = Handler.ObtainMessage();
            Bundle bundle = new Bundle();
            var time = DateTime.Now.ToString("HH:mm");
            bundle.PutString("time", time);
            msg.Data = bundle;
            Handler.SendMessage(msg);
        }
    }
}