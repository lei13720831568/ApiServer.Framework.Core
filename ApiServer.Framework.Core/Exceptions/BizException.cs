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
        }

        /// <summary>
        /// 异常id
        /// </summary>
        public String Id { get; set; }
    }
}
