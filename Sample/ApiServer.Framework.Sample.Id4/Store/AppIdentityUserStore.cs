using AgileObjects.AgileMapper;
using AgileObjects.AgileMapper.Extensions;
using ApiServer.Framework.Core;
using ApiServer.Framework.Sample.Service;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ApiServer.Framework.Sample.Id4.Store
{
    public class AppIdentityUserStore : IUserStore<AppIdentityUser>, IUserPasswordStore<AppIdentityUser>
    {
        private readonly UserService _userService;

        public AppIdentityUserStore(UserService userService) {
            _userService = userService;
        }

        public Task<IdentityResult> CreateAsync(AppIdentityUser user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            try
            {
                var userEntity = user.Map().ToANew<Entity.Models.User>();
                userEntity.Status = 0;
                _userService.Create(userEntity);
                return Task.FromResult(IdentityResult.Success);
            }
            catch (Exception ex) {
                return Task.FromResult(IdentityResult.Failed(new IdentityError { Code = "9999", Description = ex.Message }));
            }

        }

        public Task<IdentityResult> DeleteAsync(AppIdentityUser user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            try
            {
                //2=禁用
                _userService.ChangeStatus(user.Id, 2);
                return Task.FromResult(IdentityResult.Success);
            }
            catch (Exception ex)
            {
                return Task.FromResult(IdentityResult.Failed(new IdentityError { Code = "9999", Description = ex.Message }));
            }
        }

        public void Dispose()
        {
            //throw new NotImplementedException();
        }

        public Task<AppIdentityUser> FindByIdAsync(string userId, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            try
            {
                var user = _userService.FindById(userId);
                return Task.FromResult(user.Map().ToANew<AppIdentityUser>());
            }
            catch (Exception)
            {
                return Task.FromResult<AppIdentityUser>(null);
            }
        }

        public Task<AppIdentityUser> FindByNameAsync(string normalizedUserName, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            try
            {
                var user = _userService.FindByAccount(normalizedUserName);
                return Task.FromResult(user.Map().ToANew<AppIdentityUser>());
            }
            catch (Exception)
            {
                return Task.FromResult<AppIdentityUser>(null);
            }
        }

        public Task<string> GetNormalizedUserNameAsync(AppIdentityUser user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.Account);
        }

        public Task<string> GetPasswordHashAsync(AppIdentityUser user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.Password);
        }

        public Task<string> GetUserIdAsync(AppIdentityUser user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.Id);
        }

        public Task<string> GetUserNameAsync(AppIdentityUser user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.Account);
        }

        public Task<bool> HasPasswordAsync(AppIdentityUser user, CancellationToken cancellationToken)
        {
            return Task.FromResult(true);
        }

        public Task SetNormalizedUserNameAsync(AppIdentityUser user, string normalizedName, CancellationToken cancellationToken)
        {
            user.Account = user.Account.ToLower();
            return Task.CompletedTask;
        }

        public Task SetPasswordHashAsync(AppIdentityUser user, string passwordHash, CancellationToken cancellationToken)
        {
            user.Password = passwordHash;
            return Task.CompletedTask;
        }

        public Task SetUserNameAsync(AppIdentityUser user, string userName, CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        public Task<IdentityResult> UpdateAsync(AppIdentityUser user, CancellationToken cancellationToken)
        {
            return Task.FromResult<IdentityResult>(IdentityResult.Success);
        }
    }
}
