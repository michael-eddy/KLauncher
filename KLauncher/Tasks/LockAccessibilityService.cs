using Android;
using Android.AccessibilityServices;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Views.Accessibility;

namespace KLauncher
{
    [IntentFilter(new[] { "android.accessibilityservice" })]
    [Service(Permission = Manifest.Permission.BindAccessibilityService)]
    [MetaData("android.accessibilityservice.AccessibilityService", Resource = "@xml/lock_accessibility_service_config")]
    public class LockAccessibilityService : AccessibilityService
    {
        public const string ACTION_LOCK = "com.michael.klauncher.LOCK";
        public override void OnAccessibilityEvent(AccessibilityEvent e) { }
        public override void OnInterrupt() { }
        public override StartCommandResult OnStartCommand(Intent intent, StartCommandFlags flags, int startId)
        {
            if (ACTION_LOCK.Equals(intent.Action))
            {
                if (Build.VERSION.SdkInt >= BuildVersionCodes.P)
                    PerformGlobalAction(GlobalAction.LockScreen);
            }
            return StartCommandResult.Sticky;
        }
    }
}