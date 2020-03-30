using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace ApiServer.Framework.Core.Web.Permission
{
    /// <summary>
    /// 从session获取用户id
    /// </summary>
    public class SessionCurrentUserProvider : ICurrentUserProvider
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly string _sessionName;

        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="httpContextAccessor">http上下文</param>
        /// <param name="sessionName"></param>
        public SessionCurrentUserProvider(IHttpContextAccessor httpContextAccessor, string sessionName = "user_id") {
            _httpContextAccessor = httpContextAccessor;
            _sessionName = sessionName;
        }

        /// <summary>
        /// 从session获取当前用户id
        /// </summary>
        /// <returns></returns>
        public dynamic GetId()
        {
            return _httpContextAccessor.HttpContext.Session.GetString(_sessionName);
        }
    }
}
