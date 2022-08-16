using BulletNET.Database.Repositories.Interfaces;
using BulletNET.EntityFramework.Entities.Radar;
using BulletNET.EntityFramework.Entities.Users;
using BulletNET.EntityFramework.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace BulletNET.EntityFramework
{
    public static class RepositoryRegistrator
    {
        public static IServiceCollection AddRepositoriesInDB(this IServiceCollection services) => services
            .AddTransient<IDbRepository<TestAction>, DbRepository<TestAction>>()
            .AddTransient<IDbRepository<RadarBoard>, DbRepository<RadarBoard>>()
            .AddTransient<IDbRepository<TestGroup>, DbRepository<TestGroup>>()
            .AddTransient<IDbRepository<User>, DbRepository<User>>()
        ;
    }
}