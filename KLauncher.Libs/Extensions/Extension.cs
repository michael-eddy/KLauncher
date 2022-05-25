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
using System.Linq;
using Newtonsoft.Json;
using Java.Lang;
using Exception = System.Exception;
using Newtonsoft.Json.Linq;
using Android.Telephony;

namespace KLauncher.Libs
{
    public static class Extension
    {
        public static long ClickTime { get; set; }
        public static JObject ParseJObject(this string json)
        {
            try
            {
                return JObject.Parse(json);
            }
            catch (Exception ex)
            {
                LogManager.Instance.LogError("JObject", ex);
            }
            return default;
        }
        public static T ParseObject<T>(this string json)
        {
            try
            {
                return JsonConvert.DeserializeObject<T>(json);
            }
            catch (Exception ex)
            {
                LogManager.Instance.LogError("ToObject", ex);
            }
            return default;
        }
        public static string OperatorName(this Context context)
        {
            var code = TelephonyManager.FromContext(context).SimOperator;
            return code switch
            {
                "46000" or "46002" or "46004" or "46007" or "46008" => "中国移动",
                "46001" or "46006" or "46009" => "中国联通",
                "46003 " or "46005" or "46011" => "中国电信",
                "46020" => "中国铁通",
                _ => "其他",
            };
        }
        public static bool OpenApp(this Context context, string packageName)
        {
            try
            {
                PackageManager manager = context.PackageManager;
                Intent intent = new Intent(Intent.ActionMain);
                intent.AddCategory(Intent.CategoryLauncher);
                intent.SetPackage(packageName);
                var apps = manager.QueryIntentActivities(intent, 0);
                if (apps.Count > 0)
                {
                    ResolveInfo ri = apps.FirstOrDefault();
                    string className = ri.ActivityInfo.Name;
                    if (className.Split('.').Length > 2)
                    {
                        ComponentName cn = new ComponentName(packageName, className);
                        intent.SetComponent(cn);
                        context.StartActivity(intent);
                    }
                    else
                    {
                        intent = manager.GetLaunchIntentForPackage(packageName);
                        if (intent != null)
                            context.StartActivity(intent);
                    }
                }
                return true;
            }
            catch { }
            return false;
        }
        public static int ToInt32(this object token)
        {
            if (token == null)
                return -1;
            var str = token.ToString();
            try
            {
                if (int.TryParse(str, out int result))
                    return result;
                else
                    return Convert.ToInt32(str);
            }
            catch
            {
                try
                {
                    return Convert.ToInt32(str.Split('.')[0]);
                }
                catch
                {
                    return -1;
                }
            }
        }
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
        public static bool IsFastDoubleClick(this Context _)
        {
            long time = JavaSystem.CurrentTimeMillis();
            long timeD = time - ClickTime;
            if (timeD > 1000)
                ClickTime = time;
            return timeD <= 1000;
        }
        public static bool IsNotEmpty<T>(this T obj)
        {
            return !IsEmpty(obj);
        }
    }
}