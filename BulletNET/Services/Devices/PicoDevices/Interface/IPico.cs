using BulletNET.Services.Devices.Base;

namespace BulletNET.Services.Devices.PicoDevices.Interface
{
    public interface IPico : ITest
    {
        bool isEnabled { get; set; }

        void Connect();


        bool ReadFreq(double minimum, double maximum, string TestName);

        double[] ReadVoltage();

        void SetSignalGenerator(bool ON, uint pkToPk, uint frequency);

        bool MeasureVoltage(double minimum, double maximum, string valueName, string channel);
    }
}