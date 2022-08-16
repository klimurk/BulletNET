using BulletNET.Database.Entities.Base;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BulletNET.EntityFramework.Entities.Users
{
    [Table("PROG_USER")]
    public class User : NamedEntity
    {
        [Required]
        [Column("USER_NAME", Order = 1, TypeName = "varchar(50)")]
        public new string Name { get; set; }

        [Column("USER_DESC", TypeName = "nvarchar(200)")]
        public string Description { get; set; }

        [Column("USER_ROLE")]
        public int RoleNum { get; set; }

        [Column("USER_HASH", TypeName = "char(64)")]
        public string Hashcode { get; set; }
    }
}