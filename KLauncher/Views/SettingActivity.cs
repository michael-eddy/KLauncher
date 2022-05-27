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
        private TextView TextViewSave { get; set; }
        private TextView TextViewBack { get; set; }
        private EditText EditTextThread { get; set; }
        private EditText EditTextPercent { get; set; }
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Platform.Init(this, savedInstanceState);
            SetContentView(Resource.Layout.activity_setting);
            TextViewSave = FindViewById<TextView>(Resource.Id.textViewSave);
            TextViewBack = FindViewById<TextView>(Resource.Id.textViewBack);
            ShowSecSwitch = FindViewById<Switch>(Resource.Id.showSecSwitch);
            HideAppSwitch = FindViewById<Switch>(Resource.Id.hideAppSwitch);
            EditTextThread = FindViewById<EditText>(Resource.Id.editTextThraed);
            EditTextPercent = FindViewById<EditText>(Resource.Id.editTextPercent);
            TextViewSave.Click += TextViewSave_Click;
            TextViewBack.Click += TextViewBack_Click;
            HideAppSwitch.CheckedChange += HideAppSwitch_CheckedChange;
            ShowSecSwitch.CheckedChange += ShowSecSwitch_CheckedChange;
            RunOnUiThread(() =>
            {
                ShowSecSwitch.Checked = SettingHelper.ShowSec;
                HideAppSwitch.Checked = SettingHelper.ShowHidden;
                EditTextThread.Text = SettingHelper.CleanThread.ToString();
                EditTextPercent.Text = (SettingHelper.CleanPercent * 100).ToString();
                FirstLoad = false;
            });
        }
        private void TextViewSave_Click(object sender, EventArgs e) => Saved();
        private void Saved()
        {
            uint thread = uint.Parse(EditTextThread.Text), percent = uint.Parse(EditTextPercent.Text);
            if (thread <= 0) thread = 1;
            if (percent <= 0) percent = 60;
            SettingHelper.CleanThread = thread;
            SettingHelper.CleanPercent = percent / 100;
        }
        private void TextViewBack_Click(object sender, EventArgs e) => Finish();
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
                case Keycode.Menu:
                    {
                        if (!this.IsFastDoubleClick())
                            Saved();
                        break;
                    }
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