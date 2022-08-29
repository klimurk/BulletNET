using BulletNET.Database.Entities.Base;
using System.ComponentModel.DataAnnotations.Schema;

namespace BulletNET.EntityFramework.Entities.Radar;

[Table("TestAction")]
public class TestAction : NamedEntity, ICloneable
{
    [Column("ValueName", TypeName = "text")]
    public new string Name { get; set; }

    [Column("Measured")]
    public double? Measured { get; set; }

    [Column("Maximum")]
    public double? Maximum { get; set; }

    [Column("Minimun")]
    public double? Minimum { get; set; }

    [Column("Passed")]
    public bool? IsPassed
    {
        get => _IsPassed;
        set
        {
            _IsPassed = value;
            IsRunning = false;
            Application.Current.Dispatcher.Invoke(() => { RefreshEvents?.Invoke(this, EventArgs.Empty); });
        }
    }

    private bool? _IsPassed;

    [ForeignKey("TestGroupID")]
    [InverseProperty("TestActions")]
    public TestGroup TestGroup { get; set; }

    //============================
    [NotMapped]
    public bool IsRunning { get; set; }

    public event EventHandler RefreshEvents;

    public object Clone() => MemberwiseClone();


    [NotMapped]
    public int groupId { get; set; }
}