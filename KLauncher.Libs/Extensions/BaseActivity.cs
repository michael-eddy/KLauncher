using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Views;
using AndroidX.AppCompat.App;
using AndroidX.Fragment.App;
using System;

namespace KLauncher.Libs
{
    public abstract class BaseActivity : AppCompatActivity
    {
        protected PackageReceiver Receiver { get; private set; }
        protected override void OnStart()
        {
            base.OnStart();
            Window.SetStatusBarColor(Color.Transparent);
            try
            {
                Receiver = new PackageReceiver();
                IntentFilter filter = new IntentFilter();
                filter.AddAction("android.intent.action.PACKAGE_ADDED");
                filter.AddAction("android.intent.action.PACKAGE_REMOVED");
                filter.AddDataScheme("package");
                RegisterReceiver(Receiver, filter);
            }
            catch (Exception ex)
            {
                LogManager.Instance.LogError("PackageReceiver", ex);
            }
        }
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            ClearFragments();
        }
        protected override void OnDestroy()
        {
            base.OnDestroy();
            if (Receiver != null)
                UnregisterReceiver(Receiver);
        }
        public override void StartActivity(Intent intent)
        {
            base.StartActivity(intent);
            OverridePendingTransition(Resource.Animation.abc_slide_in_bottom, Resource.Animation.abc_slide_out_top);
        }
        public override void Finish()
        {
            base.Finish();
            OverridePendingTransition(Resource.Animation.abc_slide_in_top, Resource.Animation.abc_slide_out_bottom);
        }
        protected virtual void ClearFragments()
        {
            if (SupportFragmentManager != null && SupportFragmentManager.Fragments != null && SupportFragmentManager.Fragments.Count > 0)
            {
                FragmentTransaction ft = SupportFragmentManager.BeginTransaction();
                foreach (Fragment fragment in SupportFragmentManager.Fragments)
                    ft.Remove(fragment);
                ft.Commit();
            }
        }
        protected void ShowFragment(DialogFragment fragment, string tag)
        {
            FragmentTransaction fragmentTransaction;
            if (!fragment.IsAdded && null == SupportFragmentManager.FindFragmentByTag(tag))
            {
                fragmentTransaction = SupportFragmentManager.BeginTransaction();
                SupportFragmentManager.ExecutePendingTransactions();
                fragmentTransaction.Add(fragment, tag);
                fragmentTransaction.CommitAllowingStateLoss();
            }
            fragmentTransaction = SupportFragmentManager.BeginTransaction();
            fragmentTransaction.Show(fragment);
            fragmentTransaction.CommitAllowingStateLoss();
        }
        protected void HideFragment(DialogFragment fragment)
        {
            fragment.DismissAllowingStateLoss();
        }
    }
}