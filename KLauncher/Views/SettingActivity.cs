using Android.App;
using Android.OS;
using Android.Views;
using Android.Widget;
using KLauncher.Libs;
using KLauncher.Libs.Core;
using System;
using Xamarin.Essentials;

namespace KLauncher
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme")]
    public sealed class SettingActivity : BaseActivity
    {
        private bool FirstLoad = true;
        private Switch ShowSecSwitch { get; set; }
        private Switch HideAppSwitch { get; set; }
        private TextView TextViewBack { get; set; }
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Platform.Init(this, savedInstanceState);
            SetContentView(Resource.Layout.activity_setting);
            ShowSecSwitch = FindViewById<Switch>(Resource.Id.showSecSwitch);
            HideAppSwitch = FindViewById<Switch>(Resource.Id.hideAppSwitch);
            TextViewBack = FindViewById<TextView>(Resource.Id.textViewBack);
            TextViewBack.Click += TextViewBack_Click;
            HideAppSwitch.CheckedChange += HideAppSwitch_CheckedChange;
            ShowSecSwitch.CheckedChange += ShowSecSwitch_CheckedChange;
            RunOnUiThread(() =>
            {
                ShowSecSwitch.Checked = SettingHelper.ShowSec;
                HideAppSwitch.Checked = SettingHelper.ShowHidden;
                FirstLoad = false;
            });
        }
        private void TextViewBack_Click(object sender, EventArgs e)
        {
            Finish();
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
        public override bool DispatchKeyEvent(KeyEvent e)
        {
            switch (e.KeyCode)
            {
                case Keycode.SoftRight:
                    {
                        if (!this.IsFastDoubleClick())
                            Finish();
                        return true;
                    }
            }
            return base.DispatchKeyEvent(e);
        }
    }
}