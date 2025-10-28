using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using AIInstructor.src.AIKisiOzellik.DTO;
using AIInstructor.src.AIKisiOzellik.Entity;
using AIInstructor.src.AIKisiOzellik.Repository;
using AIInstructor.src.SenaryoAdim.DTO;
using AIInstructor.src.SenaryoAdim.Entity;
using AIInstructor.src.SenaryoAdim.Repository;
using AIInstructor.src.Senaryolar.DTO;
using AIInstructor.src.Senaryolar.Entity;
using AIInstructor.src.Senaryolar.Repository;

namespace AIInstructor.src.Senaryolar.Service
{
    public class SenaryoService : ISenaryoService
    {
        private readonly ISenaryoRepository senaryoRepository;
        private readonly IAIKisiOzellikRepository ozellikRepository;
        private readonly ISenaryoAdimRepository adimRepository;
        private readonly IMapper mapper;

        public SenaryoService(
            ISenaryoRepository senaryoRepository,
            IAIKisiOzellikRepository ozellikRepository,
            ISenaryoAdimRepository adimRepository,
            IMapper mapper)
        {
            this.senaryoRepository = senaryoRepository;
            this.ozellikRepository = ozellikRepository;
            this.adimRepository = adimRepository;
            this.mapper = mapper;
        }

        public async Task<IEnumerable<SenaryoDto>> GetAllAsync()
        {
            var entities = await senaryoRepository.GetAllAsync(query => query
                .Include(e => e.Ozellikler)
                .Include(e => e.Adimlar));
            return mapper.Map<IEnumerable<SenaryoDto>>(entities);
        }

        public async Task<IEnumerable<SenaryoDto>> GetByOwnerAsync(Guid ownerId)
        {
            var entities = await senaryoRepository.GetByOwnerAsync(ownerId);
            return mapper.Map<IEnumerable<SenaryoDto>>(entities);
        }

        public async Task<SenaryoDto?> GetByIdAsync(Guid id)
        {
            var entity = await senaryoRepository.GetByIdAsync(id, query => query
                .Include(e => e.Ozellikler)
                .Include(e => e.Adimlar));
            if (entity == null)
            {
                return null;
            }

            return mapper.Map<SenaryoDto>(entity);
        }

        public async Task<SenaryoDto> CreateAsync(CreateSenaryoDto dto)
        {
            var entity = mapper.Map<Senaryo>(dto);
            entity.Id = Guid.NewGuid();

            await senaryoRepository.AddAsync(entity);
            await senaryoRepository.SaveChangesAsync();

            await SyncOzelliklerAsync(entity.Id, dto.Ozellikler);
            await SyncAdimlarAsync(entity.Id, dto.Adimlar);

            return await GetByIdAsync(entity.Id) ?? mapper.Map<SenaryoDto>(entity);
        }

        public async Task<SenaryoDto> UpdateAsync(Guid id, CreateSenaryoDto dto)
        {
            var entity = await senaryoRepository.GetByIdAsync(id, q => q
                .Include(e => e.Ozellikler)
                .Include(e => e.Adimlar));

            if (entity == null)
            {
                throw new ArgumentException("Senaryo bulunamadı", nameof(id));
            }

            entity.Ad = dto.Ad;
            entity.Aciklama = dto.Aciklama;

            await senaryoRepository.SaveChangesAsync();

            await SyncOzelliklerAsync(id, dto.Ozellikler);
            await SyncAdimlarAsync(id, dto.Adimlar);

            return await GetByIdAsync(id) ?? mapper.Map<SenaryoDto>(entity);
        }

        public async Task DeleteAsync(Guid id)
        {
            var entity = await senaryoRepository.GetByIdAsync(id);
            if (entity == null)
            {
                throw new ArgumentException("Senaryo bulunamadı", nameof(id));
            }

            senaryoRepository.Delete(entity);
            await senaryoRepository.SaveChangesAsync();
        }

        private async Task SyncOzelliklerAsync(Guid senaryoId, IEnumerable<AIKisiOzellikCreateDto> ozellikler)
        {
            var existing = await ozellikRepository.GetBySenaryoIdAsync(senaryoId);
            var toDelete = existing.Where(e => ozellikler.All(o => o.Id != e.Id)).ToList();
            if (toDelete.Any())
            {
                ozellikRepository.DeleteRange(toDelete);
            }

            foreach (var ozellikDto in ozellikler)
            {
                var entity = mapper.Map<AIKisiOzellik>(ozellikDto);
                entity.SenaryoId = senaryoId;
                entity.Id = ozellikDto.Id ?? Guid.NewGuid();
                await ozellikRepository.SyncAsync(entity);
            }

            await ozellikRepository.SaveChangesAsync();
        }

        private async Task SyncAdimlarAsync(Guid senaryoId, IEnumerable<SenaryoAdimCreateDto> adimlar)
        {
            var existing = await adimRepository.GetBySenaryoIdAsync(senaryoId);
            var toDelete = existing.Where(e => adimlar.All(o => o.Id != e.Id)).ToList();
            if (toDelete.Any())
            {
                adimRepository.DeleteRange(toDelete);
            }

            foreach (var adimDto in adimlar)
            {
                var entity = mapper.Map<SenaryoAdim>(adimDto);
                entity.SenaryoId = senaryoId;
                entity.Id = adimDto.Id ?? Guid.NewGuid();
                await adimRepository.SyncAsync(entity);
            }

            await adimRepository.SaveChangesAsync();
        }
    }
}
