using BulletNET.Database.Entities.Base.Interfaces;
using System.ComponentModel.DataAnnotations.Schema;

namespace BulletNET.EntityFramework.Entities.Base
{
    public abstract class Entity : IEntity
    {
        [Column(Order = 0, TypeName = "int(11)")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
    }
}