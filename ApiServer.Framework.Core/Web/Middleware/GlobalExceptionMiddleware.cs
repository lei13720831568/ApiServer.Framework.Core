using ApiServer.Framework.Core.Exceptions;
using ApiServer.Framework.Core.Json;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;
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
            var result = ResponseFactory.CreateUnhandledException(exception);
            if (exception is AuthException) {
                code = HttpStatusCode.Forbidden;
                msg = $"拒绝访问:{exception}";
                result = ResponseFactory.CreateUnauthorized();
                logger.Info(msg);

            } else if (exception is BadRequestException) {
                code = HttpStatusCode.BadRequest;
                msg =$"错误的请求:{exception}";
                result = ResponseFactory.CreateBadRequestException(exception);
                logger.Info(msg);
            } 
            else  if (exception is BizException)
            {
                msg = $"错误:{exception}";
                result = ResponseFactory.CreateUnhandledException(exception);
                logger.Info(msg);
            }
            else
            {
                code = HttpStatusCode.InternalServerError;
                msg = $"异常:{exception}";
                result = ResponseFactory.CreateUnhandledException(exception);
                logger.Error($"未处理的异常:{exception}");
            }

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)code;

            var jsonOption = new JsonSerializerOptions();
            //格式化日期时间格式
            jsonOption.Converters.Add(new DatetimeJsonConverter("yyyy-MM-dd HH:mm:ss"));
            jsonOption.Converters.Add(new DatetimeNullConverter("yyyy-MM-dd HH:mm:ss"));
            //数据格式首字母小写
            jsonOption.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
            //取消Unicode编码
            jsonOption.Encoder = JavaScriptEncoder.Create(UnicodeRanges.All);
            //忽略空值
            jsonOption.IgnoreNullValues = true;
            //允许额外符号
            jsonOption.AllowTrailingCommas = true;
            //反序列化过程中属性名称是否使用不区分大小写的比较
            jsonOption.PropertyNameCaseInsensitive = false;

            return context.Response.WriteAsync(JsonSerializer.Serialize(result,jsonOption));
        }
    }
}
