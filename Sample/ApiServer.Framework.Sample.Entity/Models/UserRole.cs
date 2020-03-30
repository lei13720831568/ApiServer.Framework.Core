using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ApiServer.Framework.Core.DB.Entity;

namespace ApiServer.Framework.Sample.Entity.Models
{
[Table("user_role")]
    public partial class UserRole: TrackableEntity
    {
        [Key]
        [Column("id", TypeName = "int(11)")]
        public int Id { get; set; }
        [Required]
        [Column("user_id", TypeName = "varchar(255)")]
        public string UserId { get; set; }
        [Column("role_id", TypeName = "int(11)")]
        public int RoleId { get; set; }
    }
}
