using BulletNET.EntityFramework.Entities.Users;

namespace BulletNET.Services.Managers
{
    public interface IManagerUser
    {
        bool IsLogined { get; }
        User LoginedUser { get; set; }
        IQueryable<User> Users { get; }

        bool Login(string name, string password);

        void LogOut();

        bool IsAdmin { get; }

        bool CreateNewUser(string userName, string Description, string password, int RoleNum);

        bool ChangeUserPassword(string userName, string password);

        event EventHandler UserChanged;

        public enum UserRoleNum
        {
            None = 0,
            Worker = 1,
            Manager = 2,
            Admin = 10
        }
    }
}