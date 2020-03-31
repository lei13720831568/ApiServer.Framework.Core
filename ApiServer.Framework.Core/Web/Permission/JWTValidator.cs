using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ApiServer.Framework.Core.Web.Permission
{
    public class JWTValidator : ISecurityTokenValidator
    {
        public JWTValidator() {
            MaximumTokenSizeInBytes = 8192;
        }

        public bool CanValidateToken => true;

        public int MaximumTokenSizeInBytes { get; set; }


        public bool CanReadToken(string securityToken)
        {
            //没有jwt信息
            if (string.IsNullOrEmpty(securityToken))
            {
                return false;
            }

            new JwtSecurityTokenHandler().ReadJwtToken(securityToken);
            return true;

        }

        /// <summary>
        /// jwt验证器
        /// </summary>
        /// <param name="securityToken"></param>
        /// <param name="validationParameters"></param>
        /// <param name="validatedToken"></param>
        /// <returns></returns>
        public virtual ClaimsPrincipal ValidateToken(string securityToken, TokenValidationParameters validationParameters, out SecurityToken validatedToken)
        {

            var handler = new JwtSecurityTokenHandler();
            //可以重载进行jwt信息的后端验证
            //throw new SecurityTokenException("Token not recognized");

            //jwt自身验证
            var result= handler.ValidateToken(securityToken, validationParameters, out validatedToken);
            return result;

        }
    }
}
