using KLauncher.Libs.Models;
using System;

namespace KLauncher.Libs.Core
{
    public sealed class SettingHelper
    {
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
        private static bool GetData(string name, out string value)
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
        private static void SaveData(string name, string value)
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