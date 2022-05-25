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
    {private bool Clickable { get; }
        private Context Context { get; }
        private List<AppItem> Apps { get; }
        public event CallbackObject ItemClick;
        public event CallbackViewObject ItemLongClick;
        public AppItemAdapter(Context context, List<AppItem> items, bool clickable)
        {
            Apps = items;
            Context = context;
            Clickable = clickable;
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
                if (Clickable)
                {
                    convertView.Click += (_, _) =>
                    {
                        var position = ((ViewHolderBox)convertView.Tag).Position;
                        ItemClick?.Invoke(position);
                    };
                    convertView.LongClick += (_, _) =>
                    {
                        var position = ((ViewHolderBox)convertView.Tag).Position;
                        ItemLongClick?.Invoke(convertView, position);
                    };
                }
                convertView.Tag = itemHolder;
            }
            else
            {
                itemHolder = (ViewHolderBox)convertView.Tag;
            }
            try
            {
                var item = Apps.ElementAt(position);
                itemHolder.Position = position;
                itemHolder.DisplayIcon.LoadImage(item.Icon);
                itemHolder.PackageName = item.PackageName;
                itemHolder.DisplayName.SetText(item.DisplayName, TextView.BufferType.Normal);
            }
            catch { }
            return convertView;
        }
    }
}