using System;
using System.Collections.Generic;
using System.Text;

namespace ApiServer.Framework.Core.Exceptions
{
    /// <summary>
    /// jwt方面异常
    /// </summary>
    public class JWTException : AuthException
    {
        /// <summary>
        /// jwt方面异常
        /// </summary>
        /// <param name="msg"></param>
        public JWTException(string msg) : base(msg)
        {
        }
    }
}
