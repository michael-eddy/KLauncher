using Android.Content;
using KLauncher.Libs;
using KLauncher.Libs.Models;

namespace KLauncher
{
    public sealed class PackageReceiver : BroadcastReceiver
	{
		public override void OnReceive(Context context, Intent intent)
		{
			if (intent.Action.Equals("android.intent.action.PACKAGE_ADDED"))
			{
				string packageName = intent.DataString;
				AppCenter.Instance.UpdateOne(packageName, UpdateType.Add);
			}
			if (intent.Action.Equals("android.intent.action.PACKAGE_REMOVED"))
			{
				string packageName = intent.DataString;
				AppCenter.Instance.UpdateOne(packageName, UpdateType.Remove);
			}
		}
	}
}