using System;
using Newtonsoft.Json;

namespace ApiServer.Framework.Core.Web
{
    /// <summary>
    /// 错误类
    /// </summary>
    public class Response<T>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:ApiServer.Framework.Core.Mvc.Response`1"/> class.
        /// </summary>
        public Response() { 
        
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:ApiServer.Framework.Core.Mvc.Response`1"/> class.
        /// </summary>
        /// <param name="data">Data.</param>
        public Response(T data)
        {
            this.Data = data;
        }
        /// <summary>
        /// 错误码
        /// </summary>
        /// <value>返回值: 0000=成功 9999=失败 4000=未认证 5000=未处理的异常</value>
        public String Code { get; set; }

        /// <summary>
        /// 提示信息
        /// </summary>
        /// <value>The message.</value>
        public String Message { get; set; }

        /// <summary>
        /// 数据对象
        /// </summary>
        /// <value>The data.</value>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public T Data { get; set; }
    }


}
