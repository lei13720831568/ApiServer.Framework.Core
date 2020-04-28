using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ApiServer.Framework.Sample.Id4.Store
{
    public class AppUserClaimsPrincipalFactory : UserClaimsPrincipalFactory<AppIdentityUser>
    {
        public AppUserClaimsPrincipalFactory(UserManager<AppIdentityUser> userManager, IOptions<IdentityOptions> optionsAccessor) 
            : base(userManager, optionsAccessor)
        {
        }

        protected override async Task<ClaimsIdentity> GenerateClaimsAsync(AppIdentityUser user)
        {

            var id = await base.GenerateClaimsAsync(user);
            id.AddClaim(new Claim("mobile",user.Mobile ));
            id.AddClaim(new Claim("title", user.Title));
            id.AddClaim(new Claim("name", user.Name));
            return  id;
        }
    }
}
