using Android.Content;
using KLauncher.Libs.Models;

namespace KLauncher.Libs
{
	public sealed class PackageReceiver : BroadcastReceiver
	{
		public override void OnReceive(Context context, Intent intent)
		{
			string packageName = intent.DataString.Replace("package:", "");
			var updateType = intent.Action.Equals("android.intent.action.PACKAGE_ADDED") ? UpdateType.Add : UpdateType.Remove;
			AppCenter.Instance.UpdateOne(packageName, updateType);
		}
	}
}