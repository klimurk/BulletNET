﻿using BulletNET.Database.Repositories.Interfaces;
using BulletNET.EntityFramework.Entities.Radar;
using BulletNET.Infrastructure.Commands;
using BulletNET.Services.Managers;
using BulletNET.ViewModels.Base;
using Pallet.Services.UserDialogService.Interfaces;

namespace BulletNET.ViewModels.SubView
{
    internal class HistoryViewModel : ViewModel
    {
        #region Services

        private readonly IDbRepository<RadarBoard> _IRepositoryRadarBoard;
        private readonly IDbRepository<TestGroup> _IRepositoryTestGroup;
        private readonly IDbRepository<TestAction> _IRepositoryTestAction;
        private readonly IManagerUser _IManagerUser;
        private readonly IUserDialogService _IUserDialogService;

        #endregion Services

        private readonly Uri currentImageUri = new("/Bullet;component/Resources/Images/currentSymbol.png", UriKind.Relative);
        private readonly Uri voltageImageUri = new("/Bullet;component/Resources/Images/voltageSymbol.png", UriKind.Relative);
        private readonly Uri firmwareImageUri = new("/Bullet;component/Resources/Images/firmwareSymbol.png", UriKind.Relative);
        private readonly Uri bluetoothImageUri = new("/Bullet;component/Resources/Images/bluetoothSymbol.png", UriKind.Relative);
        private readonly Uri frequencyImageUri = new("/Bullet;component/Resources/Images/frequencySymbol.png", UriKind.Relative);

        #region RadarBoards

        private RadarBoard _RadarBoardSelected;

        public RadarBoard RadarBoardSelected
        {
            get => _RadarBoardSelected;
            set
            {
                if (Set(ref _RadarBoardSelected, value))
                {
                    TestGroups.Clear();
                    TestGroups.Add(value?.TestGroups);
                }
            }
        }

        public ICollectionView RadarBoardsView => _RadarBoardViewSource.View;

        private CollectionViewSource _RadarBoardViewSource;

        private ObservableCollection<RadarBoard> RadarBoards
        {
            get => _RadarBoards;
            set => Set(ref _RadarBoards, value);
        }

        private ObservableCollection<RadarBoard> _RadarBoards;

        #endregion RadarBoards

        #region TestGroups

        private TestGroup _TestGroupSelected;

        public TestGroup TestGroupSelected
        {
            get => _TestGroupSelected;
            set
            {
                if (Set(ref _TestGroupSelected, value))
                {
                    _TestActions.Clear();
                    _TestActions.Add(value?.TestActions);
                }
            }
        }

        public ICollectionView TestGroupsView => _TestGroupViewSource.View;

        private CollectionViewSource _TestGroupViewSource;

        private ObservableCollection<TestGroup> TestGroups
        {
            get => _TestGroups;
            set => Set(ref _TestGroups, value);
        }

        private ObservableCollection<TestGroup> _TestGroups;

        private List<TestGroup> _TestGroupsOld;

        #endregion TestGroups

        #region TestActions

        public ICollectionView TestActionsView => _TestActionViewSource.View;

        private CollectionViewSource _TestActionViewSource;

        private ObservableCollection<TestAction> _TestActions;

        #endregion TestActions

        #region Filtering (view)

        public string Filter
        {
            get => _Filter;
            set
            {
                if (Set(ref _Filter, value))
                    _TestActionViewSource.View.Refresh();
            }
        }

        private string _Filter;

        #endregion Filtering (view)

        public HistoryViewModel(
            IDbRepository<RadarBoard> IRepositoryRadarBoard,
            IDbRepository<TestGroup> IRepositoryTestGroup,
            IDbRepository<TestAction> IRepositoryTestAction,
            IManagerUser IManagerUser,
            IUserDialogService IUserDialogService
            )
        {
            _IRepositoryRadarBoard = IRepositoryRadarBoard;
            _IRepositoryTestGroup = IRepositoryTestGroup;
            _IRepositoryTestAction = IRepositoryTestAction;
            _IManagerUser = IManagerUser;
            _IUserDialogService = IUserDialogService;

            RadarBoards = new ObservableCollection<RadarBoard>();
            RadarBoards.CollectionChanged += RadarBoards_CollectionChanged;
            _RadarBoardViewSource = new()
            {
                Source = RadarBoards
            };
            RadarBoards.Add(_IRepositoryRadarBoard.Items.ToList());

            _TestGroupsOld = new();
            TestGroups = new ObservableCollection<TestGroup>();
            TestGroups.CollectionChanged += TestGroups_CollectionChanged;
            _TestGroupViewSource = new()
            {
                Source = TestGroups
            };
            _TestActions = new ObservableCollection<TestAction>();
            _TestActions.CollectionChanged += _TestActions_CollectionChanged;
            _TestActionViewSource = new()
            {
                Source = _TestActions
            };
        }

        private void _TestActions_CollectionChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e) => TestActionsView.Refresh();

        private void TestGroups_CollectionChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            TestGroupsView.Refresh();
            _TestGroupsOld.Clear();
            _TestGroupsOld.AddRange(TestGroups);
        }

        private void RadarBoards_CollectionChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e) => RadarBoardsView.Refresh();

        #region SaveDataCommand

        private ICommand _SaveDataCommand;

        /// <summary>
        /// Default command.
        /// </summary>
        public ICommand SaveDataCommand => _SaveDataCommand ??= new LambdaCommand(OnSaveDataCommandExecuted, CanSaveDataCommandExecute);

        /// <summary>
        /// Can execute default command .
        /// </summary>
        /// <param name="arg">The arg.</param>
        /// <returns>A bool.</returns>
        private bool CanSaveDataCommandExecute(object arg) => true;

        /// <summary>
        /// Default function.
        /// </summary>
        /// <param name="obj">The obj.</param>
        private void OnSaveDataCommandExecuted(object obj)
        {
            var list = _TestGroupsOld.Where(s =>
            (_TestGroups.First(w => w.ID == s.ID).Comment.Text != s.Comment.Text) ||
            (_TestGroups.First(w => w.ID == s.ID).Comment.Title != s.Comment.Title)).ToList();
            foreach (var l in list)
                l.Comment.User = _IManagerUser.LoginedUser;

            _IRepositoryTestGroup.UpdateAsync(list);
            _IUserDialogService.ShowInformation("Save complete", "Database");
        }

        #endregion SaveDataCommand

        private void _TestActionViewSource_Filter(object sender, FilterEventArgs e)
        {
            if (e.Item is not TestAction testAction || string.IsNullOrEmpty(testAction.Name)) return;
            Filter ??= "";
            if (!(

                    (testAction.Name.ToLower().Contains(Filter.ToLower()))
                    || (testAction.TestGroup.Name.ToLower().Contains(Filter.ToLower()))
                    || (testAction.TestGroup.User.Name.ToLower().Contains(Filter.ToLower()))
                    || (testAction.TestGroup.RadarBoard.MainBoardID.ToLower().Contains(Filter.ToLower()))
                ))
                e.Accepted = false;
        }
    }
}