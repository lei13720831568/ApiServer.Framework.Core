using ApiServer.Framework.Core.Mvc;
using ApiServer.Framework.Core.Settings;
using ApiServer.Framework.Core.Web;
using ApiServer.Framework.Core.Web.Permission;
using ApiServer.Framework.Core.Web.Vaild;
using ApiServer.Framework.Sample.Dto;
using ApiServer.Framework.Sample.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Text;

namespace ApiServer.Framework.Sample.Controller
{
    /// <summary>
    /// 授权controller
    /// </summary>
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class AuthController
    {
        /// <summary>
        /// 用户server
        /// </summary>
        public UserService UserService { get; set; }

        /// <summary>
        /// jwt配置文件
        /// </summary>
        public IOptions<JWTSettings> JWTSetting { get; set; }

        /// <summary>
        /// 密码登录
        /// </summary>
        /// <returns>The by pwd.</returns>
        /// <param name="req">req.</param>
        [HttpPost]
        [ValidDto]
        public ActionResult<Response<LoginInfo>> LoginByPwd(LoginByPwdReq req)
        {
            var user = UserService.LoginByPwd(req);
            //创建登录凭据
            var token = Token.CreateToken(JWTSetting.Value,user.Id,System.Guid.NewGuid().ToString("N"));
            
            return ResponseFactory.CreateSuccessful<LoginInfo>(new LoginInfo { jwt=token.ToString() });
        }
    }
}
