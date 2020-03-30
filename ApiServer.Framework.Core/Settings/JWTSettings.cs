using System;
namespace ApiServer.Framework.Core.Settings
{
    /// <summary>
    /// JWTS ettings.
    /// </summary>
    public class JWTSettings
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:ApiServer.Framework.Core.Settings.JWTSettings"/> class.
        /// </summary>
        public JWTSettings()
        {
        }

        /// <summary>
        /// Gets or sets the security key.
        /// </summary>
        /// <value>The security key.</value>
        public string SecurityKey { get; set; }
        /// <summary>
        /// Gets or sets the issuer.
        /// </summary>
        /// <value>The issuer.</value>
        public string Issuer { get; set; }
        /// <summary>
        /// Gets or sets the audience.
        /// </summary>
        /// <value>The audience.</value>
        public string Audience { get; set; }

        /// <summary>
        /// 主题
        /// </summary>
        public string Subject { get; set; }

        /// <summary>
        /// 默认生命期
        /// </summary>
        public int LifeTimeSeconds { get; set; }
    }
}
