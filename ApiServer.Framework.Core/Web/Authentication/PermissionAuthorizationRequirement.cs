using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace ApiServer.Framework.Core.Web.Authentication
{
    public class PermissionAuthorizationRequirement : AuthorizationHandler<PermissionAuthorizationRequirement>, IAuthorizationRequirement
    {
        public PermissionAuthorizeData AuthorizeData { get; }

        public PermissionAuthorizationRequirement(PermissionAuthorizeData authorizeData)
        {
            AuthorizeData = authorizeData;
        }

        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, PermissionAuthorizationRequirement requirement)
        {
            if (context.User == null)
            {
                return Task.CompletedTask;
            }

            if (context.User.Claims.Count() == 0) {
                return Task.CompletedTask;
            }

            // 以半角逗号分隔的权限满足"需要"的其中之一即可。
            // 分组、角色和权限三者在此也是 Or 的关系，所以是在尽力去找任一匹配。
            var found = string.IsNullOrWhiteSpace(requirement.AuthorizeData.Permissions);
            if (requirement.AuthorizeData.Permissions != null && found == false)
            {
                
                var permissionsClaim = context.User.Claims.Where(c => c.Type.StartsWith("permession:")).Select(c=>c.Value);
                if (permissionsClaim != null )
                {
                    var permissionsClaimSplit = permissionsClaim;
                    var permissionsDataSplit = SafeSplit(requirement.AuthorizeData.Permissions);
                    found = permissionsDataSplit.Intersect(permissionsClaimSplit).Any();
                }
            }

            //if (!found && requirement.AuthorizeData.Roles != null)
            //{
            //    var rolesClaim = context.User.Claims.FirstOrDefault(c => string.Equals(c.Type, ClaimTypes.Role, StringComparison.OrdinalIgnoreCase));
            //    if (rolesClaim?.Value != null && rolesClaim.Value.Length > 0)
            //    {
            //        var rolesClaimSplit = SafeSplit(rolesClaim.Value);
            //        var rolesDataSplit = SafeSplit(requirement.AuthorizeData.Roles);
            //        found = rolesDataSplit.Intersect(rolesClaimSplit).Any();
            //    }
            //}


            if (found)
            {
                context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }

        private IEnumerable<string> SafeSplit(string source)
        {
            return source.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(m => m.Trim()).Where(m => !string.IsNullOrWhiteSpace(m));
        }
    }
}
