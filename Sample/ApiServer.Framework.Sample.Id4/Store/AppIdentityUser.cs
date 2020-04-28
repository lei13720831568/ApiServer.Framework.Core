using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiServer.Framework.Sample.Id4.Store
{
    public class AppIdentityUser
    {
        /// <summary>
        /// id
        /// </summary>
        public string Id { get; set; }
        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 手机号码
        /// </summary>
        public string Mobile { get; set; }

        /// <summary>
        /// 称谓
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        public int Status { get; set; }

        public string Password { get; set; }

        public string Account { get; set; }
    }
}
