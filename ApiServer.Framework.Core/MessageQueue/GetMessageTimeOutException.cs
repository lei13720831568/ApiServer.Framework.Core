using System;
namespace ApiServer.Framework.Core.MessageQueue
{
    public class GetMessageTimeOutException : Exception
    {
        public GetMessageTimeOutException(string message) : base(message)
        {
        }
    }
}
