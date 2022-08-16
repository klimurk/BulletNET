using BulletNET.EntityFramework.Entities.Users;
using BulletNET.Infrastructure.Commands;
using BulletNET.Services.Managers;
using BulletNET.ViewModels.Base;

namespace BulletNET.ViewModels.Windows
{
    internal class ChangeUserPasswordWindowViewModel : ViewModel
    {
        #region Services

        private readonly IManagerUser _UserManager;

        #endregion Services

        #region Fields

        /// <summary>
        /// Username input.
        /// </summary>
        public User CurrentUser { get => _CurrentUser; set => Set(ref _CurrentUser, value); }

        private User _CurrentUser;

        /// <summary>
        /// Username input.
        /// </summary>
        public string NewPassword { get => _NewPassword; set => Set(ref _NewPassword, value); }

        private string _NewPassword;

        #endregion Fields

        /// <summary>
        /// Initializes a new instance of the <see cref="ChangeUserPasswordWindowViewModel"/> class.
        /// </summary>>
        public ChangeUserPasswordWindowViewModel(
            IManagerUser UserManager
            )
        {
            _UserManager = UserManager;
            CurrentUser = _UserManager.LoginedUser;
        }

        #region ChangePasswordCommand

        private ICommand _ChangePasswordCommand;

        /// <summary>
        /// Try to log in command.
        /// </summary>
        public ICommand ChangePasswordCommand => _ChangePasswordCommand ??= new LambdaCommand(OnChangePasswordCommandExecuted, CanChangePasswordCommandExecute);

        /// <summary>
        /// Can command execute.
        /// </summary>
        /// <param name="arg">The arg.</param>
        /// <returns>A bool.</returns>
        private bool CanChangePasswordCommandExecute(object arg) => CurrentUser is null && !string.IsNullOrEmpty(NewPassword);

        /// <summary>
        /// Try to log in realization.
        /// </summary>
        /// <param name="obj">The obj.</param>
        private void OnChangePasswordCommandExecuted(object obj)
        {
            var window = (Window)obj;

            if (_UserManager.ChangeUserPassword(CurrentUser.Name, NewPassword))
            {
                window.Close();
            }
        }

        #endregion ChangePasswordCommand
    }
}