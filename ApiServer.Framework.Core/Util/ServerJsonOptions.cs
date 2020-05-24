using ApiServer.Framework.Core.Json;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;

namespace ApiServer.Framework.Core.Util
{
    public class ServerJsonOptions
    {
        public static JsonSerializerOptions Get() {

            var options =  new JsonSerializerOptions();
            //格式化日期时间格式
            options.Converters.Add(new DatetimeJsonConverter("yyyy-MM-dd HH:mm:ss"));
            options.Converters.Add(new DatetimeNullConverter("yyyy-MM-dd HH:mm:ss"));
            //数据格式首字母小写
            options.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
            //取消Unicode编码
            options.Encoder = JavaScriptEncoder.Create(UnicodeRanges.All);
            //忽略空值
            options.IgnoreNullValues = true;
            //允许额外符号
            options.AllowTrailingCommas = true;
            //反序列化过程中属性名称是否使用不区分大小写的比较
            options.PropertyNameCaseInsensitive = false;
            return options;
        }
    }
}
