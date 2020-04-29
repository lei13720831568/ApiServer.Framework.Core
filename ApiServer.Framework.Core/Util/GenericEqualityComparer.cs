using System;
using System.Collections.Generic;
using System.Text;

namespace ApiServer.Framework.Core.Util
{
    public class GenericEqualityComparer<T> : IEqualityComparer<T>
    {
        private Func<T,object> _virtualFieldComparator;
        private Func<T,T,bool> _virtualFilterComparator;
        public GenericEqualityComparer(Func<T, object> virtualFieldComparator)
        {
            if (virtualFieldComparator == null) throw new ArgumentNullException(nameof(virtualFieldComparator), $"{nameof(virtualFieldComparator)}  doesn't be null");
            Reset();
            this._virtualFieldComparator = virtualFieldComparator;
        }
        public GenericEqualityComparer(Func<T, T, bool> virtualFilterComparator)
        {
            if (virtualFilterComparator == null) throw new ArgumentNullException(nameof(virtualFilterComparator), $"{nameof(virtualFilterComparator)}  doesn't be null");
            Reset();
            this._virtualFilterComparator = virtualFilterComparator;
        }
        private void Reset()
        {
            _virtualFieldComparator = null;
            _virtualFilterComparator = null;
        }
        public bool Equals(T x, T y)
        {
            bool result = false;
            if (_virtualFieldComparator != null) result = _virtualFieldComparator(x).Equals(_virtualFieldComparator(y));
            else result = _virtualFilterComparator(x, y);
            return result;
        }
        public int GetHashCode(T obj)
        {
            int result = 0;
            if (_virtualFieldComparator != null) result = _virtualFieldComparator(obj).GetHashCode();
            else result = _virtualFilterComparator(obj, obj).GetHashCode();
            return result;
        }
    }
}
