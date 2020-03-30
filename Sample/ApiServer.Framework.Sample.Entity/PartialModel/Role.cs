using ApiServer.Framework.Core.DB.Entity;
using ApiServer.Framework.Sample.Entity.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace ApiServer.Framework.Sample.Entity.Models
{
    public partial class Role : TrackableEntity
    {
        /// <summary>
        /// 资源
        /// </summary>
        /// <value>The resources.</value>
        public List<RoleResource> Resources { get; set; }

    }

    public class RoleConfiguration : IEntityTypeConfiguration<Role>
    {
        public void Configure(EntityTypeBuilder<Role> builder)
        {
            builder.HasKey(e => e.Id);
        }
    }
}
