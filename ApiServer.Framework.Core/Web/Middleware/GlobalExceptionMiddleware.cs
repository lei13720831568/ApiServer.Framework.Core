using ApiServer.Framework.Core.Exceptions;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace ApiServer.Framework.Core.Web.Middleware
{
    public class GlobalExceptionMiddleware
    {
        private readonly RequestDelegate next;
        private static NLog.ILogger logger = NLog.LogManager.GetCurrentClassLogger();

        public GlobalExceptionMiddleware(RequestDelegate next)
        {
            this.next = next;
        }

        public async Task Invoke(HttpContext context /* other scoped dependencies */)
        {
            try
            {
                await next(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        private static Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            var code = HttpStatusCode.OK; // 200 if unexpected

            //if (exception is MyNotFoundException) code = HttpStatusCode.NotFound;
            //else if (exception is MyUnauthorizedException) code = HttpStatusCode.Unauthorized;
            //else if (exception is MyException) code = HttpStatusCode.BadRequest;
            string msg = "";
            if (exception is AuthException) {
                code = HttpStatusCode.Forbidden;
                msg = $"拒绝访问:{exception}";
                logger.Info(msg);

            } else  if (exception is BizException)
            {
                msg = $"错误:{exception}";
                logger.Info(msg);
            }
            else
            {
                msg = $"异常:{exception}";
                logger.Error($"未处理的异常:{exception}");
            }

            
            var result = JsonSerializer.Serialize(ResponseFactory.CreateUnhandledException(exception));
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)code;
            return context.Response.WriteAsync(result);
        }
    }
}
