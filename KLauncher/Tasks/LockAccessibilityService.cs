using Android.AccessibilityServices;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Views.Accessibility;

namespace KLauncher
{
    public class LockAccessibilityService : AccessibilityService
    {
        public override void OnAccessibilityEvent(AccessibilityEvent e) { }
        public override void OnInterrupt() { }
        public override StartCommandResult OnStartCommand(Intent intent, StartCommandFlags flags, int startId)
        {
            if (Build.VERSION.SdkInt >= BuildVersionCodes.P)
                PerformGlobalAction(GlobalAction.LockScreen);
            return StartCommandResult.Sticky;
        }
    }
}