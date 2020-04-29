using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;

namespace ApiServer.Framework.Core.Web.Authentication
{
    public interface IAccessTokenStore
    {
        /// <summary>
        /// 将token转换未claim
        /// </summary>
        /// <param name="tokenKey"></param>
        /// <returns>1、用户claim </returns>
        List<Claim> GetTokenToClaims(string tokenKey);
    }
}
