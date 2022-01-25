using System;
using System.Collections.Generic;

namespace KLauncher.Libs.Models
{
    public sealed class WeatherInfo
    {
        public int Status { get; set; }
        public int Count { get; set; }
        public string Info { get; set; }
        public string InfoCode { get; set; }
        public IEnumerable<WeatherInfoLive> Lives { get; set; }
    }
    public sealed class WeatherInfoLive
    {
        public string Province { get; set; }
        public string City { get; set; }
        public string Adcode { get; set; }
        public string Weather { get; set; }
        public string Temperature { get; set; }
        public string WindDirection { get; set; }
        public string WindPower { get; set; }
        public string HumIdity { get; set; }
        public DateTime ReportTime { get; set; }
    }
}