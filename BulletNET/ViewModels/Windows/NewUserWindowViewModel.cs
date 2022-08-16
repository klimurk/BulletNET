using BulletNET.Infrastructure.Commands;
using BulletNET.Services.Managers;
using BulletNET.ViewModels.Base;
using Pallet.Services.UserDialogService.Interfaces;

namespace BulletNET.ViewModels.Windows
{
    internal class NewUserWindowViewModel : ViewModel
    {
        #region Services

        private readonly IUserDialogService _UserDialogService;
        private readonly IManagerUser _UserManager;

        #endregion Services

        #region Fields

        /// <summary>
        /// Username input.
        /// </summary>
        public string UserName { get => _UserName; set => Set(ref _UserName, value); }

        private string _UserName;

        /// <summary>
        /// Username input.
        /// </summary>
        public string Description { get => _Description; set => Set(ref _Description, value); }

        private string _Description;

        /// <summary>
        /// Username input.
        /// </summary>
        public string Password { get => _Password; set => Set(ref _Password, value); }

        private string _Password;

        public string SelectedRoleNum { get => _SelectedRoleNum; set => Set(ref _SelectedRoleNum, value); }

        private string _SelectedRoleNum;

        public List<string> RoleNums { get => _RoleNums; set => Set(ref _RoleNums, value); }

        private List<string> _RoleNums;

        #endregion Fields

        /// <summary>
        /// Initializes a new instance of the <see cref="LoginWindowViewModel"/> class.
        /// </summary>
        /// <param name="UserManager">The user manager.</param>
        /// <param name="UserDialogService">The user dialog service.</param>
        public NewUserWindowViewModel(
            IManagerUser UserManager, IUserDialogService UserDialogService
            )
        {
            _UserDialogService = UserDialogService;
            _UserManager = UserManager;
            RoleNums = new()
            {
                nameof(ManagerUser.UserRoleNum.Worker),
                nameof(ManagerUser.UserRoleNum.Manager),
                nameof(ManagerUser.UserRoleNum.Admin)
            };
            //RoleNums.Append(nameof(ManagerUser.UserRoleNum.Worker));
            //RoleNums.Append(nameof(ManagerUser.UserRoleNum.Manager));
            //RoleNums.Append(nameof(ManagerUser.UserRoleNum.Admin));
        }

        #region CreateNewUserCommand

        private ICommand _CreateNewUserCommand;

        /// <summary>
        /// Try to log in command.
        /// </summary>
        public ICommand CreateNewUserCommand => _CreateNewUserCommand ??= new LambdaCommand(OnCreateNewUserCommandExecuted, CanCreateNewUserCommandExecute);

        /// <summary>
        /// Can command execute.
        /// </summary>
        /// <param name="arg">The arg.</param>
        /// <returns>A bool.</returns>
        private bool CanCreateNewUserCommandExecute(object arg) => !string.IsNullOrEmpty(UserName) && !string.IsNullOrEmpty(Description) && !string.IsNullOrEmpty(Password) && !string.IsNullOrEmpty(SelectedRoleNum);

        /// <summary>
        /// Try to log in realization.
        /// </summary>
        /// <param name="obj">The obj.</param>
        private void OnCreateNewUserCommandExecuted(object obj)
        {
            var window = (Window)obj;

            Enum.TryParse(SelectedRoleNum, out ManagerUser.UserRoleNum role);

            if (_UserManager.CreateNewUser(UserName, Description, Password, (int)role))
                window.Close();
        }

        #endregion CreateNewUserCommand
    }
}