using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace ApiServer.Framework.Core.DB
{
    public class TranHelper
    {
        public static T TranFor<T>(Func<T> f,DbContext context) {
            using (var transaction = context.Database.BeginTransaction())
            {
                try
                {
                    var result = f();
                    transaction.Commit();
                    return result;
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw ex;
                }
            }
        }
    }
}
