using Android.Content.PM;
using Android.Widget;
using Java.Lang;
using Rikka.Shizuku;

namespace KLauncher.Libs.Extensions
{
    public abstract class ShizukuActivity : BaseActivity, Shizuku.IOnRequestPermissionResultListener
    {
        public event CallbackObject PermissionCallback;
        protected bool Granted { get; private set; }
        protected bool Running { get; private set; } = true;
        protected override void OnStart()
        {
            base.OnStart();
            Shizuku.AddRequestPermissionResultListener(this);
        }
        protected override void OnDestroy()
        {
            Shizuku.RemoveRequestPermissionResultListener(this);
            base.OnDestroy();
        }
        protected void ShizukuExec(string cmd)
        {
            try
            {
                if (Granted && Running)
                {
#pragma warning disable CS0618
                    var p = Shizuku.NewProcess(new string[] { "sh" }, null, null);
                    p.OutputStream.Write((cmd + "\nexit\n").GetBytes());
                    p.OutputStream.Flush();
                    p.OutputStream.Close();
                    p.WaitFor();
#pragma warning restore CS0618 
                }
            }
            catch (Exception ex)
            {
                LogManager.Instance.LogError("ShizukuExec", ex);
            }
        }
        protected void RequestPermission() => Shizuku.RequestPermission(0);
        protected void CheckShizuku()
        {
            try
            {
                if (Shizuku.CheckSelfPermission() != (int)Permission.Granted)
                    RequestPermission();
                else
                    Granted = true;
            }
            catch (Exception e)
            {
                if (CheckSelfPermission("moe.shizuku.manager.permission.API_V23") == Permission.Granted)
                    Granted = true;
                if (e.GetType() == typeof(IllegalStateException))
                {
                    Running = false;
                    this.ShowToast("shizuku未运行", ToastLength.Short);
                }
            }
        }
        public void OnRequestPermissionResult(int p0, int p1)
        {
            CheckShizuku();
            PermissionCallback?.Invoke(this);
        }
    }
}