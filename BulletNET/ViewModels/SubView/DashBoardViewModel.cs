using BulletNET.Database.Repositories.Interfaces;
using BulletNET.EntityFramework.Entities.Radar;
using BulletNET.EntityFramework.Entities.Users;
using BulletNET.Infrastructure.Commands;
using BulletNET.Services.BarcodeReader.Interface;
using BulletNET.Services.Devices.BluetoothDevice.Interface;
using BulletNET.Services.Devices.MansonDevice.Interface;
using BulletNET.Services.Devices.PicoDevices.Interface;
using BulletNET.Services.Devices.QuidoDevice.Interfaces;
using BulletNET.Services.Managers;
using BulletNET.Services.SequenceReaderService.Interfaces;
using BulletNET.Services.UserDialogService.Interfaces;
using BulletNET.ViewModels.Base;
using LiveCharts;
using LiveCharts.Wpf;
using MySqlConnector;
using Pallet.Services.UserDialogService.Interfaces;

namespace BulletNET.ViewModels.SubView
{
    internal class DashBoardViewModel : ViewModel
    {
        #region Services

        private readonly IDbRepository<RadarBoard> _IRepositoryRadarBoards;
        private readonly IDbRepository<TestAction> _IRepositoryTestActions;
        private readonly IDbRepository<TestGroup> _IRepositoryTestGroups;

        private readonly IBarcodeCRC _IBarcodeCRC;
        private readonly ISequenceReader _ISequenceReader;
        private readonly IQuido _IQuido;
        private readonly IPico _IPico;
        private readonly IManson _IManson;
        private readonly IBluetooth _IBluetooth;
        private readonly IOpenFileDialogService _IOpenFileDialogService;
        private readonly IUserDialogService _IUserDialogService;
        private readonly IManagerUser _IManagerUser;

        #endregion Services

        #region Properties (View)

        #region Serial number board

        private string _SerialNumberBoard;

        public string SerialNumberBoard
        {
            get => _SerialNumberBoard;
            set
            {
                Set(ref _SerialNumberBoard, value);
                if (long.TryParse(value, out long l) && _IBarcodeCRC.IsAllCRCOk(l) && l != 0) IsBoardFocused = false;
                else return;

                if (long.TryParse(SerialNumberRadar, out long e) && _IBarcodeCRC.IsAllCRCOk(e) && e != 0)
                {
                    if (IsAutoStart) StartSequence();
                }
                else IsRadarFocused = true;
            }
        }

        #endregion Serial number board

        #region Serial number radar

        private string _SerialNumberRadar;

        public string SerialNumberRadar
        {
            get => _SerialNumberRadar;
            set
            {
                Set(ref _SerialNumberRadar, value);
                if (long.TryParse(value, out long s) && _IBarcodeCRC.IsAllCRCOk(s) && s != 0) IsRadarFocused = false;
                else return;

                if (long.TryParse(SerialNumberBoard, out long e) && _IBarcodeCRC.IsAllCRCOk(e) && e != 0)
                {
                    if (IsAutoStart) StartSequence();
                }
                else IsBoardFocused = true;
            }
        }

        #endregion Serial number radar

        #region Is Autostart

        private bool _IsAutoStart;

        public bool IsAutoStart
        {
            get => _IsAutoStart;
            set => Set(ref _IsAutoStart, value);
        }

        #endregion Is Autostart

        #region IS without database

        private bool _IsWithoutDatabase;

        public bool IsWithoutDatabase
        {
            get => _IsWithoutDatabase;
            set => Set(ref _IsWithoutDatabase, value);
        }

        #endregion IS without database

        #region TestGroups

        private TestGroup _TestGroupSelected;

        public TestGroup TestGroupSelected
        {
            get => _TestGroupSelected;
            set
            {
                if (Set(ref _TestGroupSelected, value))
                    _TestActions.AddClear(value?.TestActions);
            }
        }

        public ICollectionView TestGroupsView => _TestGroupViewSource.View;

        private readonly CollectionViewSource _TestGroupViewSource;

        private ObservableCollection<TestGroup> TestGroups
        {
            get => _TestGroups;
            set => Set(ref _TestGroups, value);
        }

        private ObservableCollection<TestGroup> _TestGroups;

        #endregion TestGroups

        #region TestActions

        private TestAction _TestActionSelected;

        public TestAction TestActionSelected
        {
            get => _TestActionSelected;
            set => Set(ref _TestActionSelected, value);
        }

        public ICollectionView TestActionsView => _TestActionViewSource.View;

        private readonly CollectionViewSource _TestActionViewSource;

        private ObservableCollection<TestAction> _TestActions;

        #endregion TestActions

        #region IsPicoConnected

        private bool _IsPicoConnected;

        public bool IsPicoConnected
        {
            get => _IsPicoConnected;
            set => Set(ref _IsPicoConnected, value);
        }

        #endregion IsPicoConnected

        #region IsMansonConnected

        private bool _IsMansonConnected;

        public bool IsMansonConnected
        {
            get => _IsMansonConnected;
            set => Set(ref _IsMansonConnected, value);
        }

        #endregion IsMansonConnected

        #region IsQuidoConnected

        private bool _IsQuidoConnected;

        public bool IsQuidoConnected
        {
            get => _IsQuidoConnected;
            set => Set(ref _IsQuidoConnected, value);
        }

        #endregion IsQuidoConnected

        #region IsBluetoothConnected

        private bool _IsBluetoothConnected;

        public bool IsBluetoothConnected
        {
            get => _IsBluetoothConnected;
            set => Set(ref _IsBluetoothConnected, value);
        }

        #endregion IsBluetoothConnected

        #region Series Collection

        private SeriesCollection _SeriesCollection;

        public SeriesCollection SeriesCollection
        {
            get => _SeriesCollection;
            set => Set(ref _SeriesCollection, value);
        }

        #endregion Series Collection

        #region Is enabled MainBoard input

        public bool MainBoardIsEnabled => !IsTestStarted;

        #endregion Is enabled MainBoard input

        #region Is enabled Radar board input

        public bool RadarBoardIsEnabled => !IsTestStarted;

        #endregion Is enabled Radar board input

        private bool _IsTestStarted;

        public bool IsTestStarted
        {
            get => _IsTestStarted;
            set => Set(ref _IsTestStarted, value);
        }

        #region Is enabled Button History

        public bool BtnHistoryIsEnabled => !IsTestStarted &&
            long.TryParse(SerialNumberBoard, out long bsn) && _IBarcodeCRC.IsAllCRCOk(bsn)
            && long.TryParse(SerialNumberRadar, out long rsn) && _IBarcodeCRC.IsAllCRCOk(rsn)
            && _TestActions.Any();

        #endregion Is enabled Button History

        #region IsRadar textbox Focused

        private bool _IsRadarFocused;

        public bool IsRadarFocused
        {
            get => _IsRadarFocused;
            set => Set(ref _IsRadarFocused, value);
        }

        #endregion IsRadar textbox Focused

        #region Is Board textbox focused

        private bool _IsBoardFocused;

        public bool IsBoardFocused
        {
            get => _IsBoardFocused;
            set => Set(ref _IsBoardFocused, value);
        }

        #endregion Is Board textbox focused

        #endregion Properties (View)

