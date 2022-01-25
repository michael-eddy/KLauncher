using Serilog;
using Serilog.Core;
using System;
using System.IO;

namespace KLauncher.Libs
{
    public sealed class LogManager
    {
        private static LogManager _Instance;
        public static LogManager Instance
        {
            get
            {
                if (_Instance == null)
                    _Instance = new LogManager();
                return _Instance;
            }
        }
        private Logger Logger { get; }
        private LogManager()
        {
            var LogFile = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "launcher_log", "log-.txt");
            Logger = new LoggerConfiguration().MinimumLevel.Debug()
                .WriteTo.File(LogFile, rollingInterval: RollingInterval.Day)
                .CreateLogger();
        }
        public void LogError(string messageTemplate, Exception exception)
        {
            try
            {
                if (exception != null)
                    Logger.Error(exception, messageTemplate);
            }
            catch { }
        }
        public void LogInfo(string messageTemplate)
        {
            try
            {
                Logger.Information(messageTemplate);
            }
            catch { }
        }
        public void LogInfo(string messageTemplate, params object[] propertyValues)
        {
            try
            {
                Logger.Information(messageTemplate, propertyValues);
            }
            catch { }
        }
    }
}