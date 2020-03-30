using ApiServer.Framework.Core.Exceptions;
using ApiServer.Framework.Core.Settings;
using ApiServer.Framework.Core.Web.JWT;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace ApiServer.Framework.Core.Web.Permission
{
    public class Token
    {
        /// <summary>
        /// 创建凭证
        /// </summary>
        /// <param name="secret">秘钥</param>
        /// <param name="lifeTime">生命周期 单位秒</param>
        /// <param name="iss">签发人</param>
        /// <param name="sub">主题</param>
        /// <param name="uid">用户id</param>
        /// <param name="jti">唯一编号</param>
        /// <returns></returns>
        public static Token CreateToken(JWTSettings setting, dynamic uid , string jti) {
            Token t = new Token();
            t.Header = new Header();
            var createAt = System.DateTime.Now;
            t.Payload = new Payload { 
                 Uid=uid, Aud= setting.Audience, Iat= createAt, Iss =setting.Issuer, Jti=jti, Nbf = createAt, Sub = setting.Subject
            };
            var header_str = JsonSerializer.Serialize<Header>(t.Header).Base64();
            var payload_str = JsonSerializer.Serialize<Payload>(t.Payload).Base64();
            t.Signature = CalSign(setting.SecurityKey, $"{header_str}.{payload_str}");

            return t;

        }
        /// <summary>
        /// header
        /// </summary>
        public Header Header;

        /// <summary>
        /// payload
        /// </summary>
        public Payload Payload;

        /// <summary>
        /// sign
        /// </summary>
        public string Signature;

        /// <summary>
        /// 转换为jwt对象
        /// </summary>
        /// <param name="secret"></param>
        /// <returns></returns>
        public string ToJWT(string secret)
        {
            var header_str = JsonSerializer.Serialize<Header>(Header).Base64();
            var payload_str = JsonSerializer.Serialize<Payload>(Payload).Base64();
            var sig = CalSign(secret,$"{header_str}.{payload_str}");
            return $"{header_str}.{payload_str}.{sig}";
        }

        private static string CalSign(string secret,string body) {
            return body.Hs256(secret);
        }


        private  static string Base64Decode(string content)
        {
            byte[] bytes = Convert.FromBase64String(content);
            return Encoding.UTF8.GetString(bytes);
        }

        /// <summary>
        /// 从jwt字符串得到token对象
        /// </summary>
        /// <param name="jwt"></param>
        /// <param name="secret"></param>
        /// <returns></returns>
        public static Token From(string jwt,string secret) {
            string[] strs = jwt.Split('.');
            if (strs.Length != 3) {
                throw new JWTException("jwt不正确");
            }

            var calSig= $"{strs[0]}.{strs[1]}".Hs256(secret);
            if (calSig != strs[3]) {
                throw new JWTException("jwt签名错误");
            }

            try
            {
                var token = new Token();
                var header = Base64Decode(strs[0]);
                var payload = Base64Decode(strs[1]);
                token.Header = JsonSerializer.Deserialize<Header>(header);
                token.Payload = JsonSerializer.Deserialize<Payload>(payload);
                token.Signature = calSig;
                return token;
            }
            catch (Exception ex) {
                throw new JWTException($"jwt解析失败,{ex.Message}");
            }


        }
    }
}
