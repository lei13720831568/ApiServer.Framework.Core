using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Text;

namespace ApiServer.Framework.Core.Web.Authentication
{
    public interface IPermissionAuthorizeData
    {
        public string Permissions { get; set; }
    }
}
