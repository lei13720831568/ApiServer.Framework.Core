using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ApiServer.Framework.Sample.Id4.Controllers.Dto
{

    public class SmsCaptchaReq
    {
        [Required(ErrorMessage = "手机号码不能为空")]
        [StringLength(11, MinimumLength = 11 ,ErrorMessage = "手机号码需要11位")]
        public string Mobile { get; set; }

        [Required(ErrorMessage = "type需要填写")]
        [Range(1f,1f)]
        public int Type { get; set; }
    }
}
