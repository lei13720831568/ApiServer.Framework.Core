using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ApiServer.Framework.Core.DB.Entity;

namespace ApiServer.Framework.Sample.Entity.Models
{
[Table("role_resource")]
    public partial class RoleResource: TrackableEntity
    {
        [Key]
        [Column("id", TypeName = "int(11)")]
        public int Id { get; set; }
        [Column("role_id", TypeName = "int(11)")]
        public int RoleId { get; set; }
        [Column("resource_id", TypeName = "int(11)")]
        public int ResourceId { get; set; }
    }
}
