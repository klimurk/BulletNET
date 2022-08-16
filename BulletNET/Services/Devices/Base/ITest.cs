namespace BulletNET.Services.Devices.Base;

public interface ITest
{
    bool isEnabled { get; set; }
    string TestName { get; }
    DateTime TimeStamp { get; }
    bool IsScheduled { get; }
    bool IsRunning { get; }
    bool IsPassed { get; }

    public double Measured { get; }
    bool IsError { get; }
}