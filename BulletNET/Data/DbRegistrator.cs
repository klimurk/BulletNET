using BulletNET.EntityFramework;
using BulletNET.EntityFramework.Context;
using BulletNET.Services.UserDialogService.Interfaces;
using Microsoft.Data.SqlClient;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MySqlConnector;

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
                        try
                        {
                            string connString = Configuration.GetConnectionString(type);
                            using (SqlConnection conn = new(connString))
                            {
                                conn.Open();
                            }
                            opt.
                            opt.UseSqlServer(Configuration.GetConnectionString(type));
                        }
                        catch (Exception)
                        {
                        }
                        break;

                    case "SQLite":
                        try
                        {
                            string connString = Configuration.GetConnectionString(type);
                            using (SqliteConnection conn = new(connString))
                            {
                                conn.Open();
                            }
                            opt.UseSqlite(Configuration.GetConnectionString(type));
                        }
                        catch (Exception)
                        {
                        }
                        break;

                    case "MySqlHome":
                        try
                        {
                            string connString = Configuration.GetConnectionString(type);
                            using (MySqlConnection conn = new(connString))
                            {
                                conn.Open();
                            }
                            opt.UseMySql(Configuration.GetConnectionString(type), ServerVersion.AutoDetect(Configuration.GetConnectionString(type)));
                        }
                        catch (Exception)
                        {
                        }
                        break;

                    case "MySqlOffice":
                        try
                        {
                            string connString = Configuration.GetConnectionString(type);
                            using (SqlConnection conn = new(connString))
                            {
                                conn.Open();
                            }
                            opt.UseMySql(Configuration.GetConnectionString(type), ServerVersion.AutoDetect(Configuration.GetConnectionString(type)));
                        }
                        catch (Exception)
                        {
                        }
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