using ApiServer.Framework.Core.DB.Query;
using System;
using System.Collections.Generic;
using System.Text;

namespace ApiServer.Framework.Core.Convert
{
    public interface IConverter<S,T>
    {
        T Convert(S obj);
        List<T> Convert(List<S> list);

        IPagedList<T> Convert(IPagedList<S> pagedList);
    }
}
