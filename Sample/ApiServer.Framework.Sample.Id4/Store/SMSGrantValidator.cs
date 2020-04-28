using IdentityServer4.Models;
using IdentityServer4.Validation;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ApiServer.Framework.Sample.Id4.Store
{
    public class SMSGrantValidator: IExtensionGrantValidator
    {
        public string GrantType => ExtensionGrantTypes.SMSGrantType;

        private readonly UserManager<AppIdentityUser> _userManager;
        private readonly IUserClaimsPrincipalFactory<AppIdentityUser> _claimsFactory;

        public SMSGrantValidator(UserManager<AppIdentityUser> userManager, IUserClaimsPrincipalFactory<AppIdentityUser> claimsFactory) {
            _userManager = userManager;
            _claimsFactory = claimsFactory;
        }

        public async Task ValidateAsync(ExtensionGrantValidationContext context)
        {
            var smsId = context.Request.Raw.Get("sms_id");
            var inputCode = context.Request.Raw.Get("input_code");

            if (string.IsNullOrEmpty(smsId) || string.IsNullOrEmpty(inputCode))
            {
                context.Result = new GrantValidationResult(TokenRequestErrors.InvalidGrant);
            }

            if (smsId == "123456789" && inputCode == "654321")
            {
                var user = await _userManager.FindByIdAsync("1");
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
            return;

        }
    }
}
