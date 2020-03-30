using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace ApiServer.Framework.Sample.Dto
{
    /// <summary>
    /// 密码登录
    /// </summary>
    public class LoginByPwdReq
    {
        /// <summary>
        /// 账号
        /// </summary>
        /// <value>The account.</value>
        [Required(ErrorMessage = "账号不能为空")]
        [StringLength(30, MinimumLength = 3, ErrorMessage = "账号需要3-30位之间")]
        public string Account { get; set; }

        /// <summary>
        /// 密码
        /// </summary>
        /// <value>The password.</value>
        [Required(ErrorMessage = "密码不能为空")]
        [StringLength(30, MinimumLength = 6, ErrorMessage = "密码需要6-30位之间")]
        public string Password { get; set; }

    }
}
