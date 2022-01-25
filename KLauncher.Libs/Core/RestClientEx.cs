using RestSharp;
using System;
using System.IO;
using System.IO.Compression;
using System.Text;
using System.Threading.Tasks;

namespace KLauncher.Libs.Core
{
    public sealed class RestClientEx : IApiClient
    {
        private RestClient Client { get; }
        internal RestClientEx()
        {
            Client = new RestClient();
        }
        public async Task<string> GetResults(string url)
        {
            try
            {
                var request = new RestRequest(url, Method.Get);
                var result = await Client.ExecuteAsync(request);
                if (result.IsSuccessful)
                    return result.Content;
            }
            catch (Exception ex)
            {
                LogManager.Instance.LogError("GetResults", ex);
            }
            return string.Empty;
        }
        public async Task<string> GetResultsGZip(string url)
        {
            try
            {
                var request = new RestRequest(url, Method.Get);
                var result = await Client.ExecuteAsync(request);
                if (result.IsSuccessful)
                {
                    using MemoryStream stream = new MemoryStream(result.RawBytes);
                    using GZipStream zipStream = new GZipStream(stream, CompressionMode.Decompress);
                    using StreamReader streamReader = new StreamReader(zipStream, Encoding.UTF8);
                    return streamReader.ReadToEnd();
                }
            }
            catch (Exception ex)
            {
                LogManager.Instance.LogError("GetResultsGZip", ex);
            }
            return string.Empty;
        }
        public async Task<string> GetResultsDeflate(string url)
        {
            try
            {
                var request = new RestRequest(url, Method.Get);
                var result = await Client.ExecuteAsync(request);
                if (result.IsSuccessful)
                {
                    using MemoryStream stream = new MemoryStream(result.RawBytes);
                    using DeflateStream deflateStream = new DeflateStream(stream, CompressionMode.Decompress);
                    using StreamReader streamReader = new StreamReader(deflateStream, Encoding.UTF8);
                    return streamReader.ReadToEnd();
                }
            }
            catch (Exception ex)
            {
                LogManager.Instance.LogError("GetResultsDeflate", ex);
            }
            return string.Empty;
        }
        public async Task<string> GetResultsUTF8Encode(string url)
        {
            try
            {
                var request = new RestRequest(url, Method.Get);
                var result = await Client.ExecuteAsync(request);
                var encodeResults = result.RawBytes;
                return Encoding.UTF8.GetString(encodeResults, 0, encodeResults.Length);
            }
            catch (Exception ex)
            {
                LogManager.Instance.LogError("GetResultsUTF8Encode", ex);
                return string.Empty;
            }
        }
        public void Dispose() { }
    }
}