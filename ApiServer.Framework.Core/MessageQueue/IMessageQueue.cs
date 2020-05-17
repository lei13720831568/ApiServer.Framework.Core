using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ApiServer.Framework.Core.MessageQueue
{
    /// <summary>
    /// 消息队列接口
    /// </summary>
    public interface IMessageQueue
    {
        /// <summary>
        /// 发送消息到队列
        /// </summary>
        /// <param name="queueName"></param>
        /// <param name="msg"></param>
        public void Send(string queueName,Message msg);

        /// <summary>
        /// 从队列获取消息,如果没有消息则等待直到N秒
        /// </summary>
        /// <param name="queueName">队列名称</param>
        /// <param name="waitSeconds">长轮询时间30秒</param>
        /// <returns></returns>
        public  Task<List<Message>> Receive(string queueName, int count = 5, int waitSeconds=30);

        /// <summary>
        /// 确认消息已被消费,确认后消息将被删除
        /// </summary>
        /// <param name="msgHandle"></param>\
        /// <param name="result">true确认 false回退</param>
        public void Ack(string msgHandle,bool result);


    }
}
