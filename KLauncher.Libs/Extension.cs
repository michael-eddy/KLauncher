using Android.Content.PM;
using Android.Graphics.Drawables;
using Android.Graphics;
using System;
using System.IO;

namespace KLauncher.Libs
{
    public static class Extension
    {
        public static bool IsSystem(this ApplicationInfo applicationInfo)
        {
            try
            {
                var b1 = (applicationInfo.Flags & ApplicationInfoFlags.System) != 0;
                var b2 = (applicationInfo.Flags & ApplicationInfoFlags.UpdatedSystemApp) != 0;
                return b1 || b2;
            }
            catch { }
            return false;
        }
        public static string ToBas64Code(this Drawable drawable)
        {
            string base64Code = string.Empty;
            try
            {
                Bitmap bitmap = Bitmap.CreateBitmap(drawable.IntrinsicWidth, drawable.IntrinsicHeight,
                           drawable.Opacity != -1 ? Bitmap.Config.Argb8888 : Bitmap.Config.Rgb565);
                Canvas canvas = new Canvas(bitmap);
                drawable.SetBounds(0, 0, drawable.IntrinsicWidth, drawable.IntrinsicHeight);
                drawable.Draw(canvas);
                using var ms = new MemoryStream();
                bitmap.Compress(Bitmap.CompressFormat.Png, 100, ms);
                base64Code = $"data:image/png;base64,{Convert.ToBase64String(ms.ToArray())}";
            }
            catch (Exception ex)
            {
                LogManager.Instance.LogError("ToByteArray", ex);
            }
            return base64Code;
        }
    }
}