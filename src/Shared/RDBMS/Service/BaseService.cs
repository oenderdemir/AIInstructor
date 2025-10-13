using AutoMapper;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using System.Linq.Expressions;
using AIInstructor.src.Shared.RDBMS.Dto;
using AIInstructor.src.Shared.RDBMS.Entity;
using AIInstructor.src.Shared.RDBMS.Repository;
using static Grpc.Core.Metadata;

namespace AIInstructor.src.Shared.RDBMS.Service
{
    public class BaseService<TDto, TEntity> : IBaseService<TDto, TEntity>
        where TEntity : BaseEntity
        where TDto : BaseRDBMSDto
    {
        protected readonly IBaseRepository<TEntity> _repository;
        protected readonly IMapper _mapper;
    

        public BaseService(IBaseRepository<TEntity> repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
           
        }

        public virtual async Task<IEnumerable<TDto>> GetAllAsync(Func<IQueryable<TEntity>, IQueryable<TEntity>>? include = null)
        {
            var entities = await _repository.GetAllAsync(include);
            return _mapper.Map<IEnumerable<TDto>>(entities);
        }

        public virtual async Task<bool> AnyAsync(Expression<Func<TEntity, bool>> predicate, Func<IQueryable<TEntity>, IQueryable<TEntity>>? include = null)
        {
            
            return await _repository.AnyAsync(predicate, include);
        }


        public virtual async Task<TDto> GetByIdAsync(Guid id, Func<IQueryable<TEntity>, IQueryable<TEntity>>? include = null)
        {
            var entity = await _repository.GetByIdAsync(id, include);
            return _mapper.Map<TDto>(entity);
        }

        public virtual async Task<TDto> AddAsync(TDto dto)
        {
            if(!dto.Id.HasValue)
            {
                dto.Id=Guid.NewGuid();
            }
            var entity = _mapper.Map<TEntity>(dto);
            await EnrichEntityAsync(dto, entity);
            await _repository.AddAsync(entity);
            await _repository.SaveChangesAsync();
            try
            {

                await ToCouchBase(entity.Id);
            }
            catch(Exception ex)
            {
               
            }
            
            return dto;
        }

        public virtual async Task<TDto> SyncAsync(TDto dto)
        {
            var entity = _mapper.Map<TEntity>(dto);
            await EnrichEntityAsync(dto, entity);
            await _repository.SyncAsync(entity);
            await _repository.SaveChangesAsync();
            
            return dto;
        }

        protected virtual Task EnrichEntityAsync(TDto dto, TEntity entity)
        {
            return Task.CompletedTask;
        }

        public virtual Task ToCouchBase(Guid id)
        {
            return Task.CompletedTask;
        }



        public virtual async Task<TDto> UpdateAsync(TDto dto)
        {
            if (dto.Id.HasValue)
            {
                
                var existingEntity = await _repository.GetByIdAsync(dto.Id.Value);
                if (existingEntity == null)
                {
                    throw new Exception("Güncellenecek veri bulunamadı.");
                }
                existingEntity.IsDeleted=false;

                _mapper.Map(dto, existingEntity); // var olan nesneye map et → EF Core değişiklikleri takip eder
                await EnrichEntityAsync(dto, existingEntity);
                _repository.Update(existingEntity);
                await _repository.SaveChangesAsync();
                try
                {
                   
                    await ToCouchBase(existingEntity.Id);
                }
                catch (Exception ex)
                {

                   
                }

                return _mapper.Map<TDto>(existingEntity); // en güncel halini döner
            }

            throw new Exception("Id boş olamaz.");
        }


        public virtual async Task DeleteAsync(Guid id)
        {
            var entity = await _repository.GetByIdAsync(id);
            if (entity == null)
                throw new Exception("Entity not found");

            _repository.Delete(entity); // sadece Remove çağrısı yeterli
            await _repository.SaveChangesAsync();
        }

        public virtual async Task<IEnumerable<TDto>> WhereAsync(Expression<Func<TEntity, bool>> predicate, Func<IQueryable<TEntity>, IQueryable<TEntity>>? include = null)
        {
            var entities= _repository.Where(predicate, include);
            return _mapper.Map<IEnumerable<TDto>>(entities);

        }
    }

}
