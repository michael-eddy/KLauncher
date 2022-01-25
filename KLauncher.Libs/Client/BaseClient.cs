using KLauncher.Libs.Core;
using KLauncher.Libs.Models;
using System;

namespace KLauncher.Libs.Client
{
    public abstract class BaseClient
    {
        protected IApiClient ApiClient => Current.Instance.ApiClient;
        protected ReturnModel<T> BuildExceptionResult<T>(Exception exception)
        {
            return new ReturnModel<T>
            {
                Success = false,
                Message = exception.Message
            };
        }
        protected ReturnModel<T> BuildFailedResult<T>(string msg = "")
        {
            return new ReturnModel<T>
            {
                Success = false,
                Message = msg
            };
        }
        protected ReturnModel<T> BuildSuccessResult<T>(T data, string msg = "")
        {
            return new ReturnModel<T>
            {
                Success = true,
                Data = data,
                Message = msg
            };
        }
    }
}