using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BulletNET.EntityFramework.Context.Configuration.Base
{
    internal abstract class DbEntityConfiguration<TEntity> where TEntity : class
    {
        public abstract void Configure(EntityTypeBuilder<TEntity> entity);
    }
}