using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Options;
using AIInstructor.src.Gamification.DTO;
using AIInstructor.src.Gamification.Entity;
using AIInstructor.src.Gamification.Settings;
using AIInstructor.src.Gamification.Repository;

namespace AIInstructor.src.Gamification.Service
{
    public interface IGamificationResultService
    {
        Task<GamificationResultDto> CalculateAsync(Guid ogrenciSenaryoId, IEnumerable<string> successCriteriaMet);
        Task SaveResultAsync(GamificationResultDto result);
        Task<GamificationResultDto?> GetResultAsync(Guid ogrenciSenaryoId);
    }

    public class GamificationService : IGamificationResultService
    {
        private readonly IGamificationResultRepository repository;
        private readonly GamificationSettings settings;

        public GamificationService(IGamificationResultRepository repository, IOptions<GamificationSettings> options)
        {
            this.repository = repository;
            this.settings = options.Value;
        }

        public async Task<GamificationResultDto> CalculateAsync(Guid ogrenciSenaryoId, IEnumerable<string> successCriteriaMet)
        {
            var metCriteria = successCriteriaMet?.ToList() ?? new List<string>();
            var puan = metCriteria.Count * settings.PointPerSuccess;
            var badgeEntry = settings.BadgeThresholds
                .OrderByDescending(t => t.Value)
                .FirstOrDefault(t => puan >= t.Value);
            var badge = string.IsNullOrWhiteSpace(badgeEntry.Key) ? settings.DefaultBadge : badgeEntry.Key;

            return new GamificationResultDto
            {
                Id = Guid.NewGuid(),
                OgrenciSenaryoId = ogrenciSenaryoId,
                Puan = puan,
                Badge = badge,
                Detay = string.Join(',', metCriteria)
            };
        }

        public async Task SaveResultAsync(GamificationResultDto result)
        {
            var entity = new GamificationResult
            {
                Id = result.Id ?? Guid.NewGuid(),
                OgrenciSenaryoId = result.OgrenciSenaryoId,
                Puan = result.Puan,
                Badge = result.Badge,
                Detay = result.Detay
            };

            await repository.SyncAsync(entity);
            await repository.SaveChangesAsync();
        }

        public async Task<GamificationResultDto?> GetResultAsync(Guid ogrenciSenaryoId)
        {
            var entity = await repository.FirstOrDefaultAsync(e => e.OgrenciSenaryoId == ogrenciSenaryoId);
            if (entity == null)
            {
                return null;
            }

            return new GamificationResultDto
            {
                Id = entity.Id,
                OgrenciSenaryoId = entity.OgrenciSenaryoId,
                Puan = entity.Puan,
                Badge = entity.Badge,
                Detay = entity.Detay
            };
        }
    }
}
