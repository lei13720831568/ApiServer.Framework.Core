using System;
using System.Collections.Generic;
using System.Text;

namespace ApiServer.Framework.Core.Web.Permission
{
    /// <summary>
    /// 验证用户资源接口
    /// </summary>
    public interface IUserResourceProvider
    {
        /// <summary>
        /// 获取指定用户有权限的资源列表
        /// </summary>
        /// <param name="id">user id</param>
        /// <returns></returns>
        List<string> GetUserResources(dynamic id);
    }
}
