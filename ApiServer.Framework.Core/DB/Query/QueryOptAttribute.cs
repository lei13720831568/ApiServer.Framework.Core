using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace ApiServer.Framework.Core.DB.Query
{
    /// <summary>
    /// linq查询特性
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class QueryOptAttribute : Attribute
    {
        private QueryOptEnum Operation { get; set; }

        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="opt"></param>
        public QueryOptAttribute(QueryOptEnum opt = QueryOptEnum.Equal)
        {
            Operation = opt;
        }

        private static readonly MethodInfo containsMethod = typeof(string).GetMethod("Contains", new Type[] { typeof(string) });
        private static readonly MethodInfo startsWithMethod =
                                typeof(string).GetMethod("StartsWith", new Type[] { typeof(string) });
        private static readonly MethodInfo endsWithMethod =
                                typeof(string).GetMethod("EndsWith", new Type[] { typeof(string) });

        /// <summary>
        ///  检查字段是否为nullable 以及是否为空
        /// </summary>
        /// <returns><c>true</c>, if is null was checked, <c>false</c> otherwise.</returns>
        /// <param name="prop">Property.</param>
        /// <param name="obj">Object.</param>
        private bool CheckIsNull(PropertyInfo prop, Object obj)
        {
            var propValue = prop.GetValue(obj);
            if (prop.PropertyType.IsPrimitive || prop.PropertyType.IsEnum)
            {
                throw new Exception("动态查询特性(QueryOpt)不能应用在简单类型/枚举上,需要使用Nullable替代");
            }

            var propBaseType = GetBaseType(prop);
            if (propBaseType == typeof(string))
            {
                return !string.IsNullOrEmpty(propValue as string);
            }

            return !(propValue is null);
        }

        /// <summary>
        /// 获取字段基础类型,如果是Nullable则返回基础类型
        /// </summary>
        /// <returns>The base type.</returns>
        /// <param name="prop">Property.</param>
        private Type GetBaseType(PropertyInfo prop)
        {
            if (prop.PropertyType.IsGenericType && prop.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                return prop.PropertyType.GetGenericArguments()[0];
            }

            return prop.PropertyType;
        }

        /// <summary>
        /// 构建表达式树
        /// </summary>
        /// <returns>The lambda.</returns>
        /// <param name="destObjType">Destination object type.</param>
        /// <param name="destProp">Destination property.</param>
        /// <param name="searchObj">Search object.</param>
        /// <param name="searchProp">Search property.</param>
        /// <param name="func">Func.</param>
        /// <typeparam name="T">The 1st type parameter.</typeparam>
        private Expression<Func<T, bool>> BuildLambda<T>(Type destObjType, PropertyInfo destProp, Object searchObj, PropertyInfo searchProp, Func<Expression, Expression, Expression> func)
        {
            ParameterExpression pe = Expression.Parameter(destObjType, "obj");
            var left = Expression.Property(pe, destProp);
            var right = Expression.Constant(searchProp.GetValue(searchObj));
            return Expression.Lambda<Func<T, bool>>(func(left, right), pe);
        }

        /// <summary>
        /// 构建表达式内的动作
        /// </summary>
        /// <returns>The expression.</returns>
        /// <param name="left">Left.</param>
        /// <param name="right">Right.</param>
        public Expression GetExpression(Expression left, Expression right)
        {
            Expression result = null;
            switch (this.Operation)
            {
                case QueryOptEnum.LessThen:
                    result = Expression.LessThan(left, right);
                    break;
                case QueryOptEnum.LessThanOrEqual:
                    result = Expression.LessThanOrEqual(left, right);
                    break;
                case QueryOptEnum.GreaterThan:
                    result = Expression.GreaterThanOrEqual(left, right);
                    break;
                case QueryOptEnum.Contains:
                    result = Expression.Call(left, containsMethod, right);
                    break;
                case QueryOptEnum.StartWith:
                    result = Expression.Call(left, startsWithMethod, right);
                    break;
                case QueryOptEnum.EndWith:
                    result = Expression.Call(left, endsWithMethod, right);
                    break;
                default:
                    result = Expression.Equal(left, right);
                    break;
            }
            return result;
        }

        /// <summary>
        /// 检查字段并构建
        /// </summary>
        /// <returns>The and build expression.</returns>
        /// <param name="destObjType">Destination object type.</param>
        /// <param name="destProp">Destination property.</param>
        /// <param name="searchProp">Search property.</param>
        /// <param name="searchObj">Search object.</param>
        /// <typeparam name="T">The 1st type parameter.</typeparam>
        public Expression<Func<T, bool>> CheckAndBuildExpression<T>(Type destObjType, PropertyInfo destProp, PropertyInfo searchProp, Object searchObj)
        {

            var searchPropType = GetBaseType(searchProp);
            if (this.Operation == QueryOptEnum.Contains
                || this.Operation == QueryOptEnum.StartWith
                || this.Operation == QueryOptEnum.EndWith
                )
            {
                if (searchPropType != typeof(string))
                {
                    throw new Exception($"{this.Operation.ToString()}只能应用在string类型上,当前类型为{searchPropType.FullName}");
                }
            }

            //目标存在属性且基类型相等且不为空
            if (destProp != null
                && destProp.PropertyType.Equals(GetBaseType(searchProp))
                && CheckIsNull(searchProp, searchObj)
                )
            {
                return BuildLambda<T>(destObjType, destProp, searchObj, searchProp, GetExpression);
            }
            return null;

        }
    }

    /// <summary>
    /// Search opt enum.
    /// </summary>
    public enum QueryOptEnum
    {
        /// <summary>
        /// 等于
        /// </summary>
        Equal,
        /// <summary>
        /// 包含
        /// </summary>
        Contains,
        /// <summary>
        /// 前缀
        /// </summary>
        StartWith,
        /// <summary>
        /// 结尾
        /// </summary>
        EndWith,
        /// <summary>
        /// 小于
        /// </summary>
        LessThen,
        /// <summary>
        /// 大于
        /// </summary>
        GreaterThan,
        /// <summary>
        /// 小于等于
        /// </summary>
        LessThanOrEqual,
        /// <summary>
        /// 大于等于
        /// </summary>
        GreaterThenOrEqual
    }
}
