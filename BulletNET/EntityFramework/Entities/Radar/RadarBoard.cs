using BulletNET.EntityFramework.Entities.Base;
using System.ComponentModel.DataAnnotations.Schema;

namespace BulletNET.EntityFramework.Entities.Radar;

[Table("MainRadarBoardPairing")]
public class RadarBoard : Entity
{
    [Column("mainBoardID", TypeName = "tinytext")]
    public string MainBoardID { get; set; }

    [Column("radarBoardID", TypeName = "tinytext")]
    public string RadarBoardID { get; set; }

    [InverseProperty("RadarBoard")]
    public ICollection<TestGroup> TestGroups { get; set; } = new HashSet<TestGroup>();
}