using Android.OS;
using Java.Lang;

namespace KLauncher.Tasks
{
    public sealed class MemoryThread : Thread
    {
        private Handler Handler { get; }
        public MemoryThread(Handler handler)
        {
            Handler = handler;
        }
        public override void Run()
        {
            try
            {
                while (true)
                {
                    Handler.SendEmptyMessage(0);
                    Sleep(500);
                }
            }
            catch { }
        }
    }
}