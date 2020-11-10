using System;
using System.Collections.Generic;
using System.Text;

namespace ApiServer.Framework.Core.Vaild
{
    /// <summary>
    /// 验证码存储接口
    /// </summary>
    public interface ICaptchaStore
    {
        /// <summary>
        /// 存储
        /// </summary>
        /// <param name="id">验证码id</param>
        /// <param name="code">验证码值</param>
        /// <param name="timeLife">验证码生命周期</param>
        /// <param name="limitKey">验证码约束key</param>
        /// <param name="limitTimeLife">验证码约束key存活周期</param>
        /// <param name="count">存活数量</param>
        /// <returns>结果</returns>
        (bool, long) Store(Captcha captcha, int timeLife, int limitKeyTimeLife= 0,int count=1);

        /// <summary>
        /// 验证
        /// </summary>
        /// <param name="id"></param>
        /// <param name="type"></param>
        /// <param name="input"></param>
        /// <returns>验证结果，业务id</returns>
        (bool,string) Vaild(string id, string type, string input);

        void Remove(string id, string type);
    }
}
