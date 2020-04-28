using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiServer.Framework.Sample.Id4.Controllers.Dto
{
    public class ImageCaptchaRsp
    {
        /// <summary>
        /// 验证码id
        /// </summary>
        public string CaptchaId { get; set; }

        /// <summary>
        /// 验证码图片
        /// </summary>
        public string CapchaBase64 { get; set; }
    }
}
