using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ApiServer.Framework.Core.DB.Entity;

namespace ApiServer.Framework.Sample.Entity.Models
{
[Table("user")]
    public partial class User: TrackableEntity
    {
        [Key]
        [Column("id", TypeName = "varchar(255)")]
        public string Id { get; set; }
        [Required]
        [Column("account", TypeName = "varchar(255)")]
        public string Account { get; set; }
        [Column("create_at")]
        public DateTime CreateAt { get; set; }
        [Required]
        [Column("password", TypeName = "varchar(255)")]
        public string Password { get; set; }
    }
}
