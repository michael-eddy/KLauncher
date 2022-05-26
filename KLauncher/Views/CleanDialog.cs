using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            View = inflater.Inflate(Resource.Layout.clean_dialog_view, container, false);


            new CleanerThread(Activity, .6).Start();
            return View;
        }
    }
}