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

namespace ApiServer.Framework.Sample.Service
{
    public class UserService: IUserResourceProvider
    {
        public IUnitOfWork Uow { get; set; }

        public List<string> GetUserResources(dynamic id)
        {
            string uid = id;
            var user = Uow.GetRepository<User>().GetFirstOrDefault(
            predicate: p => p.Id == uid
            , include: p => p.Include(b => b.UserRoles)
            .ThenInclude(a => a.Role)
            .ThenInclude(a => a.Resources)
            .ThenInclude(a => a.Resource));

            if (user == null)
                throw new BizException($"用户{uid}不存在");
            return user.Roles.SelectMany(p => p.Resources.Select(a => a.Resource.Code)).ToList();

        }

        public User LoginByPwd(LoginByPwdReq dto) {

            string inputPwdMd5 = dto.Password.Md5();
            User user = Uow.GetRepository<User>().GetFirstOrDefault(
               predicate: w => w.Account == dto.Account
                           && w.Password == inputPwdMd5,
               include: p => p.Include(b => b.UserRoles).ThenInclude(a => a.Role)
               );
            if (user == null)
            {
                throw new BizException("用户名/密码错误");
            }
            return user;

        }

    }
}
