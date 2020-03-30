using ApiServer.Framework.Core.DB.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace ApiServer.Framework.Sample.Entity.Models
{
    public partial class RoleResource : TrackableEntity
    {
        public Role Role { get; set; }
        public Resource Resource { get; set; }
    }

    /// <summary>
    /// 角色资源配置文件
    /// </summary>
    public class RoleResourceConfiguration : IEntityTypeConfiguration<RoleResource>
    {
        public void Configure(EntityTypeBuilder<RoleResource> builder)
        {
            builder.HasKey(e => new { e.RoleId, e.ResourceId });
        }
    }
}
