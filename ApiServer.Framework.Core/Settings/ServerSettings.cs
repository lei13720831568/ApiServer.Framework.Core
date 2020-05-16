using System;
using System.Collections.Generic;

namespace ApiServer.Framework.Core.Settings
{
    /// <summary>
    /// 服务器配置
    /// </summary>
    public class ServerSettings
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:ApiServer.Framework.Core.ServerSettings"/> class.
        /// </summary>
        public ServerSettings()
        {
        }

        /// <summary>
        /// 数据库链接串
        /// </summary>
        /// <value>The db connect strings.</value>
        public Dictionary<String,String> ConnectionStrings { get; set; }

        /// <summary>
        /// 进程id
        /// </summary>
        public long WorkerId { get; set; }

        /// <summary>
        /// 数据中心id
        /// </summary>
        public long DataCenterId { get; set; }
    }
}
