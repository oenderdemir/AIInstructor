using System;
using System.Collections.Generic;
using AutoMapper;
using AIInstructor.src.AIKisiOzellik.DTO;
using AIInstructor.src.AIKisiOzellik.Entity;
using AIInstructor.src.AIKisiOzellik.Repository;

namespace AIInstructor.src.AIKisiOzellik.Service
{
    public class AIKisiOzellikService : IAIKisiOzellikService
    {
        private readonly IAIKisiOzellikRepository repository;
        private readonly IMapper mapper;

        public AIKisiOzellikService(IAIKisiOzellikRepository repository, IMapper mapper)
        {
            this.repository = repository;
            this.mapper = mapper;
        }

        public async Task<IEnumerable<AIKisiOzellikDto>> GetBySenaryoIdAsync(Guid senaryoId)
        {
            var entities = await repository.GetBySenaryoIdAsync(senaryoId);
            return mapper.Map<IEnumerable<AIKisiOzellikDto>>(entities);
        }

        public async Task<AIKisiOzellikDto> CreateAsync(AIKisiOzellikCreateDto dto)
        {
            var entity = mapper.Map<AIKisiOzellik>(dto);
            entity.Id = dto.Id ?? Guid.NewGuid();
            await repository.SyncAsync(entity);
            await repository.SaveChangesAsync();
            return mapper.Map<AIKisiOzellikDto>(entity);
        }

        public async Task DeleteAsync(Guid id)
        {
            var entity = await repository.GetByIdAsync(id);
            if (entity == null)
            {
                throw new ArgumentException("Özellik bulunamadı", nameof(id));
            }

            repository.Delete(entity);
            await repository.SaveChangesAsync();
        }
    }
}
