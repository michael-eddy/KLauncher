using Android.App;
using Android.OS;
using Android.Widget;
using KLauncher.Libs;
using KLauncher.Libs.Core;
using Xamarin.Essentials;

namespace KLauncher
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme")]
    public sealed class SettingActivity : BaseActivity
    {
        private bool FirstLoad = true;
        private Switch ShowSecSwitch { get; set; }
        private Switch HideAppSwitch { get; set; }
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Platform.Init(this, savedInstanceState);
            SetContentView(Resource.Layout.activity_setting);
            ShowSecSwitch = FindViewById<Switch>(Resource.Id.showSecSwitch);
            HideAppSwitch = FindViewById<Switch>(Resource.Id.hideAppSwitch);
            HideAppSwitch.CheckedChange += HideAppSwitch_CheckedChange;
            ShowSecSwitch.CheckedChange += ShowSecSwitch_CheckedChange;
            RunOnUiThread(() =>
            {
                ShowSecSwitch.Checked = SettingHelper.ShowSec;
                HideAppSwitch.Checked = SettingHelper.ShowHidden;
                FirstLoad = false;
            });
        }
        private void ShowSecSwitch_CheckedChange(object sender, CompoundButton.CheckedChangeEventArgs e)
        {
            if (FirstLoad) return;
            SettingHelper.ShowSec = e.IsChecked;
            this.ShowToast(e.IsChecked ? "启用成功！" : "关闭成功！", ToastLength.Short);
        }
        private void HideAppSwitch_CheckedChange(object sender, CompoundButton.CheckedChangeEventArgs e)
        {
            if (FirstLoad) return;
            SettingHelper.ShowHidden = e.IsChecked;
            this.ShowToast(e.IsChecked ? "启用成功！" : "关闭成功！", ToastLength.Short);
        }
    }
}