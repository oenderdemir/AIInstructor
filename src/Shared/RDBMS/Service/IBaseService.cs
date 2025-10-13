using System.Linq.Expressions;

namespace AIInstructor.src.Shared.RDBMS.Service
{
    public interface IBaseService<TDto, TEntity>
    {
        Task<IEnumerable<TDto>> GetAllAsync(Func<IQueryable<TEntity>, IQueryable<TEntity>>? include = null);
        Task<TDto> GetByIdAsync(Guid id, Func<IQueryable<TEntity>, IQueryable<TEntity>>? include = null);
        Task<TDto> AddAsync(TDto dto);
        Task<TDto> UpdateAsync(TDto dto);
        Task DeleteAsync(Guid id);
        Task<IEnumerable<TDto>> WhereAsync(Expression<Func<TEntity, bool>> predicate,Func<IQueryable<TEntity>, IQueryable<TEntity>>? include = null);


        Task<bool> AnyAsync(Expression<Func<TEntity, bool>> predicate, Func<IQueryable<TEntity>, IQueryable<TEntity>>? include = null);
    }
}
