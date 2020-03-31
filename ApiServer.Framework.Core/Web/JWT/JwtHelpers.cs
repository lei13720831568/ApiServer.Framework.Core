using ApiServer.Framework.Core.Settings;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ApiServer.Framework.Core.Web.JWT
{
    public class JwtHelpers
    {
        private readonly IOptionsSnapshot<JWTSettings> jWTSettings;

        public JwtHelpers(IOptionsSnapshot<JWTSettings> jwt)
        {
            jWTSettings = jwt;
        }

        public string GenerateToken(string userName,dynamic uId, string jti=null)
        {
            var settings = jWTSettings.Value;
            if (string.IsNullOrEmpty(jti)) jti = Guid.NewGuid().ToString("N");
            var claims = new List<Claim>();

            claims.Add(new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())); // JWT ID

            claims.Add(new Claim(JwtRegisteredClaimNames.Sub, uId));

            claims.Add(new Claim("Name", userName));

            var userClaimsIdentity = new ClaimsIdentity(claims);

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(settings.SecurityKey));

            // HmacSha256 要 16 byte以上
            var signingCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256Signature);

            // 建立 SecurityTokenDescriptor
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Issuer = settings.Issuer,
                Audience = settings.Audience, 
                Subject = userClaimsIdentity,
                Expires = DateTime.Now.AddSeconds(settings.LifeTimeSeconds),
                SigningCredentials = signingCredentials
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var securityToken = tokenHandler.CreateToken(tokenDescriptor);
            var serializeToken = tokenHandler.WriteToken(securityToken);

            return serializeToken;
        }
    }
}
