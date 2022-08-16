using BulletNET.Database.Entities.Base.Interfaces;
using BulletNET.EntityFramework.Entities.Base;
using System.ComponentModel.DataAnnotations;

namespace BulletNET.Database.Entities.Base
{
    public abstract class NamedEntity : Entity, INamedEntity
    {
        [Required]
        public virtual string Name { get; set; }
    }
}