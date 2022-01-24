using Android.Content;
using Android.Views;
using Android.Widget;
using KLauncher.Libs;
using KLauncher.Libs.Models;
using System.Collections.Generic;
using System.Linq;
using JavaObject = Java.Lang.Object;

namespace KLauncher.Adapters
{
    public sealed class AppItemAdapter : BaseAdapter
    {
        private Context Context { get; }
        private List<AppItem> Apps { get; }
        public event CallbackObject ItemClick;
        public AppItemAdapter(Context context, List<AppItem> items)
        {
            Apps = items;
            Context = context;
        }
        public override int Count => Apps == null ? 0 : Apps.Count;
        public override JavaObject GetItem(int position) => Apps?.ElementAt(position);
        public override long GetItemId(int position) => Apps == null ? 0 : Apps.ElementAt(position).Id;
        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            ViewHolderBox itemHolder;
            if (convertView == null)
            {
                convertView = LayoutInflater.From(Context).Inflate(Resource.Layout.app_item_view, null);
                itemHolder = new ViewHolderBox
                {
                    DisplayIcon = convertView.FindViewById<ImageView>(Resource.Id.displayIcon),
                    DisplayName = convertView.FindViewById<TextView>(Resource.Id.displayName)
                };
                convertView.Click += (s, ee) =>
                {
                    ItemClick?.Invoke(itemHolder);
                };
                convertView.Tag = itemHolder;
            }
            else
            {
                itemHolder = (ViewHolderBox)convertView.Tag;
            }
            try
            {
                var item = Apps.ElementAt(position);
                itemHolder.DisplayIcon.LoadImage(item.Icon);
                itemHolder.PackageName = item.PackageName;
                itemHolder.DisplayName.SetText(item.DisplayName, TextView.BufferType.Normal);
            }
            catch { }
            return convertView;
        }
    }
}