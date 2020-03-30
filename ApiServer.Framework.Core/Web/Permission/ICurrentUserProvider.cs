using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace ApiServer.Framework.Core.Web.Permission
{
    /// <summary>
    /// 用户凭证供应者
    /// </summary>
    public interface ICurrentUserProvider
    {
        /// <summary>
        /// 获取当前用户id
        /// </summary>
        /// <returns>null表示未获取到当前用户id</returns>
        dynamic GetId();
    }
}
