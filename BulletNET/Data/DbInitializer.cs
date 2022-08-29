using BulletNET.EntityFramework.Context;
using BulletNET.EntityFramework.Entities.Users;
using BulletNET.Services.Managers;

namespace BulletNET.Data
{
    internal class DbInitializer
    {
        private readonly DatabaseDB _db;
        private readonly ILogger<DbInitializer> _Logger;

        public DbInitializer(DatabaseDB db, ILogger<DbInitializer> Logger)
        {
            _db = db;
            _Logger = Logger;
        }

        public async Task InitializeAsync()
        {
            var timer = Stopwatch.StartNew();
            _Logger.LogInformation("Инициализация БД...");

            //_Logger.LogInformation("Удаление существующей БД...");
            //await _db.Database.EnsureDeletedAsync().ConfigureAwait(false);
            //_Logger.LogInformation("Удаление существующей БД выполнено за {0} мс", timer.ElapsedMilliseconds);

            //_db.Database.EnsureCreated();

            _Logger.LogInformation("Миграция БД...");
            await _db.Database.MigrateAsync().ConfigureAwait(false);
            _Logger.LogInformation("Миграция БД выполнена за {0} мс", timer.ElapsedMilliseconds);

            if (!await _db.Users.AnyAsync()) InitializeUsers();

            //await InitializeData();

            _Logger.LogInformation("Инициализация БД выполнена за {0} с", timer.Elapsed.TotalSeconds);
        }

        private async Task InitializeUsers()
        {
            var timer = Stopwatch.StartNew();
            _Logger.LogInformation("Инициализация data...");

            await _db.Users.AddAsync(new User
            {
                Name = "administrator",
                RoleNum = (int)ManagerUser.UserRoleNum.Admin,
                Description = "Administartor",
                Hashcode = ((IManagerUser)App.Services.GetService(typeof(IManagerUser))).CreateHashCode("btadmin")
            });
            await _db.SaveChangesAsync();

            _Logger.LogInformation("Инициализация data выполнена за {0} мс", timer.ElapsedMilliseconds);
        }
    }
}