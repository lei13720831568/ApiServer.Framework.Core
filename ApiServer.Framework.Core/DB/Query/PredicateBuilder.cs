using ApiServer.Framework.Core.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace ApiServer.Framework.Core.DB.Query
{
    /// <summary>
    /// Predicate builder.
    /// </summary>
    public static class PredicateBuilder
    {
        /// <summary>
        /// True this instance.
        /// </summary>
        /// <returns>The true.</returns>
        /// <typeparam name="T">The 1st type parameter.</typeparam>
        public static Expression<Func<T, bool>> True<T>() { return f => true; }

        /// <summary>
        /// False this instance.
        /// </summary>
        /// <returns>The false.</returns>
        /// <typeparam name="T">The 1st type parameter.</typeparam>
        public static Expression<Func<T, bool>> False<T>() { return f => false; }

        /// <summary>
        /// Or the specified expr1 and expr2.
        /// </summary>
        /// <returns>The or.</returns>
        /// <param name="expr1">Expr1.</param>
        /// <param name="expr2">Expr2.</param>
        /// <typeparam name="T">The 1st type parameter.</typeparam>
        public static Expression<Func<T, bool>> Or<T>(this Expression<Func<T, bool>> expr1,
                                                            Expression<Func<T, bool>> expr2)
        {
            var invokedExpr = Expression.Invoke(expr2, expr1.Parameters.Cast<Expression>());
            return Expression.Lambda<Func<T, bool>>
                  (Expression.OrElse(expr1.Body, invokedExpr), expr1.Parameters);
        }

        /// <summary>
        /// And the specified expr1 and expr2.
        /// </summary>
        /// <returns>The and.</returns>
        /// <param name="expr1">Expr1.</param>
        /// <param name="expr2">Expr2.</param>
        /// <typeparam name="T">The 1st type parameter.</typeparam>
        public static Expression<Func<T, bool>> And<T>(this Expression<Func<T, bool>> expr1,
                                                             Expression<Func<T, bool>> expr2)
        {
            var invokedExpr = Expression.Invoke(expr2, expr1.Parameters.Cast<Expression>());
            return Expression.Lambda<Func<T, bool>>
                  (Expression.AndAlso(expr1.Body, invokedExpr), expr1.Parameters);
        }


        /// <summary>
        /// 构建条件表达式
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="S"></typeparam>
        /// <param name="queryObj"></param>
        /// <returns></returns>
        public static Expression<Func<T, bool>> BuildFromCondition<T, S>(S queryObj)
        {
            var predicate = True<T>();
            Type destObjType = typeof(T);
            Type objType = typeof(S);
            Type conType = typeof(QueryOptAttribute);

            foreach (var prop in objType.GetProperties())
            {
                var optAttr = prop.GetCustomAttributes(conType, true).FirstOrDefault();
                //查找条件特性,需要是Nullable
                if (optAttr != null)
                {
                    var optAttrInstance = optAttr as QueryOptAttribute;
                    PropertyInfo destProp;
                    var fieldName = optAttrInstance.GetFieldName();
                    if (string.IsNullOrEmpty(fieldName))
                    {
                        destProp = destObjType.GetProperty(prop.Name);
                    }
                    else {
                        destProp = destObjType.GetProperty(fieldName);
                    }

                    if (destProp == null) {
                        throw new Exception($"QueryOpt 无法匹配到目标字段 {prop.Name} {fieldName}");
                    };

                   // var destProp = destObjType.GetProperty(prop.Name);
                   // var optAttrInstance = optAttr as QueryOptAttribute;
                    var p = optAttrInstance.CheckAndBuildExpression<T>(destObjType, destProp, prop, queryObj);
                    if (p != null)
                    {
                        predicate = And<T>(predicate, p);
                    }

                }
            }

            return predicate;
        }

    }
}
