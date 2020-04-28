using ApiServer.Framework.Core.Vaild;
using IdentityServer4.Models;
using IdentityServer4.Validation;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace ApiServer.Framework.Core.AppIdentity.Grant.Validator
{
    public class RegGrantValidator : IExtensionGrantValidator
    {
        public string GrantType => ExtensionGrantTypes.RegGrantType;

        private readonly UserManager<AppUser> _userManager;
        private readonly IUserClaimsPrincipalFactory<AppUser> _claimsFactory;
        private readonly ICaptchaStore _captchaStore;
        private readonly IUserStore<AppUser> _userStore;

        public RegGrantValidator(UserManager<AppUser> userManager, IUserClaimsPrincipalFactory<AppUser> claimsFactory, ICaptchaStore captchaStore, IUserStore<AppUser> userStore)
        {
            _userManager = userManager;
            _claimsFactory = claimsFactory;
            _captchaStore = captchaStore;
            _userStore = userStore;
        }

        public async Task ValidateAsync(ExtensionGrantValidationContext context)
        {

            bool isTest = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development";

            var systemId = context.Request.Client.Claims.Where(c => c.Type == "system_id").FirstOrDefault();
            if (systemId == null) {
                context.Result = new GrantValidationResult(TokenRequestErrors.InvalidClient, "client未配置system_id");
                return;
            }

            var smsId = context.Request.Raw.Get("sms_id");
            var smsInputCode = context.Request.Raw.Get("sms_input_code");
            var captchaId = context.Request.Raw.Get("captcha_id");
            var captchaInputCode = context.Request.Raw.Get("captcha_input_code");
            var req_mobile = context.Request.Raw.Get("mobile");
            var name = context.Request.Raw.Get("name");

            if (string.IsNullOrEmpty(name))
            {
                context.Result = new GrantValidationResult(TokenRequestErrors.InvalidGrant, "姓名为空");
                return;
            }

            if (string.IsNullOrEmpty(captchaId) || string.IsNullOrEmpty(captchaInputCode))
            {
                context.Result = new GrantValidationResult(TokenRequestErrors.InvalidGrant, "图形验证码为空");
                return;
            }

            if (string.IsNullOrEmpty(smsId) || string.IsNullOrEmpty(smsInputCode))
            {
                context.Result = new GrantValidationResult(TokenRequestErrors.InvalidGrant,"短信码为空");
                return;
            }

            if (string.IsNullOrEmpty(req_mobile))
            {
                context.Result = new GrantValidationResult(TokenRequestErrors.InvalidGrant, "手机号码为空");
                return;
            }

            if (isTest==false)
            {
                (var captcha_result, _) = _captchaStore.Vaild(captchaId, "image", captchaInputCode);
                if (captcha_result == false)
                {
                    context.Result = new GrantValidationResult(TokenRequestErrors.InvalidGrant, "图形验证码错误");
                    return;
                }

                (var result, var mobile) = _captchaStore.Vaild(smsId, "sms", smsInputCode);
                if (result == false)
                {
                    context.Result = new GrantValidationResult(
                    TokenRequestErrors.InvalidGrant,
                    "短信码错误!"
                    );
                    return;
                }
                else
                {
                    await regUser(context, systemId, mobile,name);
                }
            }
            else {

                await regUser(context, systemId, req_mobile,name);
            }
            return;

        }

        private async Task regUser(ExtensionGrantValidationContext context, Claim systemId, string mobile,string name)
        {
            //IdentityResult createResult = await _userManager.CreateAsync(new AppUser
            //{
            //    Account = mobile,
            //    Mobile = mobile,
            //    Name = name,
            //    SystemId = int.Parse(systemId.Value),
            //    Status = 0
            //});

            IdentityResult createResult = await _userStore.CreateAsync(new AppUser
            {
                Account = mobile,
                Mobile = mobile,
                Name = name,
                SystemId = int.Parse(systemId.Value),
                Status = 0
            },new System.Threading.CancellationTokenSource().Token);

            if (createResult.Succeeded)
            {
                var dbuser = await _userManager.FindByNameAsync(mobile);
                var principal = await _claimsFactory.CreateAsync(dbuser);
                context.Result = new GrantValidationResult(
                                    dbuser.Id,
                                    ExtensionGrantTypes.RegGrantType, System.DateTime.Now, principal.Claims.ToList());
            }
            else
            {
                var err = createResult.Errors.FirstOrDefault();
                context.Result = new GrantValidationResult(
                                    TokenRequestErrors.InvalidRequest,
                                    "用户注册失败! " + err == null ? "" : err.Description
                                    ) ;
            }
        }
    }
}
