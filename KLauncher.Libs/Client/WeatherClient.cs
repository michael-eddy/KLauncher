using KLauncher.Libs.Models;
using System;
using System.Threading.Tasks;

namespace KLauncher.Libs.Client
{
    public sealed class WeatherClient : BaseClient
    {
        private const string API_KEY = "1502b6fccfd45618a92ea9128471c8d5";
        public async Task<ReturnModel<CityInfo>> GetCityCode()
        {
            try
            {
                string url = string.Format("https://restapi.amap.com/v3/ip?key={0}", API_KEY);
                var result = await ApiClient.GetResultsGZip(url).ConfigureAwait(false);
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
                string url = string.Format("https://restapi.amap.com/v3/weather/weatherInfo?key={0}&city={1}",
                   API_KEY, cityNo);
                var result = await ApiClient.GetResultsGZip(url).ConfigureAwait(false);
                var m = result.ParseObject<WeatherInfo>();
                return BuildSuccessResult(m);
            }
            catch (Exception ex)
            {
                return BuildExceptionResult<WeatherInfo>(ex);
            }
        }
    }
}