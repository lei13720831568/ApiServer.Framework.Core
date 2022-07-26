using System;
using System.Collections.Generic;
using System.Text;

namespace ApiServer.Framework.Core.DB.Tran
{
    public class TransactionAttribute: Attribute
    {
        public TransactionAttribute()
        {
        }

        public TransactionAttribute(Action<object[]> afterCommitEventHandler)
        {
            this.AfterCommitEventHandle = afterCommitEventHandler;
        }

        public  Action<object[]> AfterCommitEventHandle = null;
    }
}