        public DashBoardViewModel(
            IDbRepository<RadarBoard> IRepositoryRadarBoards,
            IDbRepository<TestAction> IRepositoryTestActions,
            IDbRepository<TestGroup> IRepositoryTestGroups,
            ISequenceReader ISequenceReader,
            IBarcodeCRC IBarcodeCRC,
            IQuido IQuido,
            IPico IPico,
            IManson IManson,
            IBluetooth IBluetooth,
            IOpenFileDialogService IOpenFileDialogService,
            IManagerUser IManagerUser,
            IUserDialogService IUserDialogService
            )
        {
            _IRepositoryRadarBoards = IRepositoryRadarBoards;
            _IRepositoryTestActions = IRepositoryTestActions;
            _IRepositoryTestGroups = IRepositoryTestGroups;
            _ISequenceReader = ISequenceReader;
            _IBarcodeCRC = IBarcodeCRC;
            _IQuido = IQuido;
            _IPico = IPico;
            _IManson = IManson;
            _IBluetooth = IBluetooth;
            _IOpenFileDialogService = IOpenFileDialogService;
            _IUserDialogService = IUserDialogService;
            _IManagerUser = IManagerUser;

            TestGroups = new();

            _TestGroupViewSource = new()
            {
                Source = TestGroups
            };
            TestGroups.CollectionChanged += TestGroups_CollectionChanged;

            _TestActions = new();

            _TestActionViewSource = new()
            {
                Source = _TestActions
            };
            _TestActions.CollectionChanged += _TestActions_CollectionChanged;

            new Thread(() =>
            {
                _IBluetooth.StartListening();
                _IPico.Connect();
                _IManson.Start();
                _IQuido.Start();
                IsBluetoothConnected = _IBluetooth.isEnabled;
                IsMansonConnected = _IManson.isEnabled;
                IsQuidoConnected = _IQuido.isEnabled;
                IsPicoConnected = _IPico.isEnabled;
            }).Start();
        }

        private void _TestActions_CollectionChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e) => TestActionsView.Refresh();

