using BulletNET.EntityFramework;
using BulletNET.EntityFramework.Context;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BulletNET.Data
{
    internal static class DbRegistrator
    {
        public static IServiceCollection RegisterDatabase(this IServiceCollection services, IConfiguration Configuration) => services
            .AddDbContext<DatabaseDB>(opt =>
            {
                var type = Configuration["Type"];
                switch (type)
                {
                    case null: throw new InvalidOperationException("Not defined database type");
                    default: throw new InvalidOperationException($"Connection type {type} not supported");

                    case "MSSQL":
                        opt.UseSqlServer(Configuration.GetConnectionString(type));
                        break;

                    case "SQLite":
                        opt.UseSqlite(Configuration.GetConnectionString(type));
                        break;

                    case "MySqlHome":
                        opt.UseMySql(Configuration.GetConnectionString(type), ServerVersion.AutoDetect(Configuration.GetConnectionString(type)));
                        break;

                    case "MySqlOffice":
                        opt.UseMySql(Configuration.GetConnectionString(type), ServerVersion.AutoDetect(Configuration.GetConnectionString(type)));
                        break;

                    case "InMemory":
                        opt.UseInMemoryDatabase("Database.db");
                        break;
                }
            })
            .AddTransient<DbInitializer>()
            .AddRepositoriesInDB()
        ;
    }
}