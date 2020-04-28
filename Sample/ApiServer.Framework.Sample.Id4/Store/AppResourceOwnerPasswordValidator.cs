﻿using ApiServer.Framework.Core;
using IdentityModel;
using IdentityServer4.Models;
using IdentityServer4.Validation;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ApiServer.Framework.Sample.Id4.Store
{
    public class AppResourceOwnerPasswordValidator : IResourceOwnerPasswordValidator
    {
		private readonly IUserClaimsPrincipalFactory<AppIdentityUser> _claimsFactory;
		private readonly UserManager<AppIdentityUser> _userManager;

		public AppResourceOwnerPasswordValidator(UserManager<AppIdentityUser> userManager, IUserClaimsPrincipalFactory<AppIdentityUser> claimsFactory) {
			_userManager = userManager;
			_claimsFactory = claimsFactory;
		}

		public async Task ValidateAsync(ResourceOwnerPasswordValidationContext context)
        {
			var user = await _userManager.FindByNameAsync(context.UserName);
			var principal = await _claimsFactory.CreateAsync(user);

			if (user.Password == context.Password.Md5()) {

				context.Result = new GrantValidationResult(
					user.Id,
					OidcConstants.AuthenticationMethods.Password, System.DateTime.Now, principal.Claims.ToList());
			}
			else
			{
				//验证失败
				context.Result = new GrantValidationResult(TokenRequestErrors.InvalidGrant, "无效的用户名/密码");
			}
		}
    }
}
