using BulletNET.Services.Devices.Base;
using BulletNET.Services.Devices.BluetoothDevice.Interface;
using BulletNET.Services.Devices.QuidoDevice.Interfaces;
using BulletNET.Services.Devices.QuidoDevice.QuidoSupport;
using Microsoft.Win32;
using Pallet.Services.UserDialogService.Interfaces;
using System.IO;
using System.IO.Ports;
using System.Text.RegularExpressions;

namespace BulletNET.Services.Devices.QuidoDevice
{
    public class Quido : Test, IQuido
    {
        #region Services

        private readonly IUserDialogService _IUserDialogService;

        #endregion Services

        //public
        public bool[] Relay = new bool[32];

        //
        private readonly SerialPort SP_Quido = new SerialPort();

        private SpinelCMD SpinelCMD = new();
        private Thread COLLECTOR;

        //
        private bool onWAY;

        //SP_Quido
        private readonly string SP_Quido_PortName = "";

        private readonly int SP_Quido_BaudRate = 115200;
        private readonly int SP_Quido_DataBits = 8;
        private readonly StopBits SP_Quido_StopBits = (StopBits)Enum.Parse(typeof(StopBits), "One");
        private readonly Parity SP_Quido_Parity = (Parity)Enum.Parse(typeof(Parity), "None");

        private readonly string _QuidoVID = "0403";
        private readonly string _QuidoPID = "6015";

        public Quido(IUserDialogService IUserDialogService)
        {
            _IUserDialogService = IUserDialogService;
            SP_Quido_PortName = "";

            Regex _rx = new Regex(string.Format("^VID_{0}.PID_{1}", _QuidoVID, _QuidoPID), RegexOptions.IgnoreCase);
            RegistryKey rk1 = Registry.LocalMachine;
            RegistryKey rk2 = rk1.OpenSubKey("SYSTEM\\CurrentControlSet\\Enum");
            foreach (string s3 in rk2.GetSubKeyNames())
            {
                RegistryKey rk3 = rk2.OpenSubKey(s3);
                foreach (string s in rk3.GetSubKeyNames())
                {
                    if (_rx.Match(s).Success)
                    {
                        RegistryKey rk4 = rk3.OpenSubKey(s);
                        foreach (string s2 in rk4.GetSubKeyNames())
                        {
                            RegistryKey rk5 = rk4.OpenSubKey(s2);
                            RegistryKey rk6 = rk5.OpenSubKey("Device Parameters");
                            SP_Quido_PortName = (string)rk6.GetValue("PortName");

                            if (!string.IsNullOrEmpty(SP_Quido_PortName))
                            {
                                return;
                            }
                        }
                    }
                }
            }
        }

        public void Start()
        {
            Connect();
            if (!isEnabled || !SP_Quido.IsOpen)
            {
                isEnabled = false;
                return;
            }
            COLLECTOR ??= new Thread(Collector);
            if (!COLLECTOR.IsAlive) COLLECTOR.Start();
        }

        private bool Connect()
        {
            if (SP_Quido.IsOpen) SP_Quido.Close();
            SP_Quido.PortName = SP_Quido_PortName;
            SP_Quido.BaudRate = SP_Quido_BaudRate;
            SP_Quido.DataBits = SP_Quido_DataBits;
            SP_Quido.StopBits = SP_Quido_StopBits;
            SP_Quido.Parity = SP_Quido_Parity;

            try
            {
                SP_Quido.Open();
                isEnabled = true;
                Console.WriteLine("Quido.Connection: SP_Quido connected");
                return true;
            }
            catch
            {
                isEnabled = false;
                Console.WriteLine("Quido.Connection: SP_Quido not connected");
                return false;
            }
        }

        private void Collector()
        {
            int err;
            int BtR1;
            int BtR2;

            try
            {
                while (true)
                {
                    if (SP_Quido.IsOpen)
                    {
                        if (onWAY)
                        {
                            while (SP_Quido.BytesToRead < 9)
                            {
                                Thread.Sleep(1);
                            }
                            BtR1 = SP_Quido.BytesToRead;
                            byte[] ANSW1 = new byte[BtR1];
                            SP_Quido.Read(ANSW1, 0, BtR1);
                            err = SpinelCMD.Auditor(ANSW1);

                            if (err == 4)
                            {
                                while (SP_Quido.BytesToRead < SpinelCMD.length - (BtR1 - 4))
                                {
                                    Thread.Sleep(1);
                                }
                                BtR2 = SP_Quido.BytesToRead;
                                byte[] ANSW2 = new byte[BtR2];
                                SP_Quido.Read(ANSW2, 0, BtR2);
                                byte[] ANSW12 = new byte[BtR1 + BtR2];
                                Buffer.BlockCopy(ANSW1, 0, ANSW12, 0, BtR1);
                                Buffer.BlockCopy(ANSW2, 0, ANSW12, BtR1, BtR2);
                                err = SpinelCMD.Auditor(ANSW12);
                            }
                            if (err != 0)
                            {
                                Console.WriteLine("Quido.Colector: ERROR: " + err);
                            }
                            onWAY = false;
                        }
                        else
                        {
                            if (SP_Quido.BytesToRead > 0)
                            {
                                BtR1 = SP_Quido.BytesToRead;
                                byte[] TRASH = new byte[BtR1];
                                SP_Quido.Read(TRASH, 0, BtR1);
                                Console.WriteLine("Quido.Colector: WARNING: unexpected data on bus: ");
                                foreach (byte B in TRASH)
                                {
                                    Console.Write(B + " ");
                                }
                            }
                        }
                        Thread.Sleep(10);
                    }
                }
            }
            catch
            {
                Console.WriteLine("Quido.Colector: ERROR!");
            }
        }

        //********************************************************************************************************************************************************
        //FUNKCE
        //********************************************************************************************************************************************************

        public void SetAllOff()
        {
            IsError = false;
            byte[] data = new byte[32];
            for (int x = 0; x < 32; x++)
            {
                data[x] = (byte)(x + 1);
            }

            SpinelCMD = new SpinelCMD(0x00, 0x00, 0x20, data);
            onWAY = true;
            SP_Quido.Write(SpinelCMD.CMD, 0, SpinelCMD.CMD.Length);

            while (onWAY)
            {
                Thread.Sleep(1);
            }

            Read();
            for (int x = 0; x < 32; x++)
            {
                if (Relay[x])
                {
                    IsError = true;
                }
            }
        }

        public bool Set(byte ADR, bool State)
        {
            Read();

            if ((ADR == 15 && State && (Relay[16] || Relay[17]))
                || (ADR == 17 && State && (Relay[14] || Relay[17]))
                || (ADR == 18 && State && (Relay[14] || Relay[16])))
            {
                return false;
            }

            byte[] data = new byte[1];
            data[0] = ADR;
            if (State) data[0] += 128;

            SpinelCMD = new SpinelCMD(0x00, 0x00, 0x20, data);
            onWAY = true;
            SP_Quido.Write(SpinelCMD.CMD, 0, SpinelCMD.CMD.Length);

            while (onWAY) Thread.Sleep(1);

            Read();
            return Relay[ADR - 1] == State;
        }

        public void Read()
        {
            byte[] data = Array.Empty<byte>();

            SpinelCMD = new SpinelCMD(0x00, 0x00, 0x30, data);
            onWAY = true;
            SP_Quido.Write(SpinelCMD.CMD, 0, SpinelCMD.CMD.Length);

            while (onWAY) Thread.Sleep(1);

            for (int x = 0; x < 4; x++)
            {
                uint mask = 1;
                for (int y = 0; y < 8; y++)
                {
                    Relay[(8 * x) + y] = (SpinelCMD.ANSWD[3 - x] & mask) > 0;

                    mask <<= 1;
                }
            }
        }

        //********************************************************************************************************************************************************
        //TESTS.ANSWD
        //********************************************************************************************************************************************************
        public bool CommunicationTest(byte relay, bool relayON)
        {
            StartTest(relay == 99 ? "Relay ALL OFF" : "Relay " + relay + (relayON ? " ON" : " OFF"));
            IsScheduled = true;

            if (relay == 99 && !relayON)
            {
                SetAllOff();
                if (IsError)
                {
                    _IUserDialogService.ShowError("Chyba komunikace s Quido", "Quido Test");
                    IsError = true;
                }
            }
            else
            {
                if (!Set(relay, relayON))
                {
                    _IUserDialogService.ShowError("Chyba komunikace s Quido", "Quido Test");
                    IsError = true;
                }
            }

            IsPassed = !IsError;
            EndTest();
            if (!IsPassed && _IUserDialogService.ConfirmInformation($"{TestName} failed. Retry?", "Test failed")) CommunicationTest(relay, relayON);
            return IsPassed;
        }

        public bool Flash(string firmwareName)
        {
            StartTest("Flash firmware " + firmwareName);

            Process STM32Process = null;
            var path = @"C:\Program Files\STMicroelectronics\STM32Cube\STM32CubeProgrammer\bin\STM32_Programmer_CLI.exe";
            if (File.Exists(path))
            {
                STM32Process = new()
                {
                    StartInfo =
                            {
                                CreateNoWindow = true,
                                UseShellExecute = false,
                                RedirectStandardOutput = true,
                                FileName = path,
                                Arguments = "-c port=SWD freq=4000 mode=normal -e all"
                            }
                };
            }
            var path86 = @"C:\Program Files(x86)\STMicroelectronics\STM32Cube\STM32CubeProgrammer\bin\STM32_Programmer_CLI.exe";
            if (File.Exists(path86))
            {
                STM32Process = new()
                {
                    StartInfo =
                            {
                                CreateNoWindow = true,
                                UseShellExecute = false,
                                RedirectStandardOutput = true,
                                FileName = path86,
                                Arguments = "-c port=SWD freq=4000 mode=normal -e all"
                            }
                };
            }
            if (STM32Process is null)
            {
                _IUserDialogService.ShowError("STM32_Programmer_CLI.exe has not been finded. Please install in C drive", "STM32 Programmer");
                return false;
            }
            for (int i = 0; i < 3 && IsPassed; i++)
            {
                SetAllOff();
                Thread.Sleep(2000);
                Set(15, true);
                Thread.Sleep(200);
                Set(14, true);
                Thread.Sleep(1000);

                STM32Process.Start();
                // Redirect the output stream of the child process.

                string output = STM32Process.StandardOutput.ReadToEnd();

                STM32Process.WaitForExit();

                if (output.Contains("Download verified successfully"))
                {
                    Console.WriteLine("Flash OK. Restart, wait for boot");

                    SetAllOff();
                    Thread.Sleep(2000);
                    Set(15, true);
                    Thread.Sleep(500);
                    Set(14, true);
                    Thread.Sleep(200);
                    Set(14, false);
                    Thread.Sleep(2000);

                    IBluetooth bluetooth = App.Services.GetService(typeof(IBluetooth)) as IBluetooth;
                    bluetooth.AdvertisementReceived = false;

                    Stopwatch stopwatch = new();
                    stopwatch.Start();

                    while (!bluetooth.AdvertisementReceived)
                    {
                        Thread.Sleep(100);
                        if (stopwatch.ElapsedMilliseconds > 20000)
                        {
                            Console.WriteLine("Wait for advertisement timeout");
                            break;
                        }
                    }

                    if (bluetooth.AdvertisementReceived)
                    {
                        Console.WriteLine("Boot OK");
                        IsPassed = true;
                        break;
                    }
                }
                else
                {
                    IsError = true;
                }
            }
            IsPassed = !IsError;
            EndTest();
            if (!IsPassed && _IUserDialogService.ConfirmInformation($"{TestName} failed. Retry?", "Test failed")) Flash(firmwareName);
            return IsPassed;
        }

        public bool EraseFirmware()
        {
            StartTest("Erase firmware");

            Process STM32Process = null;
            var path = @"C:\Program Files\STMicroelectronics\STM32Cube\STM32CubeProgrammer\bin\STM32_Programmer_CLI.exe";
            if (File.Exists(path))
            {
                STM32Process = new()
                {
                    StartInfo =
                            {
                                CreateNoWindow = true,
                                UseShellExecute = false,
                                RedirectStandardOutput = true,
                                FileName = path,
                                Arguments = "-c port=SWD freq=4000 mode=normal -e all"
                            }
                };
            }
            var path86 = @"C:\Program Files(x86)\STMicroelectronics\STM32Cube\STM32CubeProgrammer\bin\STM32_Programmer_CLI.exe";
            if (File.Exists(path86))
            {
                STM32Process = new()
                {
                    StartInfo =
                            {
                                CreateNoWindow = true,
                                UseShellExecute = false,
                                RedirectStandardOutput = true,
                                FileName = path86,
                                Arguments = "-c port=SWD freq=4000 mode=normal -e all"
                            }
                };
            }
            if (STM32Process is null)
            {
                _IUserDialogService.ShowError("STM32_Programmer_CLI.exe has not been finded. Please install in C drive", "STM32 Programmer");
                return false;
            }

            for (int i = 0; i < 3 && !IsPassed; i++)
            {
                SetAllOff();
                Thread.Sleep(2000);
                Set(15, true);
                Set(14, true);

                for (int j = 0; j < 10 && !IsPassed; j++)
                {
                    // Redirect the output stream of the child process.
                    STM32Process.Start();

                    string output = STM32Process.StandardOutput.ReadToEnd();
                    STM32Process.WaitForExit();

                    if (output.Contains("Mass erase successfully achieved"))
                    {
                        IsPassed = true;
                    }
                    else
                    {
                        IsError = true;
                    }
                }
            }
            EndTest();
            if (!IsPassed && _IUserDialogService.ConfirmInformation($"{TestName} failed. Retry?", "Test failed")) EraseFirmware();
            return IsPassed;
        }
    }
}