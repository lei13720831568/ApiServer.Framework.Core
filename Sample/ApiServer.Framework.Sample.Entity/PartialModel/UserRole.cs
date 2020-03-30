using ApiServer.Framework.Core.DB.Entity;
using ApiServer.Framework.Sample.Entity.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace ApiServer.Framework.Sample.Entity.Models
{
    public partial class UserRole : TrackableEntity
    {
        public User User { get; set; }
        public Role Role { get; set; }
    }

    public class UserRoleConfiguration : IEntityTypeConfiguration<UserRole>
    {
        public void Configure(EntityTypeBuilder<UserRole> builder)
        {
            builder.HasKey(e => new { e.UserId, e.RoleId });
            builder.HasOne(e => e.User)
                .WithMany(e => e.UserRoles);
        }
    }
}
