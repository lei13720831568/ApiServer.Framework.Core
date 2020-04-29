using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Text;

namespace ApiServer.Framework.Core.Web.Authentication
{
    public interface IPermissionAuthorizeData: IAuthorizeData
    {
        public string Permissions { get; set; }
    }
}
