using BulletNET.Database.Entities.Base;
using BulletNET.EntityFramework.Entities.Users;
using System.ComponentModel.DataAnnotations.Schema;

namespace BulletNET.EntityFramework.Entities.Radar;

[Table("TestGroup")]
public class TestGroup : NamedEntity, ICloneable
{
    [Column("TimeStamp")]
    public DateTime TimeStamp { get; set; }

    [Column("Name", TypeName = "text")]
    public new string Name { get; set; }

    [Column("UserID")]
    public User User { get; set; }

    [InverseProperty("TestGroup")]
    public ICollection<TestAction> TestActions { get; set; } = new HashSet<TestAction>();

    [ForeignKey("RadarBoardID")]
    [InverseProperty("TestGroups")]
    public RadarBoard RadarBoard { get; set; }

    public Comment? Comment { get; set; }

    //==========================================

    [NotMapped]
    public bool IsBusy { get; set; }

    [NotMapped]
    public bool? IsPassed { get; set; }

    public event EventHandler TestEvents;

    public void OnTestEvents()
    {
        IsBusy = true;
        TestEvents?.Invoke(this, EventArgs.Empty);
        IsPassed = TestActions.All(s => s.IsPassed is not null && (bool)s.IsPassed);
        IsBusy = false;
    }

    public object Clone()
    {
        TestGroup testGroup = new()
        {
            Name = Name,
            TestEvents = TestEvents
        };
        foreach (var action in TestActions)
        {
            TestAction? act = (TestAction)action.Clone();
            act.TestGroup = testGroup;
            testGroup.TestActions.Add(act);
        }
        return testGroup;
    }
}