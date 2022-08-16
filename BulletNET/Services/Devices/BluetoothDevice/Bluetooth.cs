using BulletNET.Services.Devices.Base;
using BulletNET.Services.Devices.BluetoothDevice.Interface;
using BulletNET.Services.Devices.MansonDevice.Interface;
using BulletNET.Services.Devices.PicoDevices.Interface;
using BulletNET.Services.Devices.QuidoDevice.Interfaces;
using Pallet.Services.UserDialogService.Interfaces;
using Windows.Devices.Bluetooth;
using Windows.Devices.Bluetooth.Advertisement;
using Windows.Devices.Bluetooth.GenericAttributeProfile;
using Windows.Devices.Enumeration;
using Windows.Storage.Streams;

namespace BulletNET.Services.Devices.BluetoothDevice
{
    public class Bluetooth : Test, IBluetooth
    {
        #region Services

        private readonly IPico _IPico;
        private readonly IQuido _IQuido;
        private readonly IManson _IManson;
        private readonly IUserDialogService _IUserDialogService;

        #endregion Services

        #region fields

        #region private

        private readonly BluetoothLEAdvertisementWatcher _watcher = new BluetoothLEAdvertisementWatcher();
        private ulong _lastAdvertisementAdress;
        private BluetoothLEDevice _BSdevice;
        private GattCharacteristic _characteristic_notify;
        private GattCharacteristic _characteristic_write;

        private readonly string __ServiceId = "49535343-fe7d-4ae5-8fa9-9fafd205e455";
        private readonly string __WriteCharacteristicsId = "49535343-8841-43f4-a8d4-ecbe34729bb3";
        private readonly string __NotifyCharacteristicsId = "49535343-1e4d-4bd9-ba61-23c647249616";
        public bool AdvertisementReceived { get; set; }

        private GattCharacteristicsResult characteristicResult;
        private ulong previousPairAdress;

        #endregion private

        #region public

        public bool BLEMessageReceived { get; set; }
        public byte BoardStatusPacket { get; set; }
        public char BoardStatus => Convert.ToChar(BoardStatusPacket);

        public bool IsConnected { get; set; }
        public bool isEnabled { get; set; }

        #endregion public

        #endregion fields

        public Bluetooth(
            IPico IPico, IQuido IQuido, IManson IManson,
            IUserDialogService IUserDialogService)
        {
            _IPico = IPico;
            _IQuido = IQuido;
            _IManson = IManson;
            _IUserDialogService = IUserDialogService;

            IsScheduled = true;
        }

        public void StartListening()
        {
            _watcher.Received += OnAdvertisementReceived;
            _watcher.ScanningMode = BluetoothLEScanningMode.Passive;
            _watcher.Start();
            isEnabled = true;
        }

        #region Events

        private void OnAdvertisementReceived(BluetoothLEAdvertisementWatcher sender, BluetoothLEAdvertisementReceivedEventArgs args)
        {
            if (args.Advertisement.LocalName == "B-SEEKER")
            {
                _lastAdvertisementAdress = args.BluetoothAddress;
                AdvertisementReceived = true;
            }
        }

        private void OnCharacteristic_ValueChanged(GattCharacteristic sender, GattValueChangedEventArgs args)
        {
            var reader = DataReader.FromBuffer(args.CharacteristicValue);

            switch ((char)reader.ReadByte())
            {
                case 'T':
                    ProcessShotPacket(reader);
                    break;

                case 'S':
                    ProcessStatusPacket(reader);
                    break;

                case 'J':
                    break;

                case 'V':
                    break;

                case 'L':
                    break;
            }
        }

        #endregion Events

        #region Pairing

        public async void Pair()
        {
            int attempts = 0;
            bool forceCharacteristicReload = false;

            while (attempts < 6)
            {
                _BSdevice = await BluetoothLEDevice.FromBluetoothAddressAsync(_lastAdvertisementAdress);

                if (_BSdevice == null)
                {
                    Console.WriteLine("BulletSeeker device not found");
                    return;
                }

                if (!_BSdevice.DeviceInformation.Pairing.IsPaired)
                {
                    _BSdevice.DeviceInformation.Pairing.Custom.PairingRequested += (sender, args) => { args.Accept(); };

                    DevicePairingResult pairingResult = await _BSdevice.DeviceInformation.Pairing.Custom.PairAsync(DevicePairingKinds.ConfirmOnly, DevicePairingProtectionLevel.EncryptionAndAuthentication);

                    if (pairingResult.Status != DevicePairingResultStatus.Paired)
                    {
                        Console.WriteLine("Pairing failed: " + pairingResult.Status.ToString());
                        _BSdevice.Dispose();
                        continue;
                    }
                    try
                    {

                        GattDeviceServicesResult GattServices = await _BSdevice.GetGattServicesAsync();
                        if (GattServices.Status == GattCommunicationStatus.Success)
                        {
                            var services = GattServices.Services;
                        }
                    }
                    catch
                    {
                        _IUserDialogService.ShowError("Bluetooth is disabled", "Bluetooth");
                    }

                }

                bool notifyOK = false;
                bool writeOK = false;

                try
                {
                    GattDeviceServicesResult GattServices = await _BSdevice.GetGattServicesAsync(BluetoothCacheMode.Uncached);

                    foreach (var service in GattServices.Services)
                    {
                        if (service.Uuid.ToString() == __ServiceId)
                        {
                            if (characteristicResult == null || forceCharacteristicReload)
                            {
                                characteristicResult = await service.GetCharacteristicsAsync();

                                if (characteristicResult.Status == GattCommunicationStatus.AccessDenied)
                                {
                                    Console.WriteLine("BLE ACCES DENIED");
                                }
                            }

                            if (characteristicResult.Status == GattCommunicationStatus.Success)
                            {
                                var characteristics = characteristicResult.Characteristics;

                                foreach (var characteristic in characteristics)
                                {
                                    if (characteristic.Uuid.ToString() == __NotifyCharacteristicsId)
                                    {
                                        _characteristic_notify = characteristic;
                                        GattCommunicationStatus status = await _characteristic_notify.WriteClientCharacteristicConfigurationDescriptorAsync(
                                                              GattClientCharacteristicConfigurationDescriptorValue.Notify);
                                        if (status == GattCommunicationStatus.Success)
                                        {
                                            // Server has been informed of clients interest.
                                            _characteristic_notify.ValueChanged += OnCharacteristic_ValueChanged;
                                            Console.WriteLine("BLE NOTIFY OK");
                                            notifyOK = true;
                                        }
                                    }
                                    if (characteristic.Uuid.ToString() == __WriteCharacteristicsId)
                                    {
                                        _characteristic_write = characteristic;
                                        Console.WriteLine("BLE WRITE OK");
                                        writeOK = true;
                                    }
                                }
                            }
                        }
                    }
                    if (notifyOK && writeOK)
                    {
                        Console.WriteLine("BLE characteristic OK");
                        break;
                    }

                    if (_BSdevice != null)
                    {
                        if (_BSdevice.DeviceInformation.Pairing.IsPaired)
                        {
                            await _BSdevice.DeviceInformation.Pairing.UnpairAsync();
                        }
                        _BSdevice.Dispose();
                    }
                    forceCharacteristicReload = true;

                    Console.WriteLine("BLE FAIL, repeat " + attempts);
                    attempts++;
                }
                catch (Exception ex)
                {
                    if (_BSdevice != null)
                    {
                        if (_BSdevice.DeviceInformation.Pairing.IsPaired)
                        {
                            await _BSdevice.DeviceInformation.Pairing.UnpairAsync();
                        }
                        _BSdevice.Dispose();
                    }
                    forceCharacteristicReload = true;

                    Console.WriteLine("BLE characteristic ERROR: " + ex.Message + Environment.NewLine + "Repeat: " + attempts);
                    attempts++;
                }
            }
        }

        //public async void Pair()
        //{
        //    _BSdevice = await BluetoothLEDevice.FromBluetoothAddressAsync(_lastAdvertisementAdress);

        //    if (_BSdevice == null)
        //    {
        //        Console.WriteLine("BulletNETSeeker device not found");
        //        return;
        //    }

        //    if (_BSdevice.ConnectionStatus == BluetoothConnectionStatus.Disconnected)
        //    {
        //        _BSdevice.DeviceInformation.Pairing.Custom.PairingRequested += (sender, args) =>
        //        {
        //            args.Accept();
        //        };

        //        var sss = await _BSdevice.DeviceInformation.Pairing.Custom.PairAsync(DevicePairingKinds.ConfirmOnly, DevicePairingProtectionLevel.EncryptionAndAuthentication);

        //        GattDeviceServicesResult GattServices = await _BSdevice.GetGattServicesAsync();

        //        if (GattServices.Status == GattCommunicationStatus.Success)
        //        {
        //            var services = GattServices.Services;

        //            if (_lastAdvertisementAdress == previousPairAdress)
        //            {
        //                SubscribeToCharacteristicNotifications(false);
        //            }
        //            else
        //            {
        //                SubscribeToCharacteristicNotifications(true);
        //            }
        //        }
        //    }
        //    else
        //    {
        //        SubscribeToCharacteristicNotifications(false);
        //    }

        //    previousPairAdress = _lastAdvertisementAdress;
        //}

        public async Task<bool> Unpair()
        {
            if (_BSdevice.DeviceInformation.Pairing.IsPaired)
            {
                await _BSdevice.DeviceInformation.Pairing.UnpairAsync();
            }

            _BSdevice.Dispose();

            IsPassed = !IsError;
            return IsPassed;
        }

        #endregion Pairing

        //public async void SubscribeToCharacteristicNotifications(bool firstConnection)
        //{
        //    int attempts = 0;

        //    while (attempts < 3)
        //    {
        //        try
        //        {
        //            firstConnection = true;

        //            _BSdevice = await BluetoothLEDevice.FromBluetoothAddressAsync(_lastAdvertisementAdress);
        //            GattDeviceServicesResult GattServices = await _BSdevice.GetGattServicesAsync(BluetoothCacheMode.Uncached);

        //            foreach (var service in GattServices.Services)
        //            {
        //                if (service.Uuid.ToString() == _ServiceId)
        //                {
        //                    if (firstConnection)
        //                    {
        //                        characteristicResult = await service.GetCharacteristicsAsync();

        //                        if (characteristicResult.Status == GattCommunicationStatus.AccessDenied)
        //                        {
        //                            Console.WriteLine("BLE ACCES DENIED");
        //                            IsConnected = false;
        //                        }
        //                    }

        //                    if (characteristicResult.Status == GattCommunicationStatus.Success)
        //                    {
        //                        foreach (var characteristic in characteristicResult.Characteristics)
        //                        {
        //                            if (characteristic.Uuid.ToString() == _NotifyCharacteristicsId)
        //                            {
        //                                _characteristic_notify = characteristic;
        //                                if (await _characteristic_notify.WriteClientCharacteristicConfigurationDescriptorAsync(GattClientCharacteristicConfigurationDescriptorValue.Notify) == GattCommunicationStatus.Success)
        //                                {
        //                                    // Server has been informed of clients interest.
        //                                    _characteristic_notify.ValueChanged += OnCharacteristic_ValueChanged;
        //                                    Console.WriteLine("BLE NOTIFY OK");
        //                                }
        //                            }
        //                            if (characteristic.Uuid.ToString() == _WriteCharacteristicsId)
        //                            {
        //                                _characteristic_write = characteristic;
        //                                Console.WriteLine("BLE WRITE OK");
        //                            }
        //                        }
        //                    }
        //                }
        //            }
        //            IsConnected = true;
        //            Console.WriteLine("BLE characteristic OK");
        //            break;
        //        }
        //        catch
        //        {
        //            Console.WriteLine("BLE characteristic FAIL, repeat " + attempts);
        //            attempts++;
        //        }
        //    }
        //}

        #region Processes

        private void ProcessStatusPacket(DataReader reader)
        {
            byte battery = reader.ReadByte();
            byte lowBattery = reader.ReadByte();
            byte status = reader.ReadByte();
            var fwMajor = reader.ReadByte();
            var fwMinor = reader.ReadByte();

            BLEMessageReceived = true;
            BoardStatusPacket = status;
        }

        private void ProcessShotPacket(DataReader reader)
        {
        }

        #endregion Processes

        #region Write

        public async void WriteBytes(byte[] data)
        {
            var writer = new DataWriter();
            writer.WriteBytes(data);

            if (_BSdevice != null && _characteristic_write != null && await _characteristic_write.WriteValueAsync(writer.DetachBuffer()) == GattCommunicationStatus.Success)
            {
                Console.WriteLine("WRITE OK");
            }
        }

        public async void WritePWR(ushort pwr1, ushort pwr2)
        {
            var writer = new DataWriter();

            writer.WriteByte(0x4A); //J
            writer.WriteUInt16(pwr1);
            writer.WriteUInt16(pwr2);

            if (_BSdevice != null && _characteristic_write != null)
            {
                GattCommunicationStatus result = await _characteristic_write.WriteValueAsync(writer.DetachBuffer());
                if (result == GattCommunicationStatus.Success)
                {
                    Console.WriteLine("WRITE PWR OK");
                }
            }
        }

        #endregion Write

        // test ==========================

        public bool PairTest()
        {
            StartTest("Bluetooth pair");
            _IQuido.SetAllOff();
            Thread.Sleep(2000);
            _IQuido.Set(15, true);
            Thread.Sleep(500);
            _IQuido.Set(14, true);
            Thread.Sleep(200);
            _IQuido.Set(14, false);
            Thread.Sleep(5000);
            Pairing();
            IsPassed = !IsError;
            EndTest();
            if (!IsPassed && _IUserDialogService.ConfirmInformation("> " + TestName + " < failed. Retry?", "Test failed")) PairTest();
            return IsPassed;
        }

        private void Pairing()
        {
            BLEMessageReceived = false;
            Pair();

            Stopwatch stopwatch = new();
            stopwatch.Start();

            while (!BLEMessageReceived)
            {
                Thread.Sleep(100);
                if (stopwatch.ElapsedMilliseconds > 30000)
                {
                    Console.WriteLine("BLE pair timeout");
                    IsError = true;
                    return;
                }
            }

            Console.WriteLine("BulletNETseeker paired");
        }

        public bool SwitchToTestMode()
        {
            StartTest("Bluetooth testMode");
            _IQuido.SetAllOff();
            if (_IQuido.IsError)
            {
                IsError = _IQuido.IsError;
                IsPassed = !IsError;
                EndTest();
                if (!IsPassed && _IUserDialogService.ConfirmInformation("> " + TestName + " < failed. Retry?", "Test failed")) SwitchToTestMode();
                return IsPassed;
            }

            _IQuido.SetAllOff();
            Thread.Sleep(2000);
            _IQuido.Set(15, true);
            Thread.Sleep(500);
            _IQuido.Set(14, true);
            Thread.Sleep(200);
            _IQuido.Set(14, false);
            Thread.Sleep(3000);

            Pairing();
            if (IsError)
            {
                IsPassed = !IsError;
                EndTest();
                if (!IsPassed && _IUserDialogService.ConfirmInformation("> " + TestName + " < failed. Retry?", "Test failed")) SwitchToTestMode();
                return IsPassed;
            }

            Thread.Sleep(2000);

            BLEMessageReceived = false;

            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            while (!BLEMessageReceived)
            {
                byte[] bytes = Encoding.ASCII.GetBytes("TSTART");
                WriteBytes(bytes);

                Thread.Sleep(500);
                if (stopwatch.ElapsedMilliseconds > 5000)
                {
                    IsError = true;
                    IsPassed = !IsError;
                    EndTest();
                    if (!IsPassed && _IUserDialogService.ConfirmInformation("> " + TestName + " < failed. Retry?", "Test failed")) SwitchToTestMode();
                    return IsPassed;
                }
            }

            Unpair();
            if (BoardStatusPacket == 132) Console.WriteLine("BulletNETseeker in test mode");
            else IsError = true;

            IsPassed = !IsError;
            EndTest();
            if (!IsPassed && _IUserDialogService.ConfirmInformation("> " + TestName + " < failed. Retry?", "Test failed")) SwitchToTestMode();
            return IsPassed;
        }

        public bool TestChargeInProgress()
        {
            StartTest("Bluetooth check_chargeInProgress");
            _IQuido.SetAllOff();
            _IManson.TestVOLT(50);
            Thread.Sleep(2000);
            _IQuido.Set(18, true);
            Thread.Sleep(500);
            _IQuido.Set(14, true);
            Thread.Sleep(200);
            _IQuido.Set(14, false);
            Thread.Sleep(3000);

            Pairing();
            if (IsError)
            {
                IsPassed = !IsError;
                EndTest();
                if (!IsPassed && _IUserDialogService.ConfirmInformation("> " + TestName + " < failed. Retry?", "Test failed")) TestChargeInProgress();
                return IsPassed;
            }

            _IQuido.Set(16, true);
            Thread.Sleep(1000);

            WritePWR(65535, 65535);

            Thread.Sleep(500);
            BLEMessageReceived = false;

            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            while (!BLEMessageReceived)
            {
                Thread.Sleep(100);
                if (stopwatch.ElapsedMilliseconds > 30000)
                {
                    IsError = true;
                    IsPassed = !IsError;
                    EndTest();
                    if (!IsPassed && _IUserDialogService.ConfirmInformation("> " + TestName + " < failed. Retry?", "Test failed")) TestChargeInProgress();
                    return IsPassed;
                }
            }

            Unpair();

            _IQuido.SetAllOff();
            _IManson.TestVOLT(42);

            if (BoardStatus != 'C') IsError = true;

            EndTest();
            IsPassed = !IsError;
            if (!IsPassed && _IUserDialogService.ConfirmInformation("> " + TestName + " < failed. Retry?", "Test failed")) TestChargeInProgress();
            return IsPassed;
        }

        public bool TestFinishCharge()
        {
            StartTest("Bluetooth check_chargeComplete");
            _IQuido.SetAllOff();
            _IManson.TestVOLT(50);
            Thread.Sleep(2000);
            _IQuido.Set(17, true);
            Thread.Sleep(500);
            _IQuido.Set(14, true);
            Thread.Sleep(200);
            _IQuido.Set(14, false);
            Thread.Sleep(3000);
            Pairing();
            if (IsError)
            {
                IsPassed = !IsError;
                EndTest();
                if (!IsPassed && _IUserDialogService.ConfirmInformation("> " + TestName + " < failed. Retry?", "Test failed")) TestFinishCharge();
                return IsPassed;
            }

            _IQuido.Set(16, true);
            Thread.Sleep(500);

            WritePWR(65535, 65535);

            Thread.Sleep(1000);
            BLEMessageReceived = false;

            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            while (stopwatch.ElapsedMilliseconds < 180000)
            {
                while (!BLEMessageReceived)
                {
                    Thread.Sleep(100);
                }
                if (BoardStatus == 'D')
                {
                    break;
                }
                else
                {
                    BLEMessageReceived = false;
                    Console.WriteLine("Packet Received " + BoardStatus.ToString());
                }
            }

            Unpair();

            _IQuido.SetAllOff();
            _IManson.TestVOLT(42);

            if (BoardStatus != 'D') IsError = true;
            else Console.WriteLine("Packet Received " + BoardStatus.ToString());

            IsPassed = !IsError;
            EndTest();
            if (!IsPassed && _IUserDialogService.ConfirmInformation("> " + TestName + " < failed. Retry?", "Test failed")) TestFinishCharge();
            return IsPassed;
        }

        public bool TestCharging()
        {
            StartTest("Bluetooth check_charging");
            _IQuido.SetAllOff();
            _IManson.TestVOLT(50);
            Thread.Sleep(2000);
            _IQuido.Set(17, true);
            Thread.Sleep(500);
            _IQuido.Set(14, true);
            Thread.Sleep(200);
            _IQuido.Set(14, false);
            Thread.Sleep(3000);
            Pairing();
            if (IsError)
            {
                IsPassed = !IsError;
                EndTest();
                if (!IsPassed && _IUserDialogService.ConfirmInformation("> " + TestName + " < failed. Retry?", "Test failed")) TestCharging();
                return IsPassed;
            }
            _IQuido.Set(16, true);
            Thread.Sleep(1000);

            WritePWR(65535, 65535);

            Thread.Sleep(1000);
            BLEMessageReceived = false;

            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            bool chargeInProgressOK = false;
            bool chargeCompleteOK = false;

            while (stopwatch.ElapsedMilliseconds < 180000)
            {
                while (!BLEMessageReceived)
                {
                    Thread.Sleep(100);
                }
                if (BoardStatus == 'C')
                {
                    Console.WriteLine("BS charging");
                    chargeInProgressOK = true;
                    break;
                }
                else
                {
                    BLEMessageReceived = false;
                    Console.WriteLine("Packet Received " + BoardStatus.ToString());
                }
            }

            stopwatch.Restart();
            BLEMessageReceived = false;

            while (stopwatch.ElapsedMilliseconds < 180000)
            {
                while (!BLEMessageReceived)
                {
                    Thread.Sleep(100);
                }
                if (BoardStatus == 'D')
                {
                    Console.WriteLine("BS charge complete");
                    chargeCompleteOK = true;
                    break;
                }
                else
                {
                    BLEMessageReceived = false;
                    Console.WriteLine("Packet Received " + BoardStatus.ToString());
                }
            }

            Unpair();

            _IQuido.SetAllOff();
            _IManson.TestVOLT(42);

            if (chargeInProgressOK && chargeCompleteOK) Console.WriteLine("ChargeTestOK");
            else IsError = true;
            IsPassed = !IsError;
            EndTest();
            if (!IsPassed && _IUserDialogService.ConfirmInformation("> " + TestName + " < failed. Retry?", "Test failed")) TestCharging();
            return IsPassed;
        }
    }
}