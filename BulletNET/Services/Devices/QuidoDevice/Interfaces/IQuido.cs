using BulletNET.Services.Devices.Base;

namespace BulletNET.Services.Devices.QuidoDevice.Interfaces
{
    public interface IQuido : ITest
    {

        void Read();

        bool Set(byte ADR, bool State);

        void SetAllOff();

        void Start();

        bool CommunicationTest(byte relay, bool relayON);

        bool Flash(string firmwareName);

        bool EraseFirmware();
    }
}