using Android.Content.PM;
using Android.Graphics.Drawables;
using Android.Graphics;
using System;
using System.IO;
using Android.Content;
using Android.Widget;
using Android.OS;
using FFImageLoading;
using FFImageLoading.Cache;

namespace KLauncher.Libs
{
    public static class Extension
    {
        public static async void LoadImage(this ImageView imageView, string base64Code)
        {
            try
            {
                await ImageService.Instance.LoadBase64String(base64Code).Retry(2, 1000).WithCache(CacheType.None).IntoAsync(imageView);
            }
            catch (Exception ex)
            {
                LogManager.Instance.LogError("LoadImage", ex);
            }
        }
        public static void ShowToast(this Context context, Exception exc, ToastLength toastLength)
        {
            string msg = string.Format("发生错误：{0}", exc.Message);
            ShowToast(context, msg, toastLength);
        }
        public static void ShowToast(this Context context, string message, ToastLength toastLength)
        {
            try
            {
                if (Looper.MyLooper().IsEmpty())
                    Looper.Prepare();
                Toast.MakeText(context, message, toastLength).Show();
            }
            catch (Exception ex)
            {
                LogManager.Instance.LogError("ShowToast", ex);
            }
        }
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
        public static bool IsEmpty<T>(this T obj)
        {
            try
            {
                if (obj == null) return true;
                if (obj is string)
                    return string.IsNullOrEmpty(obj.ToString());
                else
                    return obj.Equals(default(T));
            }
            catch (Exception ex)
            {
                LogManager.Instance.LogError("IsEmpty", ex);
                return true;
            }
        }
        public static bool IsNotEmpty<T>(this T obj)
        {
            return !IsEmpty(obj);
        }
    }
}