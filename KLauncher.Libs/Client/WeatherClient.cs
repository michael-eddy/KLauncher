using KLauncher.Libs.Core;
using KLauncher.Libs.Models;
using System;
using System.Globalization;
using System.Threading.Tasks;

namespace KLauncher.Libs.Client
{
    public sealed class WeatherClient : BaseClient
    {
        private const string DB_KEY = "weather";
        private const string API_KEY = "1502b6fccfd45618a92ea9128471c8d5";
        public bool GeCache(out WeatherInfo weatherInfo)
        {
            weatherInfo = default;
            try
            {
                if (SettingHelper.GetData(DB_KEY, out string jsonData) && !string.IsNullOrEmpty(jsonData))
                {
                    var cacheWeather = jsonData.ParseObject<CacheWeatherInfo>();
                    if (DateTime.TryParseExact(cacheWeather.Time, "yyyyMMdd", new CultureInfo("zh-CN"), DateTimeStyles.None, out DateTime dateTime)
                      && dateTime.Date == DateTime.Now.Date && cacheWeather.WeatherInfo != null && cacheWeather.WeatherInfo.Status == 1
                      && cacheWeather.WeatherInfo.Count > 0)
                    {
                        weatherInfo = cacheWeather.WeatherInfo;
                        return true;
                    }
                }
            }
            catch { }
            return false;
        }
        public async Task<string> GetIpAddress()
        {
            try
            {
                var result = await ApiClient.GetResults("http://ip-api.com/json").ConfigureAwait(false);
                var m = result.ParseJObject();
                if (m != null)
                    return m["query"].ToString();
            }
            catch { }
            return string.Empty;
        }
        public async Task<ReturnModel<CityInfo>> GetCityCode(string ip)
        {
            try
            {
                string url = $"https://restapi.amap.com/v3/ip?ip={ip}&key={API_KEY}";
                var result = await ApiClient.GetResults(url).ConfigureAwait(false);
                var m = result.ParseObject<CityInfo>();
                return BuildSuccessResult(m);
            }
            catch (Exception ex)
            {
                return BuildExceptionResult<CityInfo>(ex);
            }
        }
        public async Task<ReturnModel<WeatherInfo>> GetWeatherInfo(string cityNo)
        {
            try
            {
                string url = string.Format("https://restapi.amap.com/v3/weather/weatherInfo?key={0}&city={1}", API_KEY, cityNo);
                var result = await ApiClient.GetResults(url).ConfigureAwait(false);
                var m = result.ParseObject<WeatherInfo>();
                SettingHelper.SaveData(DB_KEY, new CacheWeatherInfo(m).ToString());
                return BuildSuccessResult(m);
            }
            catch (Exception ex)
            {
                return BuildExceptionResult<WeatherInfo>(ex);
            }
        }
    }
}