using ApiServer.Framework.Core.Exceptions;
using System;
using System.Collections.Generic;
using System.Text;

namespace ApiServer.Framework.Core.Vaild
{
    public class Captcha
    {
        public string Id;
        public string Code;
        public string Type;
        public string LimitKey;

        public string BizId;

        public string GetKey() {
            return $"captcha_{Type}_{Id}";
        }

        public string GetLimitKey() {
            if (string.IsNullOrWhiteSpace(LimitKey)) {
                throw new BizException("没有设置captcha limitkey");
            }
            return $"captcha_limit_{Type}_{LimitKey}";
        }

        public static string GetKey(string id, string type) {
            return $"captcha_{type}_{id}";
        }
    }
}
