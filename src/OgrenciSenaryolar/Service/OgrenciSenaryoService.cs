using System;
using System.Collections.Generic;
using AutoMapper;
using AIInstructor.src.OgrenciSenaryolar.DTO;
using AIInstructor.src.OgrenciSenaryolar.Entity;
using AIInstructor.src.OgrenciSenaryolar.Repository;

namespace AIInstructor.src.OgrenciSenaryolar.Service
{
    public class OgrenciSenaryoService : IOgrenciSenaryoService
    {
        private readonly IOgrenciSenaryoRepository repository;
        private readonly IMapper mapper;

        public OgrenciSenaryoService(IOgrenciSenaryoRepository repository, IMapper mapper)
        {
            this.repository = repository;
            this.mapper = mapper;
        }

        public async Task<IEnumerable<OgrenciSenaryoDto>> GetByOgrenciIdAsync(Guid ogrenciId)
        {
            var entities = await repository.GetByOgrenciIdAsync(ogrenciId);
            return mapper.Map<IEnumerable<OgrenciSenaryoDto>>(entities);
        }

        public async Task<OgrenciSenaryoDto> AssignAsync(OgrenciSenaryoAssignDto dto)
        {
            var existing = await repository.FirstOrDefaultAsync(e => e.OgrenciId == dto.OgrenciId && e.SenaryoId == dto.SenaryoId);
            if (existing != null)
            {
                existing.BaslamaTarihi = DateTime.UtcNow;
                existing.BitisTarihi = null;
                existing.Puan = null;
                existing.Badge = null;
                repository.Update(existing);
                await repository.SaveChangesAsync();
                return mapper.Map<OgrenciSenaryoDto>(existing);
            }

            var entity = new OgrenciSenaryo
            {
                Id = Guid.NewGuid(),
                OgrenciId = dto.OgrenciId,
                SenaryoId = dto.SenaryoId,
                BaslamaTarihi = dto.BaslamaTarihi ?? DateTime.UtcNow
            };

            await repository.AddAsync(entity);
            await repository.SaveChangesAsync();

            return mapper.Map<OgrenciSenaryoDto>(entity);
        }

        public async Task<OgrenciSenaryoDto> CompleteAsync(OgrenciSenaryoCompleteDto dto)
        {
            var entity = await repository.GetByIdAsync(dto.OgrenciSenaryoId);
            if (entity == null)
            {
                throw new ArgumentException("Öğrenci senaryo kaydı bulunamadı", nameof(dto.OgrenciSenaryoId));
            }

            entity.BitisTarihi = dto.BitisTarihi ?? DateTime.UtcNow;
            entity.Puan = dto.Puan;
            entity.Badge = dto.Badge;

            repository.Update(entity);
            await repository.SaveChangesAsync();

            return mapper.Map<OgrenciSenaryoDto>(entity);
        }
    }
}
