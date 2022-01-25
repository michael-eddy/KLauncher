using KLauncher.Libs.Models;
using SQLite;
using System;
using System.IO;

namespace KLauncher
{
    public sealed class DB : IDisposable
    {
        public SQLiteConnection Connection { get; }
        private static readonly string dbPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "data.db");
        public DB()
        {
            var options = new SQLiteConnectionString(dbPath, true, "MKBWLSAK");
            Connection = new SQLiteConnection(options);
            if (!HasTable())
                Connection.CreateTable<AppItem>(CreateFlags.None);
        }
        private bool HasTable()
        {
            try
            {
                Connection.Table<AppItem>().FirstOrDefault();
                return true;
            }
            catch { }
            return false;
        }
        public void Dispose()
        {
            try
            {
                Connection.Close();
                Connection.Dispose();
            }
            catch { }
        }
    }
}