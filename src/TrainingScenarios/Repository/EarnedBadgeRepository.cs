using AutoMapper;
using Microsoft.EntityFrameworkCore;
using AIInstructor.src.Context;
using AIInstructor.src.Shared.RDBMS.Repository;
using AIInstructor.src.TrainingScenarios.Entity;

namespace AIInstructor.src.TrainingScenarios.Repository
{
    public class EarnedBadgeRepository : BaseRepository<EarnedBadge>, IEarnedBadgeRepository
    {
        public EarnedBadgeRepository(VTSDbContext context, IMapper mapper)
            : base(context, mapper)
        {
        }

        public async Task<bool> HasBadgeAsync(Guid profileId, string badgeKey)
        {
            return await _context.EarnedBadges
                .AnyAsync(badge => badge.GamificationProfileId == profileId && badge.BadgeKey == badgeKey);
        }
    }
}
