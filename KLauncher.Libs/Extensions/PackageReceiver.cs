using Android.Content;
using KLauncher.Libs.Models;
using System;
using System.Threading.Tasks;

namespace KLauncher.Libs
{
	public sealed class PackageReceiver : BroadcastReceiver
	{
		public override void OnReceive(Context context, Intent intent)
		{
			try
			{
				string packageName = intent.DataString.Replace("package:", "");
				var updateType = intent.Action.Equals("android.intent.action.PACKAGE_ADDED", StringComparison.CurrentCultureIgnoreCase)
					? UpdateType.Add : UpdateType.Remove;
				Task.Factory.StartNew(() =>
				{
					AppCenter.Instance.UpdateOne(packageName, updateType);
				});
			}
			catch (Exception ex)
			{
				LogManager.Instance.LogError("PackageReceiver", ex);
			}
		}
	}
}