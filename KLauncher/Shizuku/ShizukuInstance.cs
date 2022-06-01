using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Widget;
using Java.IO;
using Java.Lang;
using KLauncher.Libs;
using Rikka.Shizuku;

namespace KLauncher
{
    public sealed class ShizukuInstance : Object, Shizuku.IOnRequestPermissionResultListener
    {
        private bool disposing;
        private Context Context { get; }
        public bool Granted { get; private set; }
        public bool Running { get; private set; }
        public StringBuilder OutputMsg { get; }
        private ShizukuHandler Handler { get; }
        private static ShizukuInstance instance;
        public static ShizukuInstance NewInstance(Context context)
        {
            if (instance == null)
                instance = new ShizukuInstance(context);
            return instance;
        }
        private ShizukuInstance(Context context)
        {
            Context = context;
            OutputMsg = new StringBuilder();
            Handler = new ShizukuHandler(this);
            Shizuku.AddRequestPermissionResultListener(this);
        }
        public void OnRequestPermissionResult(int p0, int p1)
        {
            try
            {
                if (Shizuku.CheckSelfPermission() != (int)Permission.Granted)
                    Shizuku.RequestPermission(0);
                else
                    Granted = true;
            }
            catch (Exception e)
            {
                if (Context.CheckSelfPermission("moe.shizuku.manager.permission.API_V23") == Permission.Granted)
                    Granted = true;
                if (e.GetType() == typeof(IllegalStateException))
                {
                    Running = false;
                    Context.ShowToast("shizuku未运行", ToastLength.Short);
                }
            }
        }
        public void ShizukuExec(string cmd)
        {
            try
            {
#pragma warning disable CS0618
                var p = Shizuku.NewProcess(new string[] { "sh" }, null, null);
                p.OutputStream.Write((cmd + "\nexit\n").GetBytes());
                p.OutputStream.Flush();
                p.OutputStream.Close();
                var h2 = new Thread(() =>
                {
                    try
                    {
                        BufferedReader mReader = new BufferedReader(new InputStreamReader(p.InputStream));
                        string inline;
                        while ((inline = mReader.ReadLine()) != null)
                        {
                            if (OutputMsg.Length() > 1000 || disposing) 
                                break;
                            Message msg = new Message
                            {
                                What = 0,
                                Obj = string.IsNullOrEmpty(inline) ? "\n" : inline + "\n"
                            };
                            Handler.SendMessage(msg);
                        }
                        mReader.Close();
                    }
                    catch (Exception ex)
                    {
                        LogManager.Instance.LogError("", ex);
                    }
                });
                h2.Start();
                var h3 = new Thread(() =>
                {
                    try
                    {
                        var mReader = new BufferedReader(new InputStreamReader(p.ErrorStream));
                        string inline;
                        while ((inline = mReader.ReadLine()) != null)
                        {
                            if (OutputMsg.Length() > 1000 || disposing)
                                break;
                            Message msg = new Message
                            {
                                What = 1,
                                Obj = string.IsNullOrEmpty(inline) ? null : $"<font color='red'>{inline}</>"
                            };
                            Handler.SendMessage(msg);
                        }
                        mReader.Close();
                    }
                    catch (Exception ex)
                    {
                        LogManager.Instance.LogError("", ex);
                    }
                    p.WaitFor();
                });
                h3.Start();
#pragma warning restore CS0618 
            }
            catch (Exception ex)
            {
                LogManager.Instance.LogError("ShizukuExec", ex);
            }
        }
        protected override void Dispose(bool disposing)
        {
            Shizuku.RemoveRequestPermissionResultListener(this);
            this.disposing = disposing;
            base.Dispose(disposing);
        }
    }
}