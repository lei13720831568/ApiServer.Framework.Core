using ApiServer.Framework.Core.Exceptions;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace ApiServer.Framework.Core.Web.Permission
{
    /// <summary>
    /// 从jwt获取当前用户id
    /// </summary>
    public class JWTCurrentUserProvider:ICurrentUserProvider
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly string _secret;

        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="httpContextAccessor">http上下文</param>
        /// <param name="secret">JWT 密钥</param>
        public JWTCurrentUserProvider(IHttpContextAccessor httpContextAccessor,string secret)
        {
            _httpContextAccessor = httpContextAccessor;
            _secret = secret;
            
        }

        /// <summary>
        /// 从jwt字符串获取id
        /// </summary>
        /// <returns></returns>
        public dynamic GetId()
        {
            string  authStr = _httpContextAccessor.HttpContext.Request.Headers["Authorization"];
            //没有jwt信息
            if (string.IsNullOrEmpty(authStr)) {
                return null;
            }

            if (!authStr.StartsWith("Bearer ")) {
                return null;
            }

            if (authStr.Length < 8)
            {
                return null;
            }
            
            authStr = authStr.Substring(7);

            var token = Token.From(authStr, _secret);

            //过期了
            if (token.Payload.Exp < System.DateTime.Now) {
                return null;
            }
            return token.Payload.Uid;

        }
    }
}
