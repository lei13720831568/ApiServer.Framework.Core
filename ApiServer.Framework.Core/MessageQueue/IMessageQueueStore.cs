using System;
using System.Collections.Generic;

namespace ApiServer.Framework.Core.MessageQueue
{
    public interface IQueueStore
    {
        /// <summary>
        /// 保存消息
        /// </summary>
        /// <param name="msg"></param>
        public void Save(Message msg);

        /// <summary>
        /// 创建消息
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        public Message Create(Message msg);

        /// <summary>
        /// 删除消息
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        public Message Remove(Message msg);

        /// <summary>
        /// 获取未消费的消息
        /// </summary>
        /// <param name="count">最多获取数量</param>
        /// <returns></returns>
        public List<Message> GetMessage(int count = 1);
    }
}
