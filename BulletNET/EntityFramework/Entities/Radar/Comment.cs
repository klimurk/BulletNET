using BulletNET.EntityFramework.Entities.Base;
using BulletNET.EntityFramework.Entities.Users;
using System.ComponentModel.DataAnnotations.Schema;

namespace BulletNET.EntityFramework.Entities.Radar;

[Table("Comments")]
public class Comment : Entity
{
    [Column("Title", TypeName = "text")]
    public string Title { get; set; }

    [Column("Comment", TypeName = "text")]
    public string Text { get; set; }

    [Column("UserID")]
    public User User { get; set; }

    [Column("datetime")]
    public DateTime TimeStamp { get; set; }
}
