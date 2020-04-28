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

        public (bool, long) Store(Captcha captcha, int timeLife, int limitKeyTimeLife = 0)
        {
            if (!string.IsNullOrWhiteSpace(captcha.LimitKey))
            {

                var ttl = RedisHelper.Ttl(captcha.GetLimitKey());
                if (ttl > 0)
                {
                    return (false, ttl);
                }
                else {
                    var pipe = RedisHelper.StartPipe();
                    pipe.Set(captcha.GetLimitKey(), captcha.Id, limitKeyTimeLife);
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
                if (!string.IsNullOrWhiteSpace(captcha.LimitKey)) pipe.Del(captcha.GetLimitKey());
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
