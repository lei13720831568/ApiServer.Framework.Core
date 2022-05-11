using ApiServer.Framework.Core.Exceptions;
using System;
using System.Collections.Generic;
using System.Text;

namespace ApiServer.Framework.Core
{
    public class Assert
    {
        /// <summary>
        /// 断言 predicate 为true 否则抛异常
        /// </summary>
        /// <param name="predicate"></param>
        /// <param name="errorMsg"></param>
        public static void True(Func<bool> predicate, string errorMsg) {
            if (!predicate()) {
                throw new BizException(errorMsg);
            }
        }

        /// <summary>
        /// 断言 predicate false 否则抛异常
        /// </summary>
        /// <param name="predicate"></param>
        /// <param name="errorMsg"></param>
        public static void False(Func<bool> predicate, string errorMsg)
        {
            if (predicate())
            {
                throw new BizException(errorMsg);
            }
        }

        /// <summary>
        /// 为空断言
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="errorMsg"></param>
        public static void Null(Object obj, string errorMsg)
        {
            True(() => obj == null, errorMsg);
        }

        /// <summary>
        /// 不为空断言
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="errorMsg"></param>
        public static void NotNull(Object obj, string errorMsg) {
            True(() => obj != null, errorMsg);
        }

        /// <summary>
        /// 非空字符串断言
        /// </summary>
        /// <param name="str"></param>
        /// <param name="errorMsg"></param>
        public static void NotEmpty(String str, string errorMsg) {
            True(() => !String.IsNullOrEmpty(str), errorMsg);
        }

        /// <summary>
        /// 空字符串断言
        /// </summary>
        /// <param name="str"></param>
        /// <param name="errorMsg"></param>
        public static void Empty(String str, string errorMsg)
        {
            True(() => String.IsNullOrEmpty(str), errorMsg);
        }
    }
}
