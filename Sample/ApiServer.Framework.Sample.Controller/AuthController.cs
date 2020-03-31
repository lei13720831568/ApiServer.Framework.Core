using ApiServer.Framework.Core.Mvc;
using ApiServer.Framework.Core.Settings;
using ApiServer.Framework.Core.Web;
using ApiServer.Framework.Core.Web.JWT;
using ApiServer.Framework.Core.Web.Permission;
using ApiServer.Framework.Core.Web.Vaild;
using ApiServer.Framework.Sample.Dto;
using ApiServer.Framework.Sample.Service;
using Microsoft.AspNetCore.Authorization;
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
    public class AuthController:ControllerBase
    {
        /// <summary>
        /// 用户server
        /// </summary>
        public UserService UserService { get; set; }

        /// <summary>
        /// jwt配置文件
        /// </summary>
        public JwtHelpers JwtHelper { get; set; }

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
            return ResponseFactory.CreateSuccessful<LoginInfo>(new LoginInfo { jwt = JwtHelper.GenerateToken(user.Account, user.Id) });
        }

        /// <summary>
        /// 测试访问
        /// </summary>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        public ActionResult<Response<string>> Test() {
            
            return ResponseFactory.CreateSuccessful<string>(HttpContext.User.Identity.Name);
        }
    }
}
