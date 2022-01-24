using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Widget;
using KLauncher.Libs;
using System;
using System.Threading;
using System.Threading.Tasks;

#if DEBUG
[Application(Debuggable = true, UsesCleartextTraffic = true)]
#else
[Application(Debuggable = false, UsesCleartextTraffic = true)]
#endif
public class XamApplication : Application
{
    public XamApplication(IntPtr handle, JniHandleOwnership ownerShip) : base(handle, ownerShip) { }
    public override void OnCreate()
    {
        base.OnCreate();
        try
        {
            TaskScheduler.UnobservedTaskException += TaskSchedulerOnUnobservedTaskException;
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            AndroidEnvironment.UnhandledExceptionRaiser += AndroidEnvironment_UnhandledExceptionRaiser;
        }
        catch (Exception ex)
        {
            LogManager.Instance.LogError("Register", ex);
        }
    }
    private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
    {
        if (e.ExceptionObject is Exception exc)
            LogManager.Instance.LogError("TaskSchedulerOnUnobservedTaskException", exc);
    }
    private void TaskSchedulerOnUnobservedTaskException(object sender, UnobservedTaskExceptionEventArgs e)
    {
        LogManager.Instance.LogError("TaskSchedulerOnUnobservedTaskException", e.Exception);
    }
    private void AndroidEnvironment_UnhandledExceptionRaiser(object sender, RaiseThrowableEventArgs e)
    {
        LogManager.Instance.LogError("UnhandledExceptionRaiser", e.Exception);
        Context.ShowToast("很抱歉,程序出现未知异常,即将退出", ToastLength.Long);
        Thread.Sleep(2000);
        e.Handled = true;
    }
}