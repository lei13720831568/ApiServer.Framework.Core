using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Text;

namespace ApiServer.Framework.Core.Web.Authentication
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
    public class PermissionAttribute : AuthorizeAttribute, IPermissionAuthorizeData
    {
        //public string Policy { get; set; }
        //public string Roles { get; set; }
        //public string AuthenticationSchemes { get; set; }

        private string _permissions = "Permission:";

        public string Permissions {
            get => _permissions;
            set {
                _permissions = value;
                var str = System.Text.Json.JsonSerializer.Serialize<IPermissionAuthorizeData>(this);
                Policy = $"{POLICY_PREFIX}{str}";
            }
        }

        const string POLICY_PREFIX = "Permission:";

        public PermissionAttribute(string permissions="")
        {
            Permissions = permissions;
            AuthenticationSchemes = HeaderAuthenticationDefaults.AuthenticationSchema;
        }
    }
}
