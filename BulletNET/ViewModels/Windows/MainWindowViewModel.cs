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
using BulletNET.ViewModels.SubView;
using MaterialDesignExtensions.Model;

namespace BulletNET.ViewModels.Windows;

/// @author Oleksii Klymov, Benthor-automation
/// <summary>
/// The main window view model.
/// private class variable "_Variable"
/// public class variable "Variable"
/// const variable "VARIABLE"
/// static class variable "__Variable"
/// function variable "variable"
/// Interfaces "IVariable"
///
/// boolean variables "IsVariable"
///
/// All variable names "CamelCase" "From global to little"
/// </summary>
///

internal class MainWindowViewModel : ViewModel
{
    public string DialogHostName => "dialogHost";
    private HistoryViewModel _HistoryViewModel;

    private DashBoardViewModel _DashBoardViewModel;
    private AdminViewModel _AdminViewModel;

    private ViewModel _CurrentModel;

    public ViewModel CurrentModel
    {
        get => _CurrentModel;
        set => Set(ref _CurrentModel, value);
    }

    #region Services

    #region Entity framework repositories

    //private readonly IDbRepository<User> _IRepositoryUsers;

    #endregion Entity framework repositories

    private readonly IDbRepository<TestAction> _IRepositoryTestAction;
    private readonly IManagerUser _IManagerUser;

    private readonly IDbRepository<RadarBoard> _IRepositoryRadarBoards;
    private readonly IDbRepository<TestAction> _IRepositoryTestActions;
    private readonly IDbRepository<TestGroup> _IRepositoryTestGroups;

    private readonly IBarcodeCRC _IBarcodeCRC;
    private readonly ISequenceReader _ISequenceReader;
    private readonly IQuido _IQuido;
    private readonly IPico _IPico;
    private readonly IManson _IManson;
    private readonly IBluetooth _IBluetooth;
    private readonly IUserDialogService _IUserDialogService;
    private readonly IOpenFileDialogService _IOpenFileDialogService;

    #endregion Services

    #region Properties (View)

    #region Users

    public User CurrentUser
    {
        get => _CurrentUser;
        set => Set(ref _CurrentUser, value);
    }

    private User _CurrentUser;

    #endregion Users

    public bool IsUserLogined
    {
        get => _IsUserLogined;
        set => Set(ref _IsUserLogined, value);
    }

    private bool _IsUserLogined;

    #endregion Properties (View)

    private bool IsAdmin => CurrentUser?.RoleNum >= (int)IManagerUser.UserRoleNum.Admin;

    public ICollectionView NavigationItemsView => _NavigationItemViewSource.View;

    private CollectionViewSource _NavigationItemViewSource;

    public ObservableCollection<INavigationItem> _NavigationItems;

    private INavigationItem _NavigationItemSelected;

    public INavigationItem NavigationItemSelected
    {
        get => _NavigationItemSelected;
        set
        {
            if (Set(ref _NavigationItemSelected, value))
                CurrentModel = (ViewModel)value.NavigationItemSelectedCallback(value);
            IsNavigationDrawerOpen = false;
        }
    }

    private bool _IsNavigationDrawerOpen;

    public bool IsNavigationDrawerOpen
    {
        get => _IsNavigationDrawerOpen;
        set => Set(ref _IsNavigationDrawerOpen, value);
    }

    #region Ctor

    public MainWindowViewModel(
        IDbRepository<RadarBoard> IRepositoryRadarBoards,
        IDbRepository<TestAction> IRepositoryTestActions,
        IDbRepository<TestGroup> IRepositoryTestGroups,
        IManagerUser IManagerUser,
        IBarcodeCRC IBarcodeCRC,
        ISequenceReader ISequenceReader,
        IQuido IQuido,
        IPico IPico,
        IManson IManson,
        IBluetooth IBluetooth,
        IOpenFileDialogService IOpenFileDialogService,
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

        _IManagerUser = IManagerUser;
        _IOpenFileDialogService = IOpenFileDialogService;
        _IUserDialogService = IUserDialogService;
        _IManagerUser.UserChanged += _IManagerUser_UserChanged;
    }

    private void _IManagerUser_UserChanged(object? sender, EventArgs e)
    {
        IsUserLogined = _IManagerUser.IsLogined;
        CurrentUser = _IManagerUser.LoginedUser;
        if (!IsUserLogined) CurrentModel = null;
    }

    #endregion Ctor

    #region Commands

    #region OpenHistoryViewCommand

    private ICommand _OpenHistoryViewCommand;

    /// <summary>
    /// Default command.
    /// </summary>
    public ICommand OpenHistoryViewCommand => _OpenHistoryViewCommand ??= new LambdaCommand(OnOpenHistoryViewCommandExecuted, CanOpenHistoryViewCommandExecute);

    /// <summary>
    /// Can execute default command .
    /// </summary>
    /// <param name="arg">The arg.</param>
    /// <returns>A bool.</returns>
    private bool CanOpenHistoryViewCommandExecute(object arg) => (CurrentModel is not HistoryViewModel) && CurrentUser?.RoleNum >= (int)IManagerUser.UserRoleNum.HistoryViewer;

    /// <summary>
    /// Default function.
    /// </summary>
    /// <param name="obj">The obj.</param>
    private void OnOpenHistoryViewCommandExecuted(object obj)
    {
        CurrentModel = _HistoryViewModel ??= new HistoryViewModel(
            _IRepositoryRadarBoards, _IRepositoryTestGroups,
            _IRepositoryTestAction, _IManagerUser, _IUserDialogService
            );
        //_IRepositoryUsers,
        IsNavigationDrawerOpen = false;
    }

    #endregion OpenHistoryViewCommand

    #region OpenAdminViewCommand

    private ICommand _OpenAdminViewCommand;

    /// <summary>
    /// Default command.
    /// </summary>
    public ICommand OpenAdminViewCommand => _OpenAdminViewCommand ??= new LambdaCommand(OnOpenAdminViewCommandExecuted, CanOpenAdminViewCommandExecute);

    /// <summary>
    /// Can execute default command .
    /// </summary>
    /// <param name="arg">The arg.</param>
    /// <returns>A bool.</returns>
    private bool CanOpenAdminViewCommandExecute(object arg) => IsAdmin;

    /// <summary>
    /// Default function.
    /// </summary>
    /// <param name="obj">The obj.</param>
    private void OnOpenAdminViewCommandExecuted(object obj)
    {
        CurrentModel = _AdminViewModel ??= new AdminViewModel(_IManagerUser);

        IsNavigationDrawerOpen = false;
    }

    #endregion OpenAdminViewCommand

    #region OpenDashboardViewCommand

    private ICommand _OpenDashboardViewCommand;

    /// <summary>
    /// Default command.
    /// </summary>
    public ICommand OpenDashboardViewCommand => _OpenDashboardViewCommand ??= new LambdaCommand(OnOpenDashboardViewCommandExecuted, CanOpenDashboardViewCommandExecute);

    /// <summary>
    /// Can execute default command .
    /// </summary>
    /// <param name="arg">The arg.</param>
    /// <returns>A bool.</returns>
    private bool CanOpenDashboardViewCommandExecute(object arg) => (CurrentModel is not DashBoardViewModel) && CurrentUser?.RoleNum >= (int)IManagerUser.UserRoleNum.Worker;

    /// <summary>
    /// Default function.
    /// </summary>
    /// <param name="obj">The obj.</param>
    private void OnOpenDashboardViewCommandExecuted(object obj)
    {
        CurrentModel = _DashBoardViewModel ??= new DashBoardViewModel(
        _IRepositoryRadarBoards, _IRepositoryTestActions, _IRepositoryTestGroups,
        _ISequenceReader,
        _IBarcodeCRC,
        _IQuido, _IPico, _IManson, _IBluetooth,
        _IOpenFileDialogService, _IManagerUser, _IUserDialogService
        );
        IsNavigationDrawerOpen = false;
    }

    #endregion OpenDashboardViewCommand

    #region LogoutCommand

    private ICommand _LogoutCommand;

    /// <summary>
    /// Logout command.
    /// </summary>
    public ICommand LogoutCommand => _LogoutCommand ??= new LambdaCommand(OnLogoutCommandExecuted, CanLogoutCommandExecute);

    /// <summary>
    /// Can execute default command .
    /// </summary>
    /// <param name="arg">The arg.</param>
    /// <returns>A bool.</returns>
    private bool CanLogoutCommandExecute(object arg) => true;

    /// <summary>
    /// Logout function.
    /// </summary>
    /// <param name="obj">The obj.</param>
    private void OnLogoutCommandExecuted(object obj) => _IManagerUser.LogOut();

    #endregion LogoutCommand

    #endregion Commands
}