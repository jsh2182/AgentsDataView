using AgentsDataView.Entities;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System.Data.Common;

namespace AgentsDataView.Data.Contracts
{
    public interface IRepository<TEntity> where TEntity : class, IEntity
    {
        public DbSet<TEntity> Entities { get; }
        public IQueryable<TEntity> Query => Entities;
        public IQueryable<TEntity> QueryNoTracking => Entities.AsNoTracking();
        public DbConnection DbConnection { get; }
        Task AddAsync(TEntity entity, CancellationToken cancellationToken, bool saveNow = true);
        Task AddRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken, bool saveNow = true);
        Task<IDbContextTransaction> BeginTransaction(CancellationToken cancellationToken);
        Task BulkInsertAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken);
        Task BulkInsertAsync(IEnumerable<TEntity> entities, SqlTransaction transaction, CancellationToken cancellationToken);
        Task BulkInsertWithOutputIdsAsync(IList<TEntity> entities, CancellationToken cancellationToken, SqlTransaction? transaction = null);
        Task DeleteAsync(TEntity entity, CancellationToken cancellationToken, bool saveNow = true);
        Task DeleteRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken, bool saveNow = true);
        ValueTask<TEntity?> GetByIdAsync(CancellationToken cancellationToken, params object?[]? ids);
        Task UpdateAsync(TEntity entity, CancellationToken cancellationToken, bool saveNow = true);
        Task UpdateRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken, bool saveNow = true);
    }
}
