using Microsoft.AspNetCore.Authentication;
using System;
using System.Collections.Generic;
using System.Text;

namespace ApiServer.Framework.Core.Web.Authentication
{
    public class HeaderAuthenticationOptions : AuthenticationSchemeOptions
    {
        private string _accessTokenHeader = "Authorization";

        public string AccessTokenHeader { 
            get => _accessTokenHeader; 
            set{
                if (!string.IsNullOrWhiteSpace(value))
                {
                    _accessTokenHeader = value;
                }
            }
        }
    }
}
