using KLauncher.Libs.Models;
using System;

namespace KLauncher.Libs.Core
{
    public sealed class SettingHelper
    {
        public static bool ShowHidden
        {
            get
            {
                try
                {
                    GetData("ShowHidden", out string value);
                    return value == "1";
                }
                catch { return false; }
            }
            set
            {
                SaveData("ShowHidden", value ? "1" : "0");
            }
        }
        public static bool UseRest
        {
            get
            {
                try
                {
                    GetData("UseRest", out string value);
                    return value == "1";
                }
                catch { return false; }
            }
            set
            {
                SaveData("UseRest", value ? "1" : "0");
            }
        }
        public static bool ShowSec
        {
            get
            {
                try
                {
                    GetData("ShowSec", out string value);
                    return value == "1";
                }
                catch { return false; }
            }
            set
            {
                SaveData("ShowSec", value ? "1" : "0");
            }
        }
        public static uint CleanThread
        {
            get
            {
                try
                {
                    GetData("CleanThread", out string value);
                    uint.TryParse(value, out uint resullt);
                    if (resullt <= 0) resullt = 1;
                    return resullt;
                }
                catch { return 1; }
            }
            set
            {
                SaveData("CleanThread", value.ToString());
            }
        }
        public static double CleanPercent
        {
            get
            {
                try
                {
                    GetData("CleanPercent", out string value);
                    double.TryParse(value, out double resullt);
                    if (resullt <= 0) resullt = 0.6;
                    return resullt;
                }
                catch { return 0.6; }
            }
            set
            {
                SaveData("CleanPercent", value.ToString());
            }
        }
        public static byte[] Background
        {
            get
            {
                if (GetData("Background", out string base64String))
                    return Convert.FromBase64String(base64String);
                else
                    return default;
            }
            set
            {
                try
                {
                    var base64String = Convert.ToBase64String(value);
                    SaveData("Background", base64String);
                }
                catch { }
            }
        }
        internal static bool GetData(string name, out string value)
        {
            try
            {
                using (DB db = new DB())
                {
                    var entity = db.Connection.Table<Setting>().FirstOrDefault(x => x.Name == name);
                    value = entity?.Value;
                }
                if (string.IsNullOrEmpty(value))
                    value = string.Empty;
                return true;
            }
            catch (Exception ex)
            {
                value = string.Empty;
                LogManager.Instance.LogError("GetData", ex);
                return false;
            }
        }
        internal static void SaveData(string name, string value)
        {
            try
            {
                using DB db = new DB();
                var entity = db.Connection.Table<Setting>().FirstOrDefault(x => x.Name == name);
                if (entity.IsNotEmpty())
                {
                    entity.Value = value;
                    db.Connection.Update(entity);
                }
                else
                {
                    entity = new Setting
                    {
                        Value = value,
                        Name = name
                    };
                    db.Connection.Insert(entity);
                }
            }
            catch (Exception ex)
            {
                LogManager.Instance.LogError("SaveData", ex);
            }
        }
    }
}