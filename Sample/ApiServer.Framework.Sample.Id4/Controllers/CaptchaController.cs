using ApiServer.Framework.Core;
using ApiServer.Framework.Core.Exceptions;
using ApiServer.Framework.Core.Vaild;
using ApiServer.Framework.Core.Web;
using ApiServer.Framework.Core.Web.Vaild;
using ApiServer.Framework.Sample.Id4.Controllers.Dto;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiServer.Framework.Sample.Id4.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class CaptchaController: ControllerBase
    {

        public CaptchaFactory _captchaFactory { get; set; }
        public SMSCaptchaFactory _smsCaptchaFactory { get; set; }

        /// <summary>
        /// 图形验证码
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult<Response<ImageCaptchaRsp>> Image(int? width,int? height) {
            width = !width.HasValue ? 85: width;
            height = !height.HasValue ? 40: height;
            (var cap, var img) = _captchaFactory.Create(4, width.Value, height.Value);
            return ResponseFactory.CreateSuccessful<ImageCaptchaRsp>(new ImageCaptchaRsp
            {
                CaptchaId = cap.Id,
                CapchaBase64 = $"data:image/png;base64,{img.Base64()}"
            });
        }

        /// <summary>
        /// 短信验证码
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidDto]
        public ActionResult<Response<SmsCaptchaRsp>> Sms(SmsCaptchaReq req) {

            var cap = _smsCaptchaFactory.Create(req.Mobile, "");
            if (cap == null)
                throw new BizException("获取可能太频繁，请稍后再试");

            return ResponseFactory.CreateSuccessful<SmsCaptchaRsp>(new SmsCaptchaRsp
            {
                SmsId = cap.Id
            }); ;
        }

    }
}
