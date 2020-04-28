using ApiServer.Framework.Core.Vaild;
using IdentityServer4.Models;
using IdentityServer4.Validation;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ApiServer.Framework.Core.AppIdentity.Grant.Validator
{
    public class SMSGrantValidator: IExtensionGrantValidator
    {
        public string GrantType => ExtensionGrantTypes.SMSGrantType;

        private readonly UserManager<AppUser> _userManager;
        private readonly IUserClaimsPrincipalFactory<AppUser> _claimsFactory;
        private readonly ICaptchaStore _captchaStore;

        public SMSGrantValidator(UserManager<AppUser> userManager, IUserClaimsPrincipalFactory<AppUser> claimsFactory, ICaptchaStore captchaStore) {
            _userManager = userManager;
            _claimsFactory = claimsFactory;
            _captchaStore = captchaStore;
        }

        public async Task ValidateAsync(ExtensionGrantValidationContext context)
        {
            var smsId = context.Request.Raw.Get("sms_id");
            var smsInputCode = context.Request.Raw.Get("sms_input_code");
            var captchaId = context.Request.Raw.Get("captcha_id");
            var captchaInputCode = context.Request.Raw.Get("captcha_input_code");
            var req_mobile = context.Request.Raw.Get("mobile");

            bool isTest = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development";


            if (string.IsNullOrEmpty(req_mobile) )
            {
                context.Result = new GrantValidationResult(TokenRequestErrors.InvalidGrant, "手机号码为空");
                return;
            }


            if (string.IsNullOrEmpty(captchaId) || string.IsNullOrEmpty(captchaInputCode))
            {
                context.Result = new GrantValidationResult(TokenRequestErrors.InvalidGrant, "图形验证码为空");
                return;
            }

            if (isTest == false)
            {
                (var captcha_result, _) = _captchaStore.Vaild(captchaId, "image", captchaInputCode);
                if (captcha_result == false)
                {
                    context.Result = new GrantValidationResult(TokenRequestErrors.InvalidGrant, "图形验证码错误");
                    return;
                }

                if (string.IsNullOrEmpty(smsId) || string.IsNullOrEmpty(smsInputCode))
                {
                    context.Result = new GrantValidationResult(TokenRequestErrors.InvalidGrant);
                    return;
                }

                (var result, var mobile) = _captchaStore.Vaild(smsId, "sms", smsInputCode);
                if (result)
                {
                    var user = await _userManager.FindByNameAsync(mobile);
                    var principal = await _claimsFactory.CreateAsync(user);
                    context.Result = new GrantValidationResult(
                                        user.Id,
                                        ExtensionGrantTypes.SMSGrantType, System.DateTime.Now, principal.Claims.ToList());
                }
                else
                {
                    context.Result = new GrantValidationResult(
                        TokenRequestErrors.InvalidGrant,
                        "短信码错误!"
                        );
                }
            }
            else {
                var user = await _userManager.FindByNameAsync(req_mobile);
                var principal = await _claimsFactory.CreateAsync(user);
                context.Result = new GrantValidationResult(
                                    user.Id,
                                    ExtensionGrantTypes.SMSGrantType, System.DateTime.Now, principal.Claims.ToList());
            }
            
            return;

        }
    }
}
