using Android.OS;
using Android.Views;
using Android.Widget;
using System;
using AndroidX.Fragment.App;
using KLauncher.Libs;
using KLauncher.Tasks;
using KLauncher.Libs.Core;
using Android.Graphics.Drawables;
using Android.Util;
using Android.Graphics;

namespace KLauncher
{
    public sealed class CleanDialog : DialogFragment
    {
        private CleanDialog()
        {
            TotalMemory = Activity.GetTotalMemory();
        }
        public long TotalMemory { get; }
        private static CleanDialog instance;
        public static CleanDialog Instance
        {
            get
            {
                if (instance == null)
                    instance = new CleanDialog();
                return instance;
            }
        }
        private new View View { get; set; }
        private MemoryThread Thread { get; set; }
        public Button ButtonClear { get; private set; }
        public TextView TextViewUse { get; private set; }
        public TextView TextViewTotal { get; private set; }
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            View = inflater.Inflate(Resource.Layout.clean_dialog_view, container, false);
            TextViewUse = View.FindViewById<TextView>(Resource.Id.textViewUse);
            TextViewTotal = View.FindViewById<TextView>(Resource.Id.textViewTotal);
            TextViewTotal.Text = $"总{(int)(TotalMemory / 1048576)}MB";
            ButtonClear = View.FindViewById<Button>(Resource.Id.buttonClear);
            ButtonClear.Click += ButtonClear_Click;
            return View;
        }
        public override void OnStart()
        {
            base.OnStart();
            var win = Dialog.Window;
            win.SetBackgroundDrawable(new ColorDrawable(Color.WhiteSmoke));
            DisplayMetrics dm = new DisplayMetrics();
            Activity.WindowManager.DefaultDisplay.GetMetrics(dm);
            win.Attributes.Gravity = GravityFlags.Bottom;
            win.Attributes.Width = ViewGroup.LayoutParams.MatchParent;
            win.Attributes.Height = ViewGroup.LayoutParams.WrapContent;
        }
        public override void OnResume()
        {
            base.OnResume();
            try
            {
                var handler = new MemoryHandler(this);
                Thread = new MemoryThread(handler);
                Thread.Start();
            }
            catch { }
        }
        public override void OnPause()
        {
            Thread.Interrupt();
            try
            {
                Thread.Interrupt();
                Thread.Dispose();
            }
            catch { }
            base.OnPause();
        }
        private void ButtonClear_Click(object sender, EventArgs e)
        {
            if (!Activity.IsFastDoubleClick())
                new CleanerThread(Activity, SettingHelper.CleanPercent, SettingHelper.CleanThread).Start();
        }
    }
}