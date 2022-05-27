using System;

namespace KLauncher.Libs.Models
{
    public sealed class CacheWeatherInfo
    {
        public CacheWeatherInfo(WeatherInfo m)
        {
            WeatherInfo = m;
            Time = DateTime.Now.ToString("yyyyMMdd");
        }
        public string Time { get; set; }
        public WeatherInfo WeatherInfo { get; set; }
        public override string ToString() => this.ToJson();
    }
}