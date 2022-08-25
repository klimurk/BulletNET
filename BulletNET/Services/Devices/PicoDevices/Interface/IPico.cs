using BulletNET.Services.Devices.Base;

namespace BulletNET.Services.Devices.PicoDevices.Interface
{
    public interface IPico : ITest
    {

        void Connect();

        bool CheckFrequency(double minimum, double maximum, string TestName);

        bool CheckVoltage(double minimum, double maximum, string valueName, string channel);
    }
}