using System;
using System.Collections.Generic;
using System.Text;

namespace ApiServer.Framework.Core.Exceptions
{
    public class BadRequestException : BizException
    {
        public BadRequestException(string msg) : base(msg)
        {
        }
    }
}
