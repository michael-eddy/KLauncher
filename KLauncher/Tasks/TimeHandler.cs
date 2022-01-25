using Android.OS;

namespace KLauncher.Tasks
{
    public sealed class TimeHandler : Handler
    {
        private MainActivity Activity { get; }
        public TimeHandler(MainActivity activity) : base(activity.MainLooper)
        {
            Activity = activity;
        }
        public override void HandleMessage(Message msg)
        {
            Bundle bundle = msg.Data;
            Activity.TextViewTime.Text = bundle.GetString("time");
        }
    }
}