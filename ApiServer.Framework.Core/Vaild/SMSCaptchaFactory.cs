using System;
using System.Collections.Generic;
using System.Text;

namespace ApiServer.Framework.Core.Vaild
{
    public class SMSCaptchaFactory : IDependency
    {
        protected List<char> _characters;
        public SMSCaptchaFactory()
        {
            //排除的内容
            var exts = new List<char> { '0', '1' };

            _characters = new List<char>();
            for (var c = '0'; c <= '9'; c++)
            {
                if (exts.Contains(c))
                {
                    continue;
                }
                _characters.Add(c);
            }
        }

        public ICaptchaStore CaptchaStore { get; set; }

        public Captcha Create(string mobile,string bizId,int charCount = 4)
        {
            var chars = new char[charCount];
            var len = _characters.Count;
            var random = new Random();
            for (var i = 0; i < chars.Length; i++)
            {
                var r = random.Next(len);
                chars[i] = _characters[r];
            }
            var captcha = new Captcha
            {
                Id = System.Guid.NewGuid().ToString("N"),
                Code = new string(chars),
                LimitKey = mobile,
                Type = "sms",
                BizId = bizId
            };
            (var result,_) =CaptchaStore.Store(captcha, 30 * 60, 90);
            if (result == false) return null;

            return captcha;
        }

        /// <summary>
        /// 验证
        /// </summary>
        /// <param name="id"></param>
        /// <param name="input"></param>
        /// <returns>验证结果，业务id</returns>
        public (bool,string) VaildCaptcha(string id, string input)
        {
            return CaptchaStore.Vaild(id, "sms", input);
        }
    }
}
