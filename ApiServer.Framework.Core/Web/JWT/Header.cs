using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace ApiServer.Framework.Core.Web.JWT
{
    public class Header
    {
        public string alg= "HS256";
        public string typ="JWT";
    }
}
