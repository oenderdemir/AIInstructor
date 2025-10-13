using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace AIInstructor.src.Shared.RDBMS.Repository
{
    public interface IBaseRepository<TEntity> where TEntity : class
    {
        Task<TEntity> GetByIdAsync(Guid id, Func<IQueryable<TEntity>, IQueryable<TEntity>>? include = null);
        Task<IEnumerable<TEntity>> GetAllAsync(Func<IQueryable<TEntity>, IQueryable<TEntity>>? include = null);
        IQueryable<TEntity> Where(Expression<Func<TEntity, bool>> predicate, Func<IQueryable<TEntity>, IQueryable<TEntity>>? include = null);
        Task<bool> AnyAsync(Expression<Func<TEntity, bool>> predicate, Func<IQueryable<TEntity>, IQueryable<TEntity>>? include = null);
        Task<TEntity> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> predicate, Func<IQueryable<TEntity>, IQueryable<TEntity>>? include = null);
        Task AddAsync(TEntity entity);

        Task SyncAsync(TEntity entity);
        void Update(TEntity entity);
        void Delete(TEntity entity); // Soft delete
        void DeleteWhere(Expression<Func<TEntity, bool>> predicate);
        void DeleteRange(IEnumerable<TEntity> entities);
        Task SaveChangesAsync();

        Task<TEntity> UndoDelete(Guid id);
    }
}
