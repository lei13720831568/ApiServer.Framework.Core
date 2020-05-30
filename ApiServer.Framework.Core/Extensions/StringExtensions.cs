using System;
using ApiServer.Framework.Core.Exceptions;

namespace ApiServer.Framework.Core.Extensions
{
    public static class StringExtensions
    {
        public static T TryDeserializeObject<T>(this string value,string errMsg)
        {
            T result;
            try
            {
                result = Newtonsoft.Json.JsonConvert.DeserializeObject<T>(value);
            }
            catch
            {
                throw new BizException(errMsg);
            }
            return result;
        }
    }
}
