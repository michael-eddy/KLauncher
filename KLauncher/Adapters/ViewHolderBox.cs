using Android.Widget;
using JavaObject = Java.Lang.Object;

namespace KLauncher.Adapters
{
    public sealed class ViewHolderBox : JavaObject
    {
        public string PackageName { get; set; }
        public ImageView DisplayIcon { get; set; }
        public TextView DisplayName { get; set; }
    }
}