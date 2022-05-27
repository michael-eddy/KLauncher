using Android.OS;
using KLauncher.Libs;

namespace KLauncher.Tasks
{
    public sealed class MemoryHandler : Handler
    {
        private CleanDialog Dialog { get; }
        public MemoryHandler(CleanDialog dialog)
        {
            Dialog = dialog;
        }
        public override void HandleMessage(Message msg)
        {
            base.HandleMessage(msg);
            switch (msg.What)
            {
                case 0:
                    Dialog.TextViewUse.Text = $"{(int)((Dialog.TotalMemory - Dialog.Activity.GetAvailMemory()) / 1048576)}MB已用";
                    break;
                case 1:
                    Dialog.ButtonClear.Enabled = true;
                    break;
            }
        }
    }
}