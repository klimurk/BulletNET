using BulletNET.Services.Devices.Base;

namespace BulletNET.Services.Devices.MansonDevice.Interface
{
    public interface IManson : ITest
    {

        bool GETD();

        bool GETS();

        void Start();

        bool TestCURR(int Value);

        bool TestVOLT(int Value);

        bool MeasureCurrent(double minimum, double maximum, string TestName);

        int VOLTAGE { get; }

        int CURRENT { get; }
        int C_VOLTAGE { get; }
        int C_CURRENT { get; }
        bool STATUS { get; }
    }
}