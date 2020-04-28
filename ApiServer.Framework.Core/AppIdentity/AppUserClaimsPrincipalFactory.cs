using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ApiServer.Framework.Core.AppIdentity
{
    public class AppUserClaimsPrincipalFactory : UserClaimsPrincipalFactory<AppUser>
    {
        public AppUserClaimsPrincipalFactory(UserManager<AppUser> userManager, IOptions<IdentityOptions> optionsAccessor) 
            : base(userManager, optionsAccessor)
        {
        }

        protected override  Task<ClaimsIdentity> GenerateClaimsAsync(AppUser user)
        {
            var id = new ClaimsIdentity("Identity.Application", // REVIEW: Used to match Application scheme
                Options.ClaimsIdentity.UserNameClaimType,
                Options.ClaimsIdentity.RoleClaimType);
            id.AddClaim(new Claim(Options.ClaimsIdentity.UserIdClaimType, user.Id));
            id.AddClaim(new Claim(Options.ClaimsIdentity.UserNameClaimType, user.Account));
            id.AddClaim(new Claim("mobile",user.Mobile ));
            id.AddClaim(new Claim("title", user.Title));
            id.AddClaim(new Claim("name", user.Name));
            id.AddClaim(new Claim("system_id",user.SystemId.ToString()));
            return Task.FromResult(id);
        }
    }
}
