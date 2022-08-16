using BulletNET.Services.Devices.Base;

namespace BulletNET.Services.DelayService;

public interface IDelay : ITest
{
    bool MakeDelay(int millis);
}