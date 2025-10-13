using AIInstructor.src.Shared.RDBMS.Repository;
using AIInstructor.src.TrainingScenarios.Entity;

namespace AIInstructor.src.TrainingScenarios.Repository
{
    public interface IEarnedBadgeRepository : IBaseRepository<EarnedBadge>
    {
        Task<bool> HasBadgeAsync(Guid profileId, string badgeKey);
    }
}
