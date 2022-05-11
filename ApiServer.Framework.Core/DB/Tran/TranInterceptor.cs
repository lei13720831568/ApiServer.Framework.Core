
using Castle.DynamicProxy;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Linq;

namespace ApiServer.Framework.Core.DB.Tran
{
    public class TranInterceptor : IInterceptor
    {

        private readonly DbContext dbContext;

        public TranInterceptor(DbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public void Intercept(IInvocation invocation)
        {
            MethodInfo methodInfo = invocation.MethodInvocationTarget;
            if (methodInfo == null)
            {
                methodInfo = invocation.Method;
            }

            if (IsAsyncMethod(methodInfo)) {
                throw new Exception("事务不能是异步方法。");
            }

            TransactionAttribute transactionAttr =
                methodInfo.GetCustomAttributes<TransactionAttribute>(true).FirstOrDefault();
            if (transactionAttr != null) {
                if (dbContext.Database.CurrentTransaction == null)
                {

                    using (var transaction = dbContext.Database.BeginTransaction())
                    {
                        try
                        {
                            invocation.Proceed();
                            transaction.Commit();
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();
                            throw ex;
                        }
                    }
                }
                else {
                    invocation.Proceed();
                }
            } else {
                invocation.Proceed();
            }


        }


        /// <summary>
        /// 判断是否异步方法
        /// </summary>
        public static bool IsAsyncMethod(MethodInfo method)
        {
            return (
                method.ReturnType == typeof(Task) ||
                (method.ReturnType.IsGenericType && method.ReturnType.GetGenericTypeDefinition() == typeof(Task<>))
                );
        }

    }
}
