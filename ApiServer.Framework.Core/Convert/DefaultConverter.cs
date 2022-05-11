using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using AgileObjects.AgileMapper;
using ApiServer.Framework.Core.DB.Query;

namespace ApiServer.Framework.Core.Convert
{
    public abstract class DefaultConverter<S, T> : IConverter<S, T>
    {
        public virtual T Convert(S obj)
        {
            return Mapper.Map(obj).ToANew<T>();
        }

        public virtual List<T> Convert(List<S> list)
        {
            return list.Select(obj => Convert(obj)).ToList();
        }

        public virtual IPagedList<T> Convert(IPagedList<S> pagedList)
        {
            var newPagedList =  new PagedList<T>();
            newPagedList.PageSize = pagedList.PageSize;
            newPagedList.PageIndex = pagedList.PageSize;
            newPagedList.TotalCount = pagedList.TotalCount;
            newPagedList.TotalPages = pagedList.TotalPages;
            newPagedList.IndexFrom = pagedList.IndexFrom;
            newPagedList.Items = Convert(pagedList.Items.ToList());
            return newPagedList;
        }
    }
}
