using BulletNET.EntityFramework.Entities.Users;
using BulletNET.Infrastructure.Commands;
using BulletNET.Services.Managers;
using BulletNET.ViewModels.Base;

namespace BulletNET.ViewModels.SubView
{
    internal class AdminViewModel : ViewModel
    {
        #region Users

        public ICollectionView UsersView => _UserViewSource.View;

        private readonly CollectionViewSource _UserViewSource;
        private readonly IManagerUser _IManagerUser;
        private ObservableCollection<User> _Users;

        #endregion Users

        #region Filtering (view)

        public string Filter
        {
            get => _Filter;
            set
            {
                if (Set(ref _Filter, value))
                    _UserViewSource.View.Refresh();
            }
        }

        private string _Filter;

        #endregion Filtering (view)

        private bool _IsAdmin;

        public bool IsAdmin
        {
            get => _IsAdmin;
            set => Set(ref _IsAdmin, value);
        }

        public AdminViewModel(
            IManagerUser IManagerUser
            )
        {
            _IManagerUser = IManagerUser;
            _IManagerUser.UserAdded += _IManagerUser_UserAdded;
            _Users = new();
            _Users.CollectionChanged += _IManagerUser_UserAdded;

            _UserViewSource = new()
            {
                Source = _Users,
                SortDescriptions = { new SortDescription(nameof(User.Name), ListSortDirection.Descending) }
            };
            _Users.Add(_IManagerUser.Users);
            _UserViewSource.Filter += _UserViewSource_Filter;
        }

        private void _IManagerUser_UserAdded(object? sender, EventArgs e) => UsersView.Refresh();

        private void _UserViewSource_Filter(object sender, FilterEventArgs e)
        {
            if (e.Item is not User testAction) return;
            Filter ??= "";
            if (!testAction.Name.ToLower().Contains(Filter.ToLower())) e.Accepted = false;
        }

        #region SetNewPasswordCommand

        private ICommand _SetNewPasswordCommand;

        /// <summary>
        /// SetNewPassword command.
        /// </summary>
        public ICommand SetNewPasswordCommand => _SetNewPasswordCommand ??= new LambdaCommand(OnSetNewPasswordCommandExecuted, CanSetNewPasswordCommandExecute);

        /// <summary>
        /// Can execute default command .
        /// </summary>
        /// <param name="arg">The arg.</param>
        /// <returns>A bool.</returns>
        private bool CanSetNewPasswordCommandExecute(object arg) => IsAdmin;

        /// <summary>
        /// SetNewPassword function.
        /// </summary>
        /// <param name="obj">The obj.</param>
        private void OnSetNewPasswordCommandExecuted(object obj)
        {
            if (obj is not User) return;
        }

        #endregion SetNewPasswordCommand

        #region DeleteUserCommand

        private ICommand _DeleteUserCommand;

        /// <summary>
        /// DeleteUser command.
        /// </summary>
        public ICommand DeleteUserCommand => _DeleteUserCommand ??= new LambdaCommand(OnDeleteUserCommandExecuted, CanDeleteUserCommandExecute);

        /// <summary>
        /// Can execute default command .
        /// </summary>
        /// <param name="arg">The arg.</param>
        /// <returns>A bool.</returns>
        private bool CanDeleteUserCommandExecute(object arg) => IsAdmin;

        /// <summary>
        /// DeleteUser function.
        /// </summary>
        /// <param name="obj">The obj.</param>
        private void OnDeleteUserCommandExecuted(object obj)
        {
            if (obj is not User) return;
        }

        #endregion DeleteUserCommand
    }
}