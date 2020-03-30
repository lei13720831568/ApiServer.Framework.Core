using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Text;

namespace ApiServer.Framework.Core.Web.Permission
{
    /// <summary>
    /// authcode authorization
    /// </summary>
    public class AuthCodeRequirement : IAuthorizationRequirement
    {
        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="permissionCodeList"></param>
        public AuthCodeRequirement(List<string> permissionCodeList)
        {
            PermissionCodeList = permissionCodeList;
        }

        /// <summary>
        /// 权限列表
        /// </summary>
        public List<string> PermissionCodeList { get; set; }
    }
}
