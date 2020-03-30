using ApiServer.Framework.Core.DB.Entity;
using ApiServer.Framework.Sample.Entity.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;

namespace ApiServer.Framework.Sample.Entity.Models
{
    public partial class User : TrackableEntity
    {
        /// <summary>
        /// 用户角色关联列表
        /// </summary>
        /// <value>The user roles.</value>
        public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();

        /// <summary>
        /// 用户角色列表
        /// </summary>
        /// <value>The roles.</value>
        [NotMapped]
        public ICollection<Role> Roles => UserRoles.Select(e => e.Role).ToList();

    }

    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.HasKey(e => e.Id);
        }
    }

}
