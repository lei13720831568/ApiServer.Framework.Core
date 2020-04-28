using ApiServer.Framework.Core.Exceptions;
using ApiServer.Framework.Core.Mvc;
using System;
namespace ApiServer.Framework.Core.Web

{
    /// <summary>
    /// Response result.
    /// </summary>
    public static class ResponseResult
    {
        /// <summary>
        /// 成功
        /// </summary>
        public readonly static string Successful = "0000";
        /// <summary>
        /// 失败
        /// </summary>
        public readonly static string Failure = "9999";
        /// <summary>
        /// 未授权
        /// </summary>
        public readonly static string Unauthorized = "4010";
        /// <summary>
        /// 未知异常
        /// </summary>
        public readonly static string UnhandledException = "5000";

        /// <summary>
        /// 错误的请求
        /// </summary>
        public readonly static string BadRequestException = "4000";
    }

    /// <summary>
    /// Response factory.
    /// </summary>
    public static class ResponseFactory
    {
        /// <summary>
        /// Creates the successful.
        /// </summary>
        /// <returns>The successful.</returns>
        /// <param name="data">Data.</param>
        /// <typeparam name="T">The 1st type parameter.</typeparam>
        public static Response<T> CreateSuccessful<T>(T data)
        {
            return new Response<T>
            {
                Code = ResponseResult.Successful,
                Message = "successful",
                Data = data
            };
        }

        /// <summary>
        /// Creates the successful.
        /// </summary>
        /// <returns>The successful.</returns>
        public static Response<object> CreateSuccessful() {
            return CreateSuccessful<object>(null);
        }

        /// <summary>
        /// Creates the failure.
        /// </summary>
        /// <returns>The failure.</returns>
        public static Response<Object> CreateFailure()
        {
            return new Response<Object>
            {
                Code = ResponseResult.Failure,
                Message = "failure",
                Data = null
            };
        }

        /// <summary>
        /// Creates the unauthorized.
        /// </summary>
        /// <returns>The unauthorized.</returns>
        public static Response<Object> CreateUnauthorized()
        {
            return new Response<Object>
            {
                Code = ResponseResult.Unauthorized,
                Message = "unauthorized",
                Data = null
            };
        }
        /// <summary>
        /// Creates the unhandled exception.
        /// </summary>
        /// <returns>The unhandled exception.</returns>
        /// <param name="ex">Ex.</param>
        public static Response<Object> CreateUnhandledException(Exception ex)
        {
            return new Response<Object>
            {
                Code = ResponseResult.UnhandledException,
                Message = $"错误: {ex.Message}",
                Data = null
            };
        }

        public static Response<Object> CreateBadRequestException(Exception ex)
        {
            return new Response<Object>
            {
                Code = ResponseResult.BadRequestException,
                Message = $"错误: {ex.Message}",
                Data = null
            };
        }

      

    }
}
