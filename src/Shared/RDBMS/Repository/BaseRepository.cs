using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using AIInstructor.src.Context;

using AIInstructor.src.Shared.RDBMS.Entity;

using static Grpc.Core.Metadata;

namespace AIInstructor.src.Shared.RDBMS.Repository
{
    public class BaseRepository<TEntity> : IBaseRepository<TEntity> where TEntity : BaseEntity
    {
        protected readonly VTSDbContext _context;
        protected readonly DbSet<TEntity> _dbSet;
        private readonly IMapper mapper;

        public BaseRepository(VTSDbContext context, IMapper mapper)
        {
            _context = context;
            _dbSet = context.Set<TEntity>();
            this.mapper = mapper;
        }

        public async Task<TEntity> GetByIdAsync(Guid id, Func<IQueryable<TEntity>, IQueryable<TEntity>>? include = null)
        {
            IQueryable<TEntity> query = _context.Set<TEntity>();

            if (include != null)
                query = include(query);

            return await query.FirstOrDefaultAsync(x => x.Id == id);
        }


        public async Task<IEnumerable<TEntity>> GetAllAsync(Func<IQueryable<TEntity>, IQueryable<TEntity>>? include = null)
        {
            IQueryable<TEntity> query = _context.Set<TEntity>();

            if (include != null)
                query = include(query);

            return await query.ToListAsync();
        }


        public async Task<bool> AnyAsync(Expression<Func<TEntity, bool>> predicate, Func<IQueryable<TEntity>, IQueryable<TEntity>>? include = null)
        {
            IQueryable<TEntity> query = _context.Set<TEntity>();

            if (include != null)
                query = include(query);

            return await query.AnyAsync(predicate);
        }

        public IQueryable<TEntity> Where(Expression<Func<TEntity, bool>> predicate, Func<IQueryable<TEntity>, IQueryable<TEntity>>? include = null)
        {
            IQueryable<TEntity> query = _context.Set<TEntity>();

            if (include != null)
                query = include(query);

            return query.Where(predicate);
        }

        public async Task<TEntity> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> predicate, Func<IQueryable<TEntity>, IQueryable<TEntity>>? include = null)
        {
            IQueryable<TEntity> query = _context.Set<TEntity>();

            if (include != null)
                query = include(query);

            return await query.FirstOrDefaultAsync(predicate);
        }

        public async Task AddAsync(TEntity entity)
        {
            await _dbSet.AddAsync(entity);
        }

        public void Update(TEntity entity)
        {
            _dbSet.Update(entity);
        }

        public void Delete(TEntity entity)
        {
            _dbSet.Remove(entity); // soft-delete logic is handled in VTSDbContext.SaveChanges()
        }

        public void DeleteRange(IEnumerable<TEntity> entities)
        {
            _dbSet.RemoveRange(entities); // soft-delete logic is handled in VTSDbContext.SaveChanges()
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }

        public async Task SyncAsync(TEntity entity)
        {
            if (entity.Id != Guid.Empty)
            {
                var current = await _dbSet.FirstOrDefaultAsync(e => e.Id.Equals(entity.Id));
                if (current != null)
                {

                    mapper.Map(entity, current);
                    current.IsDeleted = false;
                    this._dbSet.Update(current);
                }
                else
                {
                    await _dbSet.AddAsync(entity);
                }
            }
            else
            {
                await _dbSet.AddAsync(entity);
            }
        }

        public  void DeleteWhere(Expression<Func<TEntity, bool>> predicate)
        {
            var entities =  _dbSet.Where(predicate).ToList();
            if (!entities.IsNullOrEmpty())
                _dbSet.RemoveRange(entities);
        }

        public async Task<TEntity> UndoDelete(Guid id)
        {
            IQueryable<TEntity> query = _context.Set<TEntity>().IgnoreQueryFilters();

            

            var item= await query.FirstOrDefaultAsync(x => x.Id == id);
            if(item!= null)
            {
                item.IsDeleted = false;
            }

            await _context.SaveChangesAsync();

            return item;
        }
    }
}
