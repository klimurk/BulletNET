using BulletNET.EntityFramework.Entities.Radar;
using BulletNET.Services.DelayService;
using BulletNET.Services.Devices.BluetoothDevice.Interface;
using BulletNET.Services.Devices.MansonDevice.Interface;
using BulletNET.Services.Devices.PicoDevices.Interface;
using BulletNET.Services.Devices.QuidoDevice.Interfaces;
using BulletNET.Services.SequenceReaderService.Interfaces;
using BulletNET.ViewModels.SubView;

namespace BulletNET.Services.SequenceReaderService
{
    public class SequenceReader : ISequenceReader
    {
        private readonly IManson _IManson;
        private readonly IQuido _IQuido;
        private readonly IPico _IPico;
        private readonly IBluetooth _IBluetooth;
        private readonly IDelay _IDelay;

        public SequenceReader(
            IManson IManson,
            IQuido IQuido,
            IPico IPico,
            IBluetooth IBluetooth,
            IDelay IDelay
            )
        {
            _IManson = IManson;
            _IQuido = IQuido;
            _IPico = IPico;
            _IBluetooth = IBluetooth;
            _IDelay = IDelay;
        }

        public (List<TestGroup> testGroupQueue, string errorString) ReadSequenceFile(string filepath)
        {
            //System.IO.StreamReader file = new(@".\Resources\sequence.txt");
            System.IO.StreamReader file = new(filepath);
            string currentLine;
            string errorString = null;

            List<TestGroup> TestGroupQueue = new();

            TestGroup currentTestGroup = new() { Name = "null" };

            while ((currentLine = file.ReadLine()) != null)
            {
                string[] words = currentLine.Split(' ');
                string testname;
                if (words[0].Contains("//") || words[0] == "")
                {
                    continue;
                }

                if (currentTestGroup == null && words[0] != "NewGroup")
                {
                    errorString = "První příkaz musí být NewResult";
                }

                switch (words[0])
                {
                    case "NewGroup":
                        string testGroupName = words[1];
                        currentTestGroup = new TestGroup() { Name = testGroupName };
                        TestGroupQueue.Add(currentTestGroup);
                        break;

                    case "delay":
                        if (int.TryParse(words[1], out int ms))
                        {
                            currentTestGroup.TestEvents += new EventHandler((object o, EventArgs a) => _IDelay.MakeDelay(ms));
                        }
                        else
                        {
                            errorString = "Chyba čtení hodnoty";
                        }
                        break;

                    case "SetVoltage":
                        if (int.TryParse(words[1], out int voltage))
                        {
                            currentTestGroup.TestEvents += new EventHandler((object o, EventArgs a) => _IManson.TestVOLT(voltage));
                        }
                        else
                        {
                            errorString = "Chyba čtení hodnoty napětí";
                        }
                        break;

                    case "SetCurrent":
                        if (int.TryParse(words[1], out int current))
                        {
                            currentTestGroup.TestEvents += new EventHandler((object o, EventArgs a) => _IManson.TestCURR(current));
                        }
                        else
                        {
                            errorString = "Chyba čtení hodnoty proudu";
                        }
                        break;

                    case "SetRelay":
                        if (words[1] == "ALL")
                        {
                            if (words[2] == "OFF")
                            {
                                const byte relayID = 99;
                                const bool relayOn = false;

                                currentTestGroup.TestEvents += new EventHandler((object o, EventArgs a) => _IQuido.CommunicationTest(relayID, relayOn));
                            }
                            else
                            {
                                errorString = "Nerozpoznaný příkaz";
                                break;
                            }
                        }
                        else
                        {
                            bool relayOn;
                            if (words[2] == "ON")
                            {
                                relayOn = true;
                            }
                            else if (words[2] == "OFF")
                            {
                                relayOn = false;
                            }
                            else
                            {
                                errorString = "třetí slovo musí být ON/OFF";
                                break;
                            }

                            if (byte.TryParse(words[1], out byte relayID))
                            {
                                currentTestGroup.TestEvents += new EventHandler((object o, EventArgs a) => _IQuido.CommunicationTest(relayID, relayOn));
                            }
                            else
                            {
                                errorString = "Chyba čtení ID relay";
                            }
                        }

                        break;

                    case "TestVoltage":
                        if (float.TryParse(words[2], out float minv) && float.TryParse(words[3], out float maxv))
                        {
                            testname = "Test Voltage " + words[4];
                            currentTestGroup.TestActions.Add(
                                new TestAction()
                                {
                                    Name = testname,
                                    TestGroup = currentTestGroup,
                                    Maximum = maxv,
                                    Minimum = minv,
                                });

                            currentTestGroup.TestEvents += new EventHandler((object o, EventArgs a) =>
                            {
                                TestAction? test = ((TestGroup)o).TestActions.First(s => s.Name == testname && s.IsPassed == null);
                                test.IsRunning = true;
                                test.IsPassed = _IPico.MeasureVoltage(minv, maxv, words[4], words[1]);
                                test.Measured = _IPico.Measured;
                            });
                        }
                        else
                        {
                            errorString = "Chyba čtení minima/maxima";
                        }
                        break;

                    case "TestCurrent":
                        if (float.TryParse(words[1], out float minc) && float.TryParse(words[2], out float maxc))
                        {
                            testname = "Test Current " + words[3];
                            currentTestGroup.TestActions.Add(
                                new TestAction()
                                {
                                    Name = testname,
                                    TestGroup = currentTestGroup,
                                    Maximum = maxc,
                                    Minimum = minc,
                                });

                            currentTestGroup.TestEvents += new EventHandler((object o, EventArgs a) =>
                            {
                                TestAction? test = ((TestGroup)o).TestActions.First(s => s.Name == testname && s.IsPassed == null);
                                test.IsRunning = true;
                                test.IsPassed = _IManson.MeasureCurrent(minc, maxc, words[3]);
                                test.Measured = _IManson.Measured;
                            });
                        }
                        else
                        {
                            errorString = "Chyba čtení minima/maxima";
                        }
                        break;

                    case "TestFreq":
                        if (float.TryParse(words[2], out float minf) && float.TryParse(words[3], out float maxf))
                        {
                            testname = "Test Frequency";
                            currentTestGroup?.TestActions.Add(
                                new TestAction()
                                {
                                    Name = testname,
                                    TestGroup = currentTestGroup,
                                    Maximum = maxf,
                                    Minimum = minf,
                                });

                            currentTestGroup.TestEvents += new EventHandler((object o, EventArgs a) =>
                            {
                                TestAction? test = ((TestGroup)o).TestActions.First(s => s.Name == testname && s.IsPassed == null);
                                test.IsRunning = true;
                                test.IsPassed = _IPico.ReadFreq(minf, maxf, words[3]);
                                test.Measured = _IPico.Measured;
                            });
                        }
                        else
                        {
                            errorString = "Chyba čtení minima/maxima";
                        }
                        break;

                    case "Flash":

                        testname = "Flash firmware " + words[1];
                        currentTestGroup.TestActions.Add(
                        new TestAction()
                        {
                            Name = testname,
                            TestGroup = currentTestGroup
                        });

                        currentTestGroup.TestEvents += new EventHandler((object o, EventArgs a) =>
                        {
                            TestAction test = ((TestGroup)o).TestActions.First(s => s.Name == testname && s.IsPassed == null);
                            test.IsRunning = true;
                            test.IsPassed = _IQuido.Flash(words[1]);
                        });
                        break;

                    case "Erase":

                        testname = "Erase firmware";
                        currentTestGroup.TestActions.Add(
                            new TestAction()
                            {
                                Name = testname,
                                TestGroup = currentTestGroup
                            });

                        currentTestGroup.TestEvents += new EventHandler((object o, EventArgs a) =>
                        {
                            TestAction test = ((TestGroup)o).TestActions.First(s => s.Name == testname && s.IsPassed == null);
                            test.IsRunning = true;
                            test.IsPassed = _IQuido.EraseFirmware();
                        });
                        break;

                    case "ShowChart":
                        if (uint.TryParse(words[1], out uint samples))
                        {
                            DashBoardViewModel dashBoardViewModel = App.Services.GetService(typeof(DashBoardViewModel)) as DashBoardViewModel;
                            //dashBoardViewModel.DrawChart(_

                            //currentTestGroup.TestEvents += new EventHandler((object o, EventArgs a) =>  ShowChart(samples));
                        }
                        else
                        {
                            errorString = "Chyba čtení počtu vzorků";
                        }
                        break;

                    case "SigGen":
                        break;

                    case "Bluetooth":
                        switch (words[1])
                        {
                            case "Pair":
                                testname = "Bluetooth pair";
                                currentTestGroup.TestActions.Add(
                                    new TestAction()
                                    {
                                        Name = testname,
                                        TestGroup = currentTestGroup
                                    });

                                currentTestGroup.TestEvents += new EventHandler((object o, EventArgs a) =>
                                {
                                    TestAction test = ((TestGroup)o).TestActions.First(s => s.Name == testname && s.IsPassed == null);
                                    test.IsRunning = true;
                                    test.IsPassed = _IBluetooth.PairTest();
                                });
                                break;

                            case "TestMode":
                                testname = "Bluetooth testMode";
                                currentTestGroup.TestActions.Add(
                                    new TestAction()
                                    {
                                        Name = testname,
                                        TestGroup = currentTestGroup
                                    });

                                currentTestGroup.TestEvents += new EventHandler((object o, EventArgs a) =>
                                {
                                    TestAction test = ((TestGroup)o).TestActions.First(s => s.Name == testname && s.IsPassed == null);
                                    test.IsRunning = true;
                                    test.IsPassed = _IBluetooth.SwitchToTestMode();
                                });
                                break;

                            case "Unpair":
                                testname = "Bluetooth unpair";
                                currentTestGroup.TestActions.Add(
                                    new TestAction()
                                    {
                                        Name = testname,
                                        TestGroup = currentTestGroup
                                    });

                                currentTestGroup.TestEvents += new EventHandler((object o, EventArgs a) =>
                                {
                                    TestAction test = ((TestGroup)o).TestActions.First(s => s.Name == testname && s.IsPassed == null);
                                    test.IsRunning = true;
                                    test.IsPassed = _IBluetooth.Unpair().Result;
                                });
                                break;

                            case "CheckChargeInProgress":
                                testname = "Bluetooth Check Charge In Progress";
                                currentTestGroup.TestActions.Add(
                                    new TestAction()
                                    {
                                        Name = testname,
                                        TestGroup = currentTestGroup
                                    });

                                currentTestGroup.TestEvents += new EventHandler((object o, EventArgs a) =>
                                {
                                    TestAction test = ((TestGroup)o).TestActions.First(s => s.Name == testname && s.IsPassed == null);
                                    test.IsRunning = true;
                                    test.IsPassed = _IBluetooth.TestChargeInProgress();
                                });
                                break;

                            case "CheckChargeComplete":
                                testname = "Bluetooth Check Charge Complete";
                                currentTestGroup.TestActions.Add(
                                    new TestAction()
                                    {
                                        Name = testname,
                                        TestGroup = currentTestGroup
                                    });

                                currentTestGroup.TestEvents += new EventHandler((object o, EventArgs a) =>
                                {
                                    TestAction test = ((TestGroup)o).TestActions.First(s => s.Name == testname && s.IsPassed == null);
                                    test.IsRunning = true;
                                    test.IsPassed = _IBluetooth.TestFinishCharge();
                                });
                                break;

                            case "CheckCharging":
                                testname = "Bluetooth Check Charging";
                                currentTestGroup.TestActions.Add(
                                    new TestAction()
                                    {
                                        Name = testname,
                                        TestGroup = currentTestGroup
                                    });

                                currentTestGroup.TestEvents += new EventHandler((object o, EventArgs a) =>
                                {
                                    TestAction test = ((TestGroup)o).TestActions.First(s => s.Name == testname && s.IsPassed == null);
                                    test.IsRunning = true;
                                    test.IsPassed = _IBluetooth.TestCharging();
                                });
                                break;

                            default:
                                errorString = "Chyba čtení příkazu Bluetooth";
                                break;
                        }
                        break;
                }

                if (errorString != null)
                {
                    break;
                }
            }

            if (errorString != null)
            {
                file.Close();
                return (TestGroupQueue, "OK");
            }
            else
            {
                file.Close();
                return (TestGroupQueue, errorString);
            }
        }
    }
}