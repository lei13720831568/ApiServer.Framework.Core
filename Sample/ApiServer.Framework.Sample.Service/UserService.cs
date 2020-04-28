using ApiServer.Framework.Core;
using ApiServer.Framework.Core.Exceptions;
using ApiServer.Framework.Sample.Entity.Models;
using Arch.EntityFrameworkCore.UnitOfWork;
using System;
using Microsoft.EntityFrameworkCore;
using ApiServer.Framework.Sample.Dto;
using ApiServer.Framework.Core.Web.Permission;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;
using System.Threading;
using ApiServer.Framework.Core.AppIdentity;
using AgileObjects.AgileMapper.Extensions;

namespace ApiServer.Framework.Sample.Service
{
    public class UserService: IDependency, IUserStore<AppUser>, IUserPasswordStore<AppUser>
    {
        public IUnitOfWork Uow { get; set; }

        public void Create(User user) {
            Uow.GetRepository<User>().Insert(user);
            Uow.SaveChanges();
        }

        public void ChangeStatus(string userid, int status) {
            
            var repo = Uow.GetRepository<User>();
            var user = repo.GetFirstOrDefault(predicate: u => u.Id == userid && u.Status != 2);
            if (user == null) throw new BizException("用户不存在");
            user.Status = status;
            Uow.SaveChanges();
        }

        public User FindById(string userid) {
            var repo = Uow.GetRepository<User>();
            var user = repo.GetFirstOrDefault(predicate: u => u.Id == userid && u.Status != 2);
            if (user == null) throw new BizException("用户不存在");
            return user;
        }

        public User FindByAccount(string account)
        {
            var repo = Uow.GetRepository<User>();
            var user = repo.GetFirstOrDefault(predicate: u => u.Account == account && u.Status != 3);
            if (user == null) throw new BizException("用户不存在");
            return user;
        }

        Task<IdentityResult> IUserStore<AppUser>.CreateAsync(AppUser user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            try
            {
                var userEntity = user.Map().ToANew<Entity.Models.User>();
                userEntity.Status = 0;
                this.Create(userEntity);
                return Task.FromResult(IdentityResult.Success);
            }
            catch (Exception ex)
            {
                return Task.FromResult(IdentityResult.Failed(new IdentityError { Code = "9999", Description = ex.Message }));
            }
        }

        Task<IdentityResult> IUserStore<AppUser>.DeleteAsync(AppUser user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            try
            {
                //2=禁用
                this.ChangeStatus(user.Id, 2);
                return Task.FromResult(IdentityResult.Success);
            }
            catch (Exception ex)
            {
                return Task.FromResult(IdentityResult.Failed(new IdentityError { Code = "9999", Description = ex.Message }));
            }
        }

        Task<AppUser> IUserStore<AppUser>.FindByIdAsync(string userId, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            try
            {
                var user = this.FindById(userId);
                return Task.FromResult(user.Map().ToANew<AppUser>());
            }
            catch (Exception)
            {
                return Task.FromResult<AppUser>(null);
            }
        }

        Task<AppUser> IUserStore<AppUser>.FindByNameAsync(string normalizedUserName, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            try
            {
                var user = this.FindByAccount(normalizedUserName);
                return Task.FromResult(user.Map().ToANew<AppUser>());
            }
            catch (Exception)
            {
                return Task.FromResult<AppUser>(null);
            }
        }

        Task<string> IUserStore<AppUser>.GetNormalizedUserNameAsync(AppUser user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.Account);
        }

        Task<string> IUserStore<AppUser>.GetUserIdAsync(AppUser user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.Id);
        }

        Task<string> IUserStore<AppUser>.GetUserNameAsync(AppUser user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.Account);
        }

        Task IUserStore<AppUser>.SetNormalizedUserNameAsync(AppUser user, string normalizedName, CancellationToken cancellationToken)
        {
            user.Account = normalizedName.ToLower();
            return Task.CompletedTask;
        }

        Task IUserStore<AppUser>.SetUserNameAsync(AppUser user, string userName, CancellationToken cancellationToken)
        {
            user.Account = userName.ToLower();
            return Task.CompletedTask;
        }

        Task<IdentityResult> IUserStore<AppUser>.UpdateAsync(AppUser user, CancellationToken cancellationToken)
        {
            return Task.FromResult<IdentityResult>(IdentityResult.Success);
        }

        void IDisposable.Dispose()
        {
            
        }

        Task<string> IUserPasswordStore<AppUser>.GetPasswordHashAsync(AppUser user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.Password);
        }

        Task<bool> IUserPasswordStore<AppUser>.HasPasswordAsync(AppUser user, CancellationToken cancellationToken)
        {
            return Task.FromResult(true);
        }

        Task IUserPasswordStore<AppUser>.SetPasswordHashAsync(AppUser user, string passwordHash, CancellationToken cancellationToken)
        {
            user.Password = passwordHash;
            return Task.CompletedTask;
        }
    }
}
