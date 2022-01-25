using Android.Content;
using Android.Graphics;
using Android.Views;
using AndroidX.AppCompat.App;

namespace KLauncher.Libs
{
    public abstract class BaseActivity : AppCompatActivity
    {
        protected override void OnStart()
        {
            base.OnStart();
            Window.SetStatusBarColor(Color.Transparent);

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
    }
}