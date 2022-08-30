using BulletNET.Database.Repositories.Interfaces;
using BulletNET.EntityFramework.Context;
using BulletNET.EntityFramework.Entities.Base;
using System.Linq.Expressions;

namespace BulletNET.EntityFramework.Repositories
{
    internal class DbRepository<T> : IDbRepository<T> where T : Entity, new()
    {
        #region Fields

        private readonly DatabaseDB _db;
        private readonly DbSet<T> _Set;

        /// <summary>
        /// Autosave for changes.
        /// </summary>
        public bool AutoSaveChanges { get; set; } = true;

        #endregion Fields

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="DbRepository"/> class.
        /// </summary>
        /// <param name="db">The db.</param>
        public DbRepository(DatabaseDB db)
        {
            _db = db;
            _Set = db.Set<T>();
        }

        #endregion Constructor

        /// <summary>
        /// All items of repository.
        /// </summary>
        public virtual IQueryable<T> Items => _Set;

        #region Get

        /// <summary>
        /// Get value.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns>A T.</returns>
        public T Get(int id) => Items.SingleOrDefault(item => item.ID == id);

        /// <summary>
        /// Get value async.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <param name="Cancel">The cancel.</param>
        /// <returns>A Task.</returns>
        public async Task<T> GetAsync(int id, CancellationToken Cancel = default) => await Items
            .SingleOrDefaultAsync(item => item.ID == id, Cancel)
            .ConfigureAwait(false);

        #endregion Get

        #region Add

        /// <summary>
        /// Add item in repository and db.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns>A T.</returns>
        public T Add(T item)
        {
            if (item is null) throw new ArgumentNullException(nameof(item));
            _db.Entry(item).State = EntityState.Added;
            if (AutoSaveChanges)
                _db.SaveChanges();
            return item;
        }

        public void Add(IEnumerable<T> item)
        {
            foreach (T? it in item) Add(it);
        }

        /// <summary>
        /// Add item in repository and db async
        /// </summary>
        /// <param name="item">The item.</param>
        /// <param name="Cancel">The cancel.</param>
        /// <returns>A Task.</returns>
        public async Task<T> AddAsync(T item, CancellationToken Cancel = default)
        {
            if (item is null) throw new ArgumentNullException(nameof(item));
            _db.Entry(item).State = EntityState.Added;
            if (AutoSaveChanges)
                await _db.SaveChangesAsync(Cancel).ConfigureAwait(false);

            return item;
        }

        public T? AddIfNotExists(T item, Expression<Func<T, bool>> predicate = null)
        {
            bool exists = predicate is null ? _Set.Any() : _Set.Any(predicate);
            return exists ? null : Add(item);
        }

        public void AddIfNotExists(IEnumerable<T> item, Expression<Func<T, bool>> predicate = null)
        {
            foreach (T? it in item) AddIfNotExists(it, predicate);
        }

        #endregion Add

        #region Update

        /// <summary>
        /// Update item in repository and db.
        /// </summary>
        /// <param name="item">The item.</param>
        public void Update(T item)
        {
            if (item is null) throw new ArgumentNullException(nameof(item));
            _db.Entry(item).State = EntityState.Modified;
            if (AutoSaveChanges)
                _db.SaveChanges();
        }

        public void Update(IEnumerable<T> item)
        {
            foreach (var it in item)
                Update(it);
        }

        public async Task UpdateAsync(IEnumerable<T> item, CancellationToken Cancel = default)
        {
            foreach (var it in item)
                await UpdateAsync(it);
        }

        /// <summary>
        /// Update item in repository and db async.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <param name="Cancel">The cancel.</param>
        /// <returns>A Task.</returns>
        public async Task UpdateAsync(T item, CancellationToken Cancel = default)
        {
            if (item is null) throw new ArgumentNullException(nameof(item));
            _db.Entry(item).State = EntityState.Modified;
            if (AutoSaveChanges)
                await _db.SaveChangesAsync(Cancel).ConfigureAwait(false);
        }

        #endregion Update

        #region Remove

        /// <summary>
        /// Remove item in repository and db.
        /// </summary>
        /// <param name="id">The id.</param>
        public void Remove(int id)
        {
            var item = _Set.Local.FirstOrDefault(i => i.ID == id) ?? new T { ID = id };

            _db.Remove(item);

            if (AutoSaveChanges)
                _db.SaveChanges();
        }

        /// <summary>
        /// Remove item in repository and db async.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <param name="Cancel">The cancel.</param>
        /// <returns>A Task.</returns>
        public async Task RemoveAsync(int id, CancellationToken Cancel = default)
        {
            _db.Remove(new T { ID = id });
            if (AutoSaveChanges)
                await _db.SaveChangesAsync(Cancel).ConfigureAwait(false);
        }

        #endregion Remove
    }
}