using PS2000AImports;

namespace BulletNET.Services.Devices.PicoDevices.PicoSupport
{
    internal struct ChannelSettings
    {
        public Imports.CouplingType couplingType;
        public Imports.Range range;
        public bool enabled;
    }
}