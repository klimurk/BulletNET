using BulletNET.Database.Repositories.Interfaces;
using BulletNET.EntityFramework.Entities.Users;
using Pallet.Services.UserDialogService.Interfaces;
using System.Security.Cryptography;

namespace BulletNET.Services.Managers
{
    public class ManagerUser : IManagerUser
    {
        private readonly IDbRepository<User> _UserRepository;
        private readonly IUserDialogService _IUserDialogService;

        public event EventHandler UserChanged;

        public event EventHandler UserAdded;

        private User _LoginedUser;

        public User? LoginedUser
        {
            get => _LoginedUser;
            set
            {
                _LoginedUser = value;
                UserChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        public IQueryable<User> Users => _UserRepository.Items;

        public ManagerUser(
            IDbRepository<User> UserRepository,
            IUserDialogService IUserDialogService)
        {
            _UserRepository = UserRepository;
            _IUserDialogService = IUserDialogService;
        }

        public bool Login(string name, string password)
        {
            if (name == "administrator" && password == "btadmin")
            {
                LoginedUser = new User()
                {
                    Name = name,
                    RoleNum = 10,
                    Description = "Built-In Administrator"
                };
                return true;
            }

            //Create a SHA256 hash from string

            string tempHashCode = CreateHashCode(password);

            try
            {
                LoginedUser = Users.First(x => x.Name == name && x.Hashcode == tempHashCode);
            }
            catch { return false; }
            return true;
        }

        public void LogOut() => LoginedUser = null;

        public bool CreateNewUser(string userName, string Description, string password, int RoleNum)
        {
            if (string.IsNullOrEmpty(userName) || string.IsNullOrEmpty(Description) || string.IsNullOrEmpty(password)) return false;
            if (Users is not null && Users.Any(s => s.Name == userName))
            {
                _IUserDialogService.ShowWarning("Username already exist", "Create new user");
                return false;
            }

            _UserRepository.Add(new User()
            {
                Name = userName,
                Description = Description,
                Hashcode = CreateHashCode(password),
                RoleNum = RoleNum
            });
            UserAdded?.Invoke(this, EventArgs.Empty);
            _IUserDialogService.ShowInformation("User has been created", "New user");
            return true;
        }

        public bool ChangeUserPassword(string userName, string password)
        {
            if (string.IsNullOrEmpty(userName) || string.IsNullOrEmpty(password)) return false;
            if (!Users.Any(s => s.Name == userName))
            {
                _IUserDialogService.ShowWarning("User has not been finded", "Change user password");
                return false;
            }

            User editedUser = _UserRepository.Items.First(s => s.Name == userName);
            editedUser.Hashcode = CreateHashCode(password);
            _UserRepository.Update(editedUser);

            _IUserDialogService.ShowInformation("User password has been changed", "Change user password");
            return true;
        }

        public bool IsLogined => LoginedUser != null || !string.IsNullOrEmpty(LoginedUser?.Name);

        public bool IsAdmin => LoginedUser?.RoleNum == 10;

        public enum UserRoleNum
        {
            None = 0,
            Worker = 1,
            Manager = 2,
            Admin = 10
        }

        private string CreateHashCode(string toHash)
        {
            using (SHA256 sha256Hash = SHA256.Create())
            {
                // Computing Hash - returns here byte array
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(toHash));

                // now convert byte array to a string
                StringBuilder stringbuilder = new();
                for (int i = 0; i < bytes.Length; i++)
                    stringbuilder.Append(bytes[i].ToString("X2"));

                return stringbuilder.ToString();
            }
        }
    }
}