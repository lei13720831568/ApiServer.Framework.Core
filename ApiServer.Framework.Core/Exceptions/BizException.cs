using ApiServer.Framework.Core.Web;
using System;
namespace ApiServer.Framework.Core.Exceptions
{
    /// <summary>
    /// 业务异常基类异常处理
    /// </summary>
    public class BizException:Exception
    {
        /// <summary>
        /// 默认构造
        /// </summary>
        /// <param name="msg"></param>
        public BizException(string msg):base(msg)
        {
            Id = System.Guid.NewGuid().ToString("N");
            ReturnCode = ResponseResult.UnhandledException;
            ExceptionData = null;
        }

        public BizException(string msg,string returnCode) : base(msg)
        {
            Id = System.Guid.NewGuid().ToString("N");
            ReturnCode = returnCode;
            ExceptionData = null;
        }

        public BizException(string msg, string returnCode,Object data) : base(msg) {
            Id = System.Guid.NewGuid().ToString("N");
            ReturnCode = returnCode;
            ExceptionData = data;
        }

        /// <summary>
        /// 异常id
        /// </summary>
        public String Id { get; set; }

        public String ReturnCode { get; set; }

        public object ExceptionData { get; set; }
    }
}
