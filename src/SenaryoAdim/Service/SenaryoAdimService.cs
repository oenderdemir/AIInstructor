using System;
using System.Collections.Generic;
using AutoMapper;
using AIInstructor.src.SenaryoAdim.DTO;
using AIInstructor.src.SenaryoAdim.Entity;
using AIInstructor.src.SenaryoAdim.Repository;

namespace AIInstructor.src.SenaryoAdim.Service
{
    public class SenaryoAdimService : ISenaryoAdimService
    {
        private readonly ISenaryoAdimRepository repository;
        private readonly IMapper mapper;

        public SenaryoAdimService(ISenaryoAdimRepository repository, IMapper mapper)
        {
            this.repository = repository;
            this.mapper = mapper;
        }

        public async Task<IReadOnlyList<SenaryoAdimDto>> GetBySenaryoIdAsync(Guid senaryoId)
        {
            var entities = await repository.GetBySenaryoIdAsync(senaryoId);
            return mapper.Map<IReadOnlyList<SenaryoAdimDto>>(entities);
        }

        public async Task<SenaryoAdimDto> CreateOrUpdateAsync(SenaryoAdimCreateDto dto)
        {
            var entity = mapper.Map<SenaryoAdim>(dto);
            entity.Id = dto.Id ?? Guid.NewGuid();
            await repository.SyncAsync(entity);
            await repository.SaveChangesAsync();
            return mapper.Map<SenaryoAdimDto>(entity);
        }

        public async Task DeleteAsync(Guid id)
        {
            var entity = await repository.GetByIdAsync(id);
            if (entity == null)
            {
                throw new ArgumentException("Senaryo adımı bulunamadı", nameof(id));
            }

            repository.Delete(entity);
            await repository.SaveChangesAsync();
        }
    }
}
