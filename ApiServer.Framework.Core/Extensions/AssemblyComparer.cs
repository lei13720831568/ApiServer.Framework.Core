using System;
using System.Collections.Generic;
using System.Reflection;
using System.Diagnostics.CodeAnalysis;

namespace ApiServer.Framework.Core.Extensions
{
    public class AssemblyComparer : IEqualityComparer<Assembly>
    {
        public bool Equals([AllowNull] Assembly x, [AllowNull] Assembly y)
        {

            if (x.FullName == y.FullName)
            {
                return true;
            }
            else
            {
                return false;
            }

        }

        public int GetHashCode([DisallowNull] Assembly obj)
        {
            if (obj == null) return 0;
            return obj.ToString().GetHashCode();
        }
    }
}
