using ApiServer.Framework.Core.DB.Entity;
using ApiServer.Framework.Core.Exceptions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace ApiServer.Framework.Core.DB.Query
{
    public static class DbSetExtensions
    {
        /// <summary>
        /// 获取一个实体，并进行空检查
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="dbSet"></param>
        /// <param name="errorMsg">为空时的错误信息</param>
        /// <param name="predicate">查询条件</param>
        /// <param name="disableTracking">关闭实体追踪</param>
        /// <param name="fromCache">是否从缓存获取</param>
        /// <returns></returns>
        public static TEntity GetOneCheckNull<TEntity>(this DbSet<TEntity> dbSet,string errorMsg, Expression<Func<TEntity, bool>> predicate = null, Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> include = null, bool disableTracking = false,bool fromCache = false) where TEntity : EntityBase
        {
            IQueryable<TEntity> query = dbSet;

            if (disableTracking)
            {
                query = query.AsNoTracking();
            }
            if (include != null)
            {
                query = include(query);
            }

            if (predicate != null)
            {
                query = query.Where(predicate);
            }

            TEntity result = query.FirstOrDefault();
            if (result == null) {
                throw new BizException(errorMsg);
            }
            return result;
        }

        /// <summary>
        /// 获取一个实体
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="dbSet"></param>
        /// <param name="predicate">查询条件</param>
        /// <param name="disableTracking">关闭实体追踪</param>
        /// <param name="fromCache">是否从缓存获取</param>
        /// <returns></returns>
        public static TEntity GetOne<TEntity>(this DbSet<TEntity> dbSet, Expression<Func<TEntity, bool>> predicate = null, Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> include = null, bool disableTracking = false, bool fromCache = false) where TEntity : EntityBase
        {
            IQueryable<TEntity> query = dbSet;

            if (disableTracking)
            {
                query = query.AsNoTracking();
            }
            if (include != null)
            {
                query = include(query);
            }

            if (predicate != null)
            {
                query = query.Where(predicate);
            }
            TEntity result = query.FirstOrDefault();
            return result;
        }

        public static IQueryable<TEntity> BuildQuery<TEntity>(this DbSet<TEntity> dbSet,
            Expression<Func<TEntity, bool>> predicate = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> include = null,
            List<OrderByInfo> orderBys=null, bool disableTracking = true,
            bool ignoreQueryFilters = false) where TEntity : EntityBase
        {
            IQueryable<TEntity> query = dbSet;

            if (disableTracking)
            {
                query = query.AsNoTracking();
            }

            if (include != null)
            {
                query = include(query);
            }

            if (predicate != null)
            {
                query = query.Where(predicate);
            }

            if (ignoreQueryFilters)
            {
                query = query.IgnoreQueryFilters();
            }

            if (orderBy != null)
            {
                query = orderBy(query);
            }

            if (orderBys != null && orderBys.Count > 0) {
                foreach (var orderby in orderBys)
                {
                    //var isAscending = orderby.OrderType == 1;
                    //var pi = typeof(TEntity).GetProperty(orderby.Field);
                    //if (pi != null)
                    //    query = isAscending
                    //        ? query.OrderBy(a => pi.GetValue(a, null))
                    //        : query.OrderByDescending(a => pi.GetValue(a, null));

                    var pi = typeof(TEntity).GetProperty(orderby.Field);
                    if (pi == null) continue;
                    if (orderby.OrderType > 0)
                        query = query.OrderBy(orderby.Field, orderby.OrderType == 1);
                }
            }
            return query;
        }


        public static IQueryable<T> OrderBy<T>(this IQueryable<T> source, string columnName, bool isAscending = true)
        {
            if (String.IsNullOrEmpty(columnName))
            {
                return source;
            }

            ParameterExpression parameter = Expression.Parameter(source.ElementType, "");

            MemberExpression property = Expression.Property(parameter, columnName);
            LambdaExpression lambda = Expression.Lambda(property, parameter);

            string methodName = isAscending ? "OrderBy" : "OrderByDescending";

            Expression methodCallExpression = Expression.Call(typeof(Queryable), methodName,
                                  new Type[] { source.ElementType, property.Type },
                                  source.Expression, Expression.Quote(lambda));

            return source.Provider.CreateQuery<T>(methodCallExpression);
        }

        public static IPagedList<TEntity> GetPagedList<TEntity>(this DbSet<TEntity> dbSet,
            Expression<Func<TEntity, bool>> predicate = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> include = null,
            int pageIndex = 0,
            int pageSize = 20,
            bool disableTracking = true,
            bool ignoreQueryFilters = false) where TEntity : EntityBase
        {

            var query = dbSet.BuildQuery<TEntity>(predicate, orderBy, include, null, disableTracking, ignoreQueryFilters);
            return query.ToPagedList(pageIndex, pageSize);
        }




        public static IPagedList<TResult> GetPagedList<TResult, TEntity>(this DbSet<TEntity> dbSet, Expression<Func<TEntity, TResult>> selector,
                                                         Expression<Func<TEntity, bool>> predicate = null,
                                                         Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
                                                         Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> include = null,
                                                         int pageIndex = 0,
                                                         int pageSize = 20,
                                                         bool disableTracking = true,
                                                         bool ignoreQueryFilters = false) where TEntity : EntityBase
        {

            var query = dbSet.BuildQuery<TEntity>(predicate, orderBy, include, null, disableTracking, ignoreQueryFilters);
            return query.Select(selector).ToPagedList(pageIndex, pageSize);
        }

        public static  IPagedList<T> ToPagedList<T>(this IQueryable<T> source, int pageIndex, int pageSize, int indexFrom = 0)
        {
            if (indexFrom > pageIndex)
            {
                throw new ArgumentException($"indexFrom: {indexFrom} > pageIndex: {pageIndex}, must indexFrom <= pageIndex");
            }

            var count = source.Count();
            var items = source.Skip((pageIndex - indexFrom) * pageSize)
                                    .Take(pageSize).ToList();

            var pagedList = new PagedList<T>()
            {
                PageIndex = pageIndex,
                PageSize = pageSize,
                IndexFrom = indexFrom,
                TotalCount = count,
                Items = items,
                TotalPages = (int)Math.Ceiling(count / (double)pageSize)
            };

            return pagedList;
        }

        public static TSource FirstOrThrow<TSource>(this IQueryable<TSource> source, Expression<Func<TSource, bool>> predicate,string throwMsg)
        {
            var obj = source.FirstOrDefault(predicate);
            Assert.NotNull(obj, throwMsg);
            return obj;
        }

        public static TEntity FirstOrThrow<TEntity, TProperty>(this IIncludableQueryable<TEntity, TProperty> source, Expression<Func<TEntity, bool>> predicate, string throwMsg)
        {
            return (source as IQueryable<TEntity>).FirstOrThrow(predicate, throwMsg);
        }
    }
}
