using System;
using System.Collections.Generic;
using System.Text;

namespace ApiServer.Framework.Core.Exceptions
{
    /// <summary>
    /// 权限相关异常
    /// </summary>
    public class AuthException : BizException
    {
        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="msg"></param>
        public AuthException(string msg) : base(msg)
        {
        }
    }
}
