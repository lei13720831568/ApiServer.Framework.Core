using ApiServer.Framework.Core.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ApiServer.Framework.Core.Extensions
{
    public static class ControllerExtensions
    {
        /// <summary>
        /// 获取当前登录用户的id
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static int GetCurrentUserId(this ControllerBase input) {
            if (input.HttpContext.User == null) throw new BizException("用户未通过认证");
            return int.Parse(input.HttpContext.User.Claims.FirstOrDefault(p => p.Type == "user_id").Value);
        }
    }
}
