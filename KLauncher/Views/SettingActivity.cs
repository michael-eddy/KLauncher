using Android.App;
using Android.Content;
using Android.OS;
using Android.Provider;
using Android.Views;
using Android.Widget;
using Java.IO;
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
           var  buttonSelect = FindViewById<Button>(Resource.Id.buttonSelect);
            TextViewSave = FindViewById<TextView>(Resource.Id.textViewSave);
            TextViewBack = FindViewById<TextView>(Resource.Id.textViewBack);
            ShowSecSwitch = FindViewById<Switch>(Resource.Id.showSecSwitch);
            HideAppSwitch = FindViewById<Switch>(Resource.Id.hideAppSwitch);
            EditTextThread = FindViewById<EditText>(Resource.Id.editTextThraed);
            EditTextPercent = FindViewById<EditText>(Resource.Id.editTextPercent);
            buttonSelect.Click += ButtonSelect_Click;
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
        private void ButtonSelect_Click(object sender, EventArgs e)
        {
            Intent intent = new Intent(Intent.ActionPick, null);
            intent.SetDataAndType(MediaStore.Images.Media.ExternalContentUri, "image/*");
            StartActivityForResult(intent, 2);
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
        protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);
            try
            {
                if (requestCode == 2 && data != null)
                {
                    int n;
                    var uri = data.Data;
                    var input = ContentResolver.OpenInputStream(uri);
                    if (input.Length > 1048576)
                        this.ShowToast("背景图片不能超过10M喔~", ToastLength.Short);
                    else
                    {
                        using var output = new ByteArrayOutputStream();
                        byte[] buffer = new byte[4096];
                        while ((n = input.Read(buffer)) > 0)
                            output.Write(buffer, 0, n);
                        SettingHelper.Background = output.ToByteArray();
                    }
                }
            }
            catch (Exception ex)
            {
                LogManager.Instance.LogError("OnActivityResult", ex);
            }
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