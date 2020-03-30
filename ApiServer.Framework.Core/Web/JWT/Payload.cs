using System;
using System.Collections.Generic;
using System.Text;

namespace ApiServer.Framework.Core.Web.JWT
{
    public class Payload
    {
        /// <summary>
        /// 签发人
        /// </summary>
        public string Iss { get; set; }
        /// <summary>
        /// 过期时间
        /// </summary>
        public DateTime Exp { get; set; }

        /// <summary>
        /// 主题
        /// </summary>
        public string Sub { get; set; }
        /// <summary>
        /// 受众
        /// </summary>
        public string Aud { get; set; }
        /// <summary>
        /// 生效时间
        /// </summary>
        public DateTime Nbf { get; set; }
        /// <summary>
        /// 签发时间
        /// </summary>
        public DateTime Iat { get; set; }
        /// <summary>
        /// 编号
        /// </summary>
        public string Jti { get; set; }

        /// <summary>
        /// 用户id
        /// </summary>
        public string Uid { get; set; }
    }
}