        private void TestGroups_CollectionChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e) => TestGroupsView.Refresh();

        #region Commands

        #region StartSequenceCommand

        private ICommand _StartSequenceCommand;

        /// <summary>
        /// Default command.
        /// </summary>
        public ICommand StartSequenceCommand => _StartSequenceCommand ??= new LambdaCommand(OnStartSequenceCommandExecuted, CanStartSequenceCommandExecute);

        /// <summary>
        /// Can execute default command .
        /// </summary>
        /// <param name="arg">The arg.</param>
        /// <returns>A bool.</returns>
        private bool CanStartSequenceCommandExecute(object arg) => !IsTestStarted &&
            long.TryParse(SerialNumberBoard, out long bsn) && _IBarcodeCRC.IsAllCRCOk(bsn)
            && long.TryParse(SerialNumberRadar, out long rsn) && _IBarcodeCRC.IsAllCRCOk(rsn)
            && _TestGroups.Any();

        /// <summary>
        /// Default function.
        /// </summary>
        /// <param name="obj">The obj.</param>
        private void OnStartSequenceCommandExecuted(object obj) => StartSequence();

        #endregion StartSequenceCommand

        #region DebugCommand

        private ICommand _DebugCommand;

        /// <summary>
        /// Default command.
        /// </summary>
        public ICommand DebugCommand => _DebugCommand ??= new LambdaCommand(OnDebugCommandExecuted, CanDebugCommandExecute);

        /// <summary>
        /// Can execute default command .
        /// </summary>
        /// <param name="arg">The arg.</param>
        /// <returns>A bool.</returns>
        private bool CanDebugCommandExecute(object arg) => true;

        /// <summary>
        /// Default function.
        /// </summary>
        /// <param name="obj">The obj.</param>
        private void OnDebugCommandExecuted(object obj)
        {
            //if (debugWindow == null)
            //{
            //    debugWindow = new DebugWindow();
            //}
            //debugWindow.Show();
        }

        #endregion DebugCommand

        #region PicoConnectCommand

        private ICommand _PicoConnectCommand;

        /// <summary>
        /// Default command.
        /// </summary>
        public ICommand PicoConnectCommand => _PicoConnectCommand ??= new LambdaCommand(OnPicoConnectCommandExecuted, CanPicoConnectCommandExecute);

        /// <summary>
        /// Can execute default command .
        /// </summary>
        /// <param name="arg">The arg.</param>
        /// <returns>A bool.</returns>
        private bool CanPicoConnectCommandExecute(object arg) => true;

        /// <summary>
        /// Default function.
        /// </summary>
        /// <param name="obj">The obj.</param>
        private void OnPicoConnectCommandExecuted(object obj)
        {
            _IPico.Connect();

            IsPicoConnected = _IPico.isEnabled;
        }

        #endregion PicoConnectCommand

        #region MansonConnectCommand

        private ICommand _MansonConnectCommand;

        /// <summary>
        /// Default command.
        /// </summary>
        public ICommand MansonConnectCommand => _MansonConnectCommand ??= new LambdaCommand(OnMansonConnectCommandExecuted, CanMansonConnectCommandExecute);

        /// <summary>
        /// Can execute default command .
        /// </summary>
        /// <param name="arg">The arg.</param>
        /// <returns>A bool.</returns>
        private bool CanMansonConnectCommandExecute(object arg) => true;

        /// <summary>
        /// Default function.
        /// </summary>
        /// <param name="obj">The obj.</param>
        private void OnMansonConnectCommandExecuted(object obj)
        {
            _IManson.Start();
            IsMansonConnected = _IManson.isEnabled;
        }

        #endregion MansonConnectCommand

        #region QuidoConnectCommand

        private ICommand _QuidoConnectCommand;

        /// <summary>
        /// Default command.
        /// </summary>
        public ICommand QuidoConnectCommand => _QuidoConnectCommand ??= new LambdaCommand(OnQuidoConnectCommandExecuted, CanQuidoConnectCommandExecute);

        /// <summary>
        /// Can execute default command .
        /// </summary>
        /// <param name="arg">The arg.</param>
        /// <returns>A bool.</returns>
        private bool CanQuidoConnectCommandExecute(object arg) => true;

        /// <summary>
        /// Default function.
        /// </summary>
        /// <param name="obj">The obj.</param>
        private void OnQuidoConnectCommandExecuted(object obj)
        {
            _IQuido.Start();
            IsQuidoConnected = _IQuido.isEnabled;
        }

        #endregion QuidoConnectCommand

        #region BluetoothConnectCommand

        private ICommand _BluetoothConnectCommand;

        /// <summary>
        /// Default command.
        /// </summary>
        public ICommand BluetoothConnectCommand => _BluetoothConnectCommand ??= new LambdaCommand(OnBluetoothConnectCommandExecuted, CanBluetoothConnectCommandExecute);

        /// <summary>
        /// Can execute default command .
        /// </summary>
        /// <param name="arg">The arg.</param>
        /// <returns>A bool.</returns>
        private bool CanBluetoothConnectCommandExecute(object arg) => true;

        /// <summary>
        /// Default function.
        /// </summary>
        /// <param name="obj">The obj.</param>
        private void OnBluetoothConnectCommandExecuted(object obj)
        {
            _IBluetooth.StartListening();
            IsBluetoothConnected = _IBluetooth.isEnabled;
        }

        #endregion BluetoothConnectCommand

        #region ReadSequenceFileCommand

        private ICommand _ReadSequenceFileCommand;

        /// <summary>
        /// Default command.
        /// </summary>
        public ICommand ReadSequenceFileCommand => _ReadSequenceFileCommand ??= new LambdaCommand(OnReadSequenceFileCommandExecuted, CanReadSequenceFileCommandExecute);

        /// <summary>
        /// Can execute default command .
        /// </summary>
        /// <param name="arg">The arg.</param>
        /// <returns>A bool.</returns>
        private bool CanReadSequenceFileCommandExecute(object arg) => true;

        /// <summary>
        /// Default function.
        /// </summary>
        /// <param name="obj">The obj.</param>
        private void OnReadSequenceFileCommandExecuted(object obj) => ReadSequenceFile();

        #endregion ReadSequenceFileCommand

        #region ChangeDBCommand

        private ICommand _ChangeDBCommand;

        /// <summary>
        /// Default command.
        /// </summary>
        public ICommand ChangeDBCommand => _ChangeDBCommand ??= new LambdaCommand(OnChangeDBCommandExecuted, CanChangeDBCommandExecute);

        /// <summary>
        /// Can execute default command .
        /// </summary>
        /// <param name="arg">The arg.</param>
        /// <returns>A bool.</returns>
        private bool CanChangeDBCommandExecute(object arg) => true;

        /// <summary>
        /// Default function.
        /// </summary>
        /// <param name="obj">The obj.</param>
        private void OnChangeDBCommandExecuted(object obj)
        {
            List<RadarBoard> RadarBoardList = new();
            List<TestGroup> TestGroupList = new();
            List<TestAction> TestActionList = new();
            string _connStr = @"server=192.168.123.64;user=root;database=BulletSeekerTest;port=3306;password=Pa44word;SslMode=none";
            MySqlConnection _conn;
            _conn = new MySqlConnection(_connStr);

            _conn.Open();

            MySqlCommand cmdGroup = new()
            {
                Connection = _conn
            };

            cmdGroup.CommandText = "SELECT mainBoardID,radarBoardID,datetime FROM MainRadarBoardPairing";
            MySqlDataReader rdrGroup = cmdGroup.ExecuteReader();
            while (rdrGroup.Read())
            {
                string mainboard = rdrGroup[0].ToString();
                string radarboard = rdrGroup[1].ToString();
                DateTime.TryParse((rdrGroup[2]).ToString(), out DateTime datetime);

                RadarBoardList.Add(new()
                {
                    MainBoardID = mainboard,
                    RadarBoardID = radarboard,
                });
            }
            rdrGroup.Close();
            cmdGroup.CommandText = "SELECT name, datetime, boardSN, id  FROM TestGroup";
            rdrGroup = cmdGroup.ExecuteReader();

            User tempUser = _IManagerUser.Users.First(s => s.Name == "simon.novotny");

            while (rdrGroup.Read())
            {
                string name = rdrGroup[0].ToString();
                DateTime.TryParse(rdrGroup[1].ToString(), out DateTime datetime);
                string board = rdrGroup[2].ToString();
                int.TryParse(rdrGroup[3].ToString(), out int intern);

                TestGroupList.Add(new()
                {
                    Name = name,
                    TimeStamp = datetime,
                    User = tempUser,
                    internalId = intern,
                    RadarBoard = RadarBoardList.Any(s => s.MainBoardID == board) ? RadarBoardList.First(s => s.MainBoardID == board) : null
                });
            }

            rdrGroup.Close();
            cmdGroup.CommandText = "SELECT measured,maximum, minimum, groupID, valueName, pass  FROM TestAction";
            rdrGroup = cmdGroup.ExecuteReader();

            while (rdrGroup.Read())
            {
                float.TryParse(rdrGroup[0].ToString(), out float measured);
                float.TryParse(rdrGroup[1].ToString(), out float maximum);
                float.TryParse(rdrGroup[2].ToString(), out float minimum);
                int.TryParse(rdrGroup[3].ToString(), out int groupID);
                string name = rdrGroup[4].ToString();
                bool.TryParse(rdrGroup[5].ToString(), out bool pass);

                TestActionList.Add(new()
                {
                    Name = name,
                    IsPassed = pass,
                    Maximum = maximum,
                    Minimum = minimum,
                    Measured = measured,
                    TestGroup = TestGroupList.Any(s => s.internalId == groupID) ? TestGroupList.First(s => s.internalId == groupID) : null,
                });
            }

            foreach (var testgroup in TestGroupList)
                testgroup.RadarBoard?.TestGroups.Add(testgroup);

            TestActionList.RemoveAll(s => s.TestGroup is null);
            TestGroupList.RemoveAll(s => s.RadarBoard is null || s.TestActions.Count == 0);
            TestGroupList.DistinctBy(s => s.TimeStamp);
            RadarBoardList.RemoveAll(s => s.TestGroups.Count == 0);

            try
            {
                _IRepositoryRadarBoards.AddIfNotExists(RadarBoardList);
            }
            catch (Exception e) { };
            try
            {
                _IRepositoryTestGroups.AddIfNotExists(TestGroupList);
            }
            catch (Exception e) { };

            foreach (var testAction in TestActionList)
                testAction.TestGroup?.TestActions.Add(testAction);
            try
            {
                _IRepositoryTestActions.AddIfNotExists(TestActionList);
            }
            catch (Exception e) { };
        }

        #endregion ChangeDBCommand

        #endregion Commands

        public void ReadSequenceFile()
        {
            string filepath = _IOpenFileDialogService.SelectFileGetPath().Result;
            (List<TestGroup> tmpqueue, string errorString) = _ISequenceReader.ReadSequenceFile(filepath);

            if (!string.IsNullOrEmpty(errorString))
                _IUserDialogService.ShowError("Chyba zpracování souboru instrukcí: " + errorString, "Error");

            TestGroups.AddClear(tmpqueue);
            foreach (TestGroup testgroup in TestGroups)
                foreach (TestAction testaction in testgroup.TestActions)
                    testaction.RefreshEvents += TestAction_RefreshEvents;
        }

        private void TestAction_RefreshEvents(object? sender, EventArgs e)
        {
            TestGroupSelected = ((TestAction)sender).TestGroup;
            TestActionSelected = (TestAction)sender;
            TestGroupsView.Refresh();
            TestActionsView.Refresh();
        }

        public void DrawChart(int[][] VoltageData)
        {
            SeriesCollection.Clear();

            var values = new ChartValues<int>();
            values.AddRange(VoltageData);

            SeriesCollection.Add(
                new LineSeries()
                {
                    Title = "Voltage PicoScope",
                    Values = values
                });
        }

        private void StartSequence()
        {
            if (string.IsNullOrEmpty(SerialNumberBoard) || string.IsNullOrEmpty(SerialNumberRadar))
            {
                _IUserDialogService.ShowWarning("Vyplňte sériové číslo", "");
                return;
            }

            if (!long.TryParse(SerialNumberBoard, out long sn_main))
            {
                _IUserDialogService.ShowWarning("Sériové číslo hlavní desky obsahuje nepovolený znak", "");
                return;
            }

            if (!long.TryParse(SerialNumberRadar, out long sn_radar))
            {
                _IUserDialogService.ShowWarning("Sériové číslo radaru obsahuje nepovolený znak", "");
                return;
            }

            if (!_IBarcodeCRC.IsAllCRCOk(sn_main) || !_IBarcodeCRC.IsAllCRCOk(sn_radar))
            {
                _IUserDialogService.ShowWarning("Špatný formát sériového čísla", "");
                return;
            }

            _IsTestStarted = true;
            TestGroupSelected = TestGroups[0];

            new Thread(() =>
            {
                foreach (TestGroup testGroup in TestGroups)
                {
                    testGroup.IsPassed = null;
                    foreach (TestAction testaction in testGroup.TestActions) testaction.IsPassed = null;
                }

                foreach (TestGroup testGroup in TestGroups)
                {
                    testGroup.OnTestEvents();

                    if (!IsWithoutDatabase)
                    {
                        TestGroup? test = (TestGroup)testGroup.Clone();
                        test.TimeStamp = DateTime.Now;
                        test.User = _IManagerUser.LoginedUser;

                        if (!_IRepositoryRadarBoards.Items.Any(s => s.RadarBoardID == SerialNumberRadar && s.MainBoardID == SerialNumberBoard))
                        {
                            _IRepositoryRadarBoards.Add(new RadarBoard()
                            {
                                RadarBoardID = SerialNumberRadar,
                                MainBoardID = SerialNumberBoard
                            });
                        }

                        test.RadarBoard = _IRepositoryRadarBoards.Items.First(s => s.RadarBoardID == SerialNumberRadar && s.MainBoardID == SerialNumberBoard);

                        _IRepositoryTestGroups.Add(test);
                        _IRepositoryTestActions.Add(test.TestActions);
                    }

                    if ((testGroup.IsPassed is null || !(bool)testGroup.IsPassed) && !_IUserDialogService.ConfirmInformation("Test obsahuje neplatné akce, pokračovat?", "Chyba")) break;
                }

                _IsTestStarted = false;
                SerialNumberBoard = "";
                SerialNumberRadar = "";
            }).Start();
        }
    }
}