
using Castle.DynamicProxy;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using NLog;

namespace ApiServer.Framework.Core.DB.Tran
{


    public class TranInterceptor : IInterceptor
    {

        private readonly DbContext dbContext;

        private ILogger logger = LogManager.GetCurrentClassLogger();

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

            TransactionAttribute transactionAttr =
                methodInfo.GetCustomAttributes<TransactionAttribute>(true).FirstOrDefault();
            if (transactionAttr != null) {

                if (IsAsyncMethod(methodInfo))
                {
                    throw new Exception("事务处理不能是异步方法。");
                }
                logger.Debug("Transaction Intercept");
                if (dbContext.Database.CurrentTransaction == null)
                {

                    using (var transaction = dbContext.Database.BeginTransaction())
                    {
                        try
                        {
                            logger.Debug("begin Transaction");
                            invocation.Proceed();
                            transaction.Commit();
                            if (transactionAttr.AfterCommitEventHandle != null) {
                                transactionAttr.AfterCommitEventHandle(invocation.Arguments);
                            }
                            logger.Debug("commited Transaction");
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();
                            logger.Debug("rollback Transaction");
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
