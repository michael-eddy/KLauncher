using Android.OS;
using Android.Views;
using Android.Widget;
using System;
using AndroidX.Fragment.App;
using KLauncher.Libs;
using KLauncher.Tasks;

namespace KLauncher
{
    public sealed class CleanDialog : DialogFragment
    {
        private CleanDialog() { }
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
        public event CallbackObject OnHidden;
        private TextView TextViewUse { get; set; }
        private TextView TextViewTotal { get; set; }
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            View = inflater.Inflate(Resource.Layout.clean_dialog_view, container, false);
            TextViewUse = View.FindViewById<TextView>(Resource.Id.textViewUse);
            TextViewTotal = View.FindViewById<TextView>(Resource.Id.textViewTotal);
            var buttonClear = View.FindViewById<Button>(Resource.Id.buttonClear);
            buttonClear.Click += ButtonClear_Click;
            return View;
        }
        private void ButtonClear_Click(object sender, EventArgs e)
        {
            new CleanerThread(Activity, .6).Start();
        }
    }
}