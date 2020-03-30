using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using NLog;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;

namespace ApiServer.Framework.Core.Web.Permission
{
    /// <summary>
    /// 资源权限验证
    /// </summary>
    public class AuthCodeHandler : AuthorizationHandler<AuthCodeRequirement>
    {
        private readonly ILogger _logger;

        private readonly ICurrentUserProvider _currentUserProvider;
        private readonly IUserResourceProvider _userResourceProvider;

        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="currentUserProvider"></param>
        /// <param name="userResourceProvider"></param>
        /// <param name="logger"></param>
        public AuthCodeHandler(ICurrentUserProvider currentUserProvider,IUserResourceProvider userResourceProvider, NLog.ILogger logger)
        {
            this._currentUserProvider = currentUserProvider;
            this._userResourceProvider = userResourceProvider;
            this._logger = logger;
        }

        /// <summary>
        /// 资源权限验证
        /// </summary>
        /// <param name="context"></param>
        /// <param name="requirement"></param>
        /// <returns></returns>
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, AuthCodeRequirement requirement)
        {
            try
            {
                var userId = this._currentUserProvider.GetId();
                if (userId != null)
                {
                    //查询用户资源列表
                    var userResources = this._userResourceProvider.GetUserResources(userId);
                    //找出不满足条件的权限
                    var noAuth = requirement.PermissionCodeList
                        .Select(p => p)
                        .Where(p => !userResources.Contains(p));

                    if (noAuth.Any())
                    {
                        var msg = $"当前用户未授权访问编号为{String.Join(",", noAuth)}的资源.";
                        _logger.Info(msg);

                        context.Fail();
                    }
                    else
                    {
                        context.Succeed(requirement);
                    }
                }
                else {
                    _logger.Info("找不到用户凭证");
                    context.Fail();
                }
               
            }
            catch (Exception ex) {
                _logger.Info($"权限验证异常:{ex.Message}");
                context.Fail();
            }

            return Task.CompletedTask;
        }
    }
}
