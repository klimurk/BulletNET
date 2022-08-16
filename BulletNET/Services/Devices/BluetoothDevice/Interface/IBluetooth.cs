using BulletNET.Services.Devices.Base;

namespace BulletNET.Services.Devices.BluetoothDevice.Interface
{
    public interface IBluetooth : ITest
    {
        bool isEnabled { get; set; }
        bool BLEMessageReceived { get; set; }
        byte BoardStatusPacket { get; set; }
        bool AdvertisementReceived { get; set; }

        void Pair();

        void StartListening();

        //void SubscribeToCharacteristicNotifications(bool firstConnection);

        string ToString();

        Task<bool> Unpair();

        void WriteBytes(byte[] data);

        void WritePWR(ushort pwr1, ushort pwr2);

        bool TestCharging();

        bool TestFinishCharge();

        bool TestChargeInProgress();

        bool SwitchToTestMode();

        bool PairTest();
    }
}