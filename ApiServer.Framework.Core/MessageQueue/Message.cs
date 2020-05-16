using System;
namespace ApiServer.Framework.Core.MessageQueue
{
    public class Message
    {
        public Message(string msgBody)
        {
            MsgBody = msgBody;
        }

        /// <summary>
        /// 消息id,一个队列内唯一
        /// </summary>
        public string MsgId { get; set; }

        /// <summary>
        /// 消息内容
        /// </summary>
        public string MsgBody { get; set; }

        /// <summary>
        /// 消息操作句柄,队列组件内唯一
        /// </summary>
        public string MsgHandle { get; set; }
    }
}
