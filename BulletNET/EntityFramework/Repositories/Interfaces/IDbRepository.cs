using BulletNET.Database.Entities.Base.Interfaces;

namespace BulletNET.Database.Repositories.Interfaces
{
    /// <summary>
    /// Base repository for database interface.
    /// </summary>
    public interface IDbRepository<T> where T : class, IEntity, new()
    {
        /// <summary>
        /// Items from database.
        /// </summary>
        IQueryable<T> Items { get; }

        /// <summary>
        /// Get item by ID
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns>A T.</returns>
        T Get(int id);

        /// <summary>
        /// Get item by ID async.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <param name="Cancel">The cancel.</param>
        /// <returns>A Task.</returns>
        Task<T> GetAsync(int id, CancellationToken Cancel = default);

        /// <summary>
        /// Add item and save.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns>A T.</returns>
        T Add(T item);

        /// <summary>
        /// Add item and save.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns>A T.</returns>
        void Add(IEnumerable<T> item);

        /// <summary>
        /// Add item and save async.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <param name="Cancel">The cancel.</param>
        /// <returns>A Task.</returns>
        Task<T> AddAsync(T item, CancellationToken Cancel = default);

        /// <summary>
        /// Update item and save.
        /// </summary>
        /// <param name="item">The item.</param>
        void Update(T item);

        /// <summary>
        /// Update item and save.
        /// </summary>
        /// <param name="item">The item.</param>
        void Update(IEnumerable<T> item);

        /// <summary>
        /// Update item and save async.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <param name="Cancel">The cancel.</param>
        /// <returns>A Task.</returns>
        Task UpdateAsync(T item, CancellationToken Cancel = default);

        /// <summary>
        /// Update item and save async.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <param name="Cancel">The cancel.</param>
        /// <returns>A Task.</returns>
        Task UpdateAsync(IEnumerable<T> item, CancellationToken Cancel = default);

        /// <summary>
        /// Remove item and save.
        /// </summary>
        /// <param name="id">The id.</param>
        void Remove(int id);

        /// <summary>
        /// Remove item and save async.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <param name="Cancel">The cancel.</param>
        /// <returns>A Task.</returns>
        Task RemoveAsync(int id, CancellationToken Cancel = default);
    }
}