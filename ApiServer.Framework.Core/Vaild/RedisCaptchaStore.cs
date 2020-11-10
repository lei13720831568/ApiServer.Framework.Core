using System;
using System.Collections.Generic;
using System.Text;

namespace ApiServer.Framework.Core.Vaild
{
    public class RedisCaptchaStore : ICaptchaStore, IDependency
    {
        public void Remove(string id, string type)
        {
            var captcha = RedisHelper.Get<Captcha>(Captcha.GetKey(id, type));

            if (captcha != null)
            {
                var pipe = RedisHelper.StartPipe();
                pipe.Del(captcha.Id);
                if (!string.IsNullOrWhiteSpace(captcha.LimitKey)) pipe.Del(captcha.GetLimitKey());
                pipe.EndPipe();
            }
            return;
        }

        public (bool, long) Store(Captcha captcha, int timeLife, int limitKeyTimeLife = 0,int count=1)
        {
            if (!string.IsNullOrWhiteSpace(captcha.LimitKey))
            {
                int limitCount = RedisHelper.Get<int>(captcha.GetLimitKey());
                var ttl = RedisHelper.Ttl(captcha.GetLimitKey());

                if (limitCount >= count && ttl>0) {
                    return (false, ttl);
                }
                else {
                    var pipe = RedisHelper.StartPipe();
                    pipe.IncrBy(captcha.GetLimitKey());
                    pipe.Expire(captcha.GetLimitKey(), limitKeyTimeLife);

                    pipe.Set(captcha.GetKey(), captcha, timeLife);
                    pipe.EndPipe();
                    return (true, timeLife);
                }
            }
            else
            {
                RedisHelper.Set(captcha.GetKey(), captcha, timeLife);
            }
            return (true, 0);
        }

        public (bool,string) Vaild(string id, string type, string input)
        {
            var captcha = RedisHelper.Get<Captcha>(Captcha.GetKey(id, type));

            if (captcha != null)
            {
                var pipe = RedisHelper.StartPipe();
                pipe.Del(Captcha.GetKey(id, type));
                //验证成功删除约束
                if (!string.IsNullOrWhiteSpace(captcha.LimitKey) && captcha.Code == input) pipe.Del(captcha.GetLimitKey());
                pipe.EndPipe();
                return (captcha.Code == input,captcha.BizId);
            }
            else
            {
                return (false,null);
            }
        }
    }
}
