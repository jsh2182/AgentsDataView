using AgentsDataView.Common.Utilities;
using AgentsDataView.Data.Contracts;
using AgentsDataView.Entities;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using System.Data.Common;
using System.Linq.Expressions;

namespace AgentsDataView.Data.Repositories
{
    public class Repository<TEntity> : IRepository<TEntity> where TEntity : class, IEntity
    {
        protected readonly ApplicationDbContext DbContext;
        public DbSet<TEntity> Entities { get; }
        public IQueryable<TEntity> Query => Entities;
        public IQueryable<TEntity> QueryNoTracking => Entities.AsNoTracking();
        public DbConnection DbConnection { get; }
        public Repository(ApplicationDbContext dbContext)
        {
            DbContext = dbContext;
            Entities = DbContext.Set<TEntity>();
            DbConnection = DbContext.Database.GetDbConnection();
        }

        #region Async Methods
        public virtual ValueTask<TEntity?> GetByIdAsync(CancellationToken cancellationToken, params object?[]? ids)
        {
            return Entities.FindAsync(ids, cancellationToken);
        }
        public virtual async Task AddAsync(TEntity entity, CancellationToken cancellationToken, bool saveNow = true)
        {
            Assert.NotNull(entity, nameof(entity)); ;
            await Entities.AddAsync(entity, cancellationToken).ConfigureAwait(false);
            if (saveNow)
            {
                await DbContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
            }
        }

        public virtual async Task AddRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken, bool saveNow = true)
        {
            Assert.NotNull(entities, nameof(entities));
            await Entities.AddRangeAsync(entities, cancellationToken).ConfigureAwait(false);
            if (saveNow)
            {
                await DbContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
            }
        }
        public virtual async Task BulkInsertAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken)
        {
            Assert.NotNull(entities, nameof(entities));
            await DbContext.BulkInsertAsync(entities, cancellationToken);
        }
        public virtual async Task BulkInsertAsync(IEnumerable<TEntity> entities, SqlTransaction transaction, CancellationToken cancellationToken)
        {
            Assert.NotNull(entities, nameof(entities));
            await DbContext.BulkInsertAsync(entities, transaction, cancellationToken);
        }
        public virtual async Task BulkInsertWithOutputIdsAsync(IList<TEntity> entities, CancellationToken cancellationToken, SqlTransaction? transaction = null)
        {
            Assert.NotNull(entities, nameof(entities));
            await DbContext.BulkInsertWithOutputIdsAsync(entities, cancellationToken, transaction);
        }
        public virtual async Task UpdateAsync(TEntity entity, CancellationToken cancellationToken, bool saveNow = true)
        {
            Assert.NotNull(entity, nameof(entity));
            Entities.Update(entity);
            if (saveNow)
            {
                await DbContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
            }
        }
        public virtual async Task UpdateRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken, bool saveNow = true)
        {
            Assert.NotNull(entities, nameof(entities));
            Entities.UpdateRange(entities);
            if (saveNow)
            {
                await DbContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
            }
        }
        public virtual async Task DeleteAsync(TEntity entity, CancellationToken cancellationToken, bool saveNow = true)
        {
            Assert.NotNull(entity, nameof(entity));
            Entities.Remove(entity);
            if (saveNow)
            {
                await DbContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
            }
        }
        public virtual async Task DeleteRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken, bool saveNow = true)
        {
            Assert.NotNull(entities, nameof(entities));
            Entities.RemoveRange(entities);
            if (saveNow)
            {
                await DbContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
            }
        }


        #endregion
    }
}
