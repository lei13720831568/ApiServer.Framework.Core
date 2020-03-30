using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ApiServer.Framework.Core.Web.Permission
{
    /// <summary>
    /// 资源认证特性
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
    public class AuthCodeAttribute : Attribute, IAuthorizationFilter
    {
        /// <summary>
        /// 权限列表
        /// </summary>
        public List<string> Permissions { get; set; }
        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="code"></param>
        public AuthCodeAttribute(params string[] code)
        {
            Permissions = new List<string>(code);
        }

        /// <summary>
        /// 验证
        /// </summary>
        /// <param name="context"></param>
        public  void OnAuthorization(AuthorizationFilterContext context)
        {
            var authorizationService = context.HttpContext.RequestServices.GetRequiredService<IAuthorizationService>();
            var authorizationResult =  authorizationService.AuthorizeAsync(context.HttpContext.User, null, new AuthCodeRequirement(Permissions.ToList())).Result;
            if (!authorizationResult.Succeeded)
            {
                var result = ResponseFactory.CreateUnauthorized();
                result.Message = "未授权的访问.";
                context.Result = new JsonResult(result);
            }
        }
    }
}
