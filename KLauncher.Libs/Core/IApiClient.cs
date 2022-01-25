using System;
using System.Threading.Tasks;

namespace KLauncher.Libs.Core
{
    public interface IApiClient : IDisposable
    {
        Task<string> GetResults(string url);
        Task<string> GetResultsGZip(string url);
        Task<string> GetResultsDeflate(string url);
        Task<string> GetResultsUTF8Encode(string url);
    }
}