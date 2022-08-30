using BulletNET.EntityFramework.Entities.Radar;
using BulletNET.EntityFramework.Entities.Users;

namespace BulletNET.EntityFramework.Context
{
    public class DatabaseDB : DbContext
    {
        /// <summary>
        /// Context for users.
        /// </summary>
        public DbSet<User> Users { get; set; }

        /// <summary>
        /// Context for EventLogs.
        /// </summary>

        /// <summary>
        /// Context for users.
        /// </summary>
        public DbSet<RadarBoard> RadarBoards { get; set; }

        /// <summary>
        /// Context for Alarm logs.
        /// </summary>
        public DbSet<TestAction> TestActions { get; set; }

        /// <summary>
        /// Context for alarms definitions.
        /// </summary>
        public DbSet<TestGroup> TestGroups { get; set; }

        /// <summary>
        /// Context for signals definitions.
        /// </summary>

        #region Ctor

        /// <summary>
        /// Initializes a new instance of the <see cref="DatabaseDB"/> class.
        /// Set lazy loading disabled (internal objects must be loaded)
        /// </summary>
        /// <param name="options">The options.</param>
        public DatabaseDB(DbContextOptions<DatabaseDB> options) : base(options)
        {
            ChangeTracker.LazyLoadingEnabled = false;
            
        }
        
        #endregion Ctor

        #region Creating model (convertors, keys...)

        /// <summary>
        /// On the model creating.
        /// Convert db values to internal and back
        /// Correct lazy loading
        /// </summary>
        /// <param name="modelBuilder">The model builder.</param>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //Keys
            modelBuilder.Entity<TestGroup>().HasIndex(p => p.TimeStamp).IsUnique();
        }

        #endregion Creating model (convertors, keys...)

        #region Extensions

        public void RefreshAll()
        {
            foreach (var entity in ChangeTracker.Entries()) entity.Reload();
        }

        #endregion Extensions
    }
}