using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ApiServer.Framework.Sample.Id4.Store
{
    public class AppIdentityRoleStore : IRoleStore<AppIdentityRole>
    {
        public List<AppIdentityRole> roles;

        public AppIdentityRoleStore() {
            roles = new List<AppIdentityRole> { 
                new AppIdentityRole { 
                     Id="1", Name="用户"
                },
                new AppIdentityRole {
                     Id="2", Name="管理用户"
                },
            };
        }


        public Task<IdentityResult> CreateAsync(AppIdentityRole role, CancellationToken cancellationToken)
        {
            //throw new NotImplementedException();
            return Task.FromResult(IdentityResult.Success);
        }

        public Task<IdentityResult> DeleteAsync(AppIdentityRole role, CancellationToken cancellationToken)
        {
            //throw new NotImplementedException();
            return Task.FromResult(IdentityResult.Success);
        }

        public void Dispose()
        {
            //throw new NotImplementedException();
        }

        public Task<AppIdentityRole> FindByIdAsync(string roleId, CancellationToken cancellationToken)
        {
            //throw new NotImplementedException();
            return Task.FromResult(this.roles.FirstOrDefault(r => r.Id == roleId));
        }

        public Task<AppIdentityRole> FindByNameAsync(string normalizedRoleName, CancellationToken cancellationToken)
        {
            return Task.FromResult(this.roles.FirstOrDefault(r => r.Name == normalizedRoleName));
        }

        public Task<string> GetNormalizedRoleNameAsync(AppIdentityRole role, CancellationToken cancellationToken)
        {
            return Task.FromResult(role.Name);
        }

        public Task<string> GetRoleIdAsync(AppIdentityRole role, CancellationToken cancellationToken)
        {
            return Task.FromResult(role.Id);
        }

        public Task<string> GetRoleNameAsync(AppIdentityRole role, CancellationToken cancellationToken)
        {
            return Task.FromResult(role.Name);
        }

        public Task SetNormalizedRoleNameAsync(AppIdentityRole role, string normalizedName, CancellationToken cancellationToken)
        {
            role.Name = normalizedName;
            return Task.CompletedTask;
        }

        public Task SetRoleNameAsync(AppIdentityRole role, string roleName, CancellationToken cancellationToken)
        {
            role.Name = roleName;
            return Task.CompletedTask;
        }

        public Task<IdentityResult> UpdateAsync(AppIdentityRole role, CancellationToken cancellationToken)
        {
            return Task.FromResult(IdentityResult.Success);
        }
    }
}
