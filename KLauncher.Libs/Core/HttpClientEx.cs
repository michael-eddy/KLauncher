using System;
using System.IO;
using System.IO.Compression;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace KLauncher.Libs.Core
{
    public sealed class HttpClientEx : IApiClient
    {
        private HttpClient HttpClient { get; set; }
        public HttpClientEx()
        {
            HttpClient = new HttpClient();
        }
        public TimeSpan Timeout { get; set; } = TimeSpan.FromSeconds(8);
        public async Task<string> GetResults(string url)
        {
            try
            {
                HttpResponseMessage hr = await HttpClient.GetAsync(url).ConfigureAwait(false);
                hr.EnsureSuccessStatusCode();
                var encodeResults = await hr.Content.ReadAsByteArrayAsync().ConfigureAwait(false);
                return Encoding.UTF8.GetString(encodeResults, 0, encodeResults.Length);
            }
            catch (Exception ex)
            {
                LogManager.Instance.LogError("GetResults", ex);
                return string.Empty;
            }
        }
        public async Task<string> GetResultsGZip(string url)
        {
            try
            {
                HttpResponseMessage hr = await HttpClient.GetAsync(url).ConfigureAwait(false);
                hr.EnsureSuccessStatusCode();
                using var encodeResults = await hr.Content.ReadAsStreamAsync().ConfigureAwait(false);
                using GZipStream zipStream = new GZipStream(encodeResults, CompressionMode.Decompress);
                using StreamReader streamReader = new StreamReader(zipStream, Encoding.UTF8);
                return streamReader.ReadToEnd();
            }
            catch (Exception ex)
            {
                LogManager.Instance.LogError("GetResultsGZip", ex);
                return string.Empty;
            }
        }
        public async Task<string> GetResultsDeflate(string url)
        {
            try
            {
                HttpResponseMessage hr = await HttpClient.GetAsync(url).ConfigureAwait(false);
                hr.EnsureSuccessStatusCode();
                using var encodeResults = await hr.Content.ReadAsStreamAsync().ConfigureAwait(false);
                using DeflateStream deflateStream = new DeflateStream(encodeResults, CompressionMode.Decompress);
                using StreamReader streamReader = new StreamReader(deflateStream, Encoding.UTF8);
                return streamReader.ReadToEnd();
            }
            catch (Exception ex)
            {
                LogManager.Instance.LogError("GetResultsDeflate", ex);
                return string.Empty;
            }
        }
        public async Task<string> GetResultsUTF8Encode(string url)
        {
            try
            {
                HttpResponseMessage hr = await HttpClient.GetAsync(url).ConfigureAwait(false);
                hr.EnsureSuccessStatusCode();
                var encodeResults = await hr.Content.ReadAsByteArrayAsync().ConfigureAwait(false);
                return Encoding.UTF8.GetString(encodeResults, 0, encodeResults.Length);
            }
            catch (Exception ex)
            {
                LogManager.Instance.LogError("GetResultsUTF8Encode", ex);
                return string.Empty;
            }
        }
        public void Dispose()
        {
            try
            {
                if (HttpClient != null)
                    HttpClient.Dispose();
            }
            catch { }
        }
    }
}