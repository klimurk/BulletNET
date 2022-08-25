using BulletNET.Services.Devices.Base;
using BulletNET.Services.Devices.MansonDevice.Interface;
using Microsoft.Win32;
using Pallet.Services.UserDialogService.Interfaces;
using System.IO.Ports;
using System.Text.RegularExpressions;

namespace BulletNET.Services.Devices.MansonDevice
{
    public class Manson : Test, IManson
    {
        private readonly IUserDialogService _IUserDialogService;
        private readonly string __MANSONVID = "10C4";
        private readonly string __MANSONPID = "EA60";

        //public
        public int VOLTAGE { get; private set; }

        public int CURRENT { get; private set; }
        public int C_VOLTAGE { get; private set; }
        public int C_CURRENT { get; private set; }
        public bool STATUS { get; private set; }
        public bool isEnabled { get;  set; }

        //
        private readonly SerialPort SP_Manson = new SerialPort();

        private Thread COLLECTOR;

        //
        private int CMD = 0;

        private bool OK;

        //SP_Manson
        private string SP_Manson_PortName = "";

        private readonly int SP_Manson_BaudRate = 9600;
        private readonly int SP_Manson_DataBits = 8;
        private readonly StopBits SP_Manson_StopBits = (StopBits)Enum.Parse(typeof(StopBits), "One");
        private readonly Parity SP_Manson_Parity = (Parity)Enum.Parse(typeof(Parity), "None");

        public Manson(IUserDialogService IUserDialogService)
        {
            _IUserDialogService = IUserDialogService;
            Regex _rx = new(string.Format($"^VID_{__MANSONVID}.PID_{__MANSONPID}"), RegexOptions.IgnoreCase);
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
                            SP_Manson_PortName = (string)rk6.GetValue("PortName");

                            if (!string.IsNullOrEmpty(SP_Manson_PortName))
                            {
                                return;
                            }
                        }
                    }
                }
            }
            if (string.IsNullOrEmpty(SP_Manson_PortName))
                _IUserDialogService.ShowWarning(
                    "Manson device wasn't finded. Please check Manson driver has been installed and restart Application.( https://www.manson.com.hk/product/hcs-3204/ )",
                    "Manson");
        }

        public void Start()
        {
            Connect();
            if (!isEnabled || !SP_Manson.IsOpen)
            {
                isEnabled = false;
                return;
            }
            COLLECTOR ??= new Thread(Collector);
            if (!COLLECTOR.IsAlive) COLLECTOR.Start();
        }

        private bool Connect()
        {
            if (SP_Manson.IsOpen) return true;
            SP_Manson.PortName = SP_Manson_PortName;
            SP_Manson.BaudRate = SP_Manson_BaudRate;
            SP_Manson.DataBits = SP_Manson_DataBits;
            SP_Manson.StopBits = SP_Manson_StopBits;
            SP_Manson.Parity = SP_Manson_Parity;

            try
            {
                SP_Manson.Open();
                isEnabled = true;
                Console.WriteLine("Manson.Connection: SP_Manson connected");
                return true;
            }
            catch
            {
                isEnabled = false;
                Console.WriteLine("Manson.Connection: SP_Manson not connected");
                return false;
            }
        }

        private void Collector()
        {
            byte[] Line = new byte[100];
            int LineSize = 0;
            bool CR = false;

            try
            {
                while (true)
                {
                    if (SP_Manson.IsOpen && SP_Manson.BytesToRead > 0)
                    {
                        int BtR = SP_Manson.BytesToRead;
                        byte[] ANSW = new byte[BtR];
                        SP_Manson.Read(ANSW, 0, BtR);

                        //Find line
                        for (int x = 0; x < BtR; x++)
                        {
                            if (ANSW[x] == 13)
                            {
                                CR = true;
                            }

                            if (LineSize < 99)
                            {
                                Line[LineSize] = ANSW[x];
                                LineSize++;
                            }
                            else
                            {
                                Line[0] = ANSW[x];
                                LineSize = 1;
                            }

                            if (CR)
                            {
                                Interpreter(Line, LineSize);
                                CR = false;
                                LineSize = 0;
                            }
                        }
                    }
                    else
                    {
                        Thread.Sleep(10);
                    }
                }
            }
            catch
            {
                Console.WriteLine("Manson.Collector: ERROR!");
            }
        }

        private bool Send(byte[] CMD)
        {
            byte[] CMDpf = new byte[CMD.Length + 1];

            for (int x = 0; x < CMD.Length; x++)
            {
                CMDpf[x] = CMD[x];
            }
            CMDpf[CMDpf.Length - 1] = 13;
            if (!SP_Manson.IsOpen)
            {
                Console.WriteLine("Manson.Send: command cannot be sent, power supply is not connected");
                IsError = true;
                return false;
            }

            try
            {
                OK = false;
                SP_Manson.Write(CMDpf, 0, CMDpf.Length);
                while (!OK)
                {
                    Thread.Sleep(1);
                }
                IsError = false;
                return true;
            }
            catch
            {
                Console.WriteLine("Manson.Send: sending command to power supply failed");
                IsError = true;
                return false;
            }
        }

        private void Interpreter(byte[] Message, int Length)
        {
            if (CMD == 1 && Length == 7)
            {
                VOLTAGE = int.Parse(Encoding.ASCII.GetString(Message, 0, 3));
                CURRENT = int.Parse(Encoding.ASCII.GetString(Message, 3, 3));
            }

            if (CMD == 2 && Length == 10)
            {
                C_VOLTAGE = int.Parse(Encoding.ASCII.GetString(Message, 0, 4));
                C_CURRENT = int.Parse(Encoding.ASCII.GetString(Message, 4, 4));
                STATUS = Message[8] == 1;
            }

            if (Length == 3 && Message[0] == 79 && Message[1] == 75)
            {
                OK = true;
            }
        }

        //********************************************************************************************************************************************************
        //FUNKCE
        //********************************************************************************************************************************************************

        public bool GETS()
        {
            byte[] _GETS = Encoding.ASCII.GetBytes("GETS");
            CMD = 1;
            return Send(_GETS);
        }

        public bool GETD()
        {
            byte[] _GETD = Encoding.ASCII.GetBytes("GETD");
            CMD = 2;
            return Send(_GETD);
        }

        //********************************************************************************************************************************************************
        //TESTS
        //********************************************************************************************************************************************************
        public bool TestVOLT(int Value)
        {
            StartTest("Set voltage");
            byte[] _VOLT;
            CMD = 0;

            if (Value < 10 || Value > 600)
            {
                Console.WriteLine("Wrong value");
                IsError = true;
                IsPassed = !IsError;
                EndTest();
                if (!IsPassed && _IUserDialogService.ConfirmInformation("> " + TestName + " < failed. Retry?", "Test failed")) TestVOLT(Value);
                return IsPassed;
            }

            if (Value < 10)
            {
                _VOLT = Encoding.ASCII.GetBytes("VOLT00" + Value.ToString());
            }
            else if (Value < 100)
            {
                _VOLT = Encoding.ASCII.GetBytes("VOLT0" + Value.ToString());
            }
            else
            {
                _VOLT = Encoding.ASCII.GetBytes("VOLT" + Value.ToString());
            }

            Send(_VOLT);
            IsPassed = !IsError;
            EndTest();
            if (!IsPassed && _IUserDialogService.ConfirmInformation("> " + TestName + " < failed. Retry?", "Test failed")) TestVOLT(Value);
            return IsPassed;
        }

        public bool TestCURR(int Value)
        {
            StartTest("Set current");
            byte[] _CURR;
            CMD = 0;

            if (Value < 0 || Value > 50)
            {
                Console.WriteLine("Wrong value");
                IsError = true;
                IsPassed = !IsError;
                EndTest();
                if (!IsPassed && _IUserDialogService.ConfirmInformation("> " + TestName + " < failed. Retry?", "Test failed")) TestCURR(Value);
                return IsPassed;
            }

            if (Value < 10)
            {
                _CURR = Encoding.ASCII.GetBytes("CURR00" + Value.ToString());
            }
            else if (Value < 100)
            {
                _CURR = Encoding.ASCII.GetBytes("CURR0" + Value.ToString());
            }
            else
            {
                _CURR = Encoding.ASCII.GetBytes("CURR" + Value.ToString());
            }

            Send(_CURR);

            IsPassed = !IsError;
            EndTest();
            if (!IsPassed && _IUserDialogService.ConfirmInformation("> " + TestName + " < failed. Retry?", "Test failed")) TestCURR(Value);
            return IsPassed;
        }

        public bool MeasureCurrent(double minimum, double maximum, string testName)
        {
            StartTest(testName);
            GETD();
            Measured = C_CURRENT;
            IsPassed = Measured >= minimum && Measured <= maximum;
            EndTest();
            if (!IsPassed && _IUserDialogService.ConfirmInformation("> " + TestName + " < failed. Retry?", "Test failed")) MeasureCurrent(minimum, maximum, testName);
            return IsPassed;
        }
    }
}