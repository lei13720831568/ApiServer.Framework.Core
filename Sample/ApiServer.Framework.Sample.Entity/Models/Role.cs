using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ApiServer.Framework.Core.DB.Entity;

namespace ApiServer.Framework.Sample.Entity.Models
{
[Table("role")]
    public partial class Role: TrackableEntity
    {
        [Key]
        [Column("id", TypeName = "int(11)")]
        public int Id { get; set; }
        [Required]
        [Column("name", TypeName = "varchar(255)")]
        public string Name { get; set; }
    }
}
