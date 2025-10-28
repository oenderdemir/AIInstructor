using AutoMapper;
using AIInstructor.src.Context;
using AIInstructor.src.Gamification.Entity;
using AIInstructor.src.Shared.RDBMS.Repository;

namespace AIInstructor.src.Gamification.Repository
{
    public class GamificationResultRepository : BaseRepository<GamificationResult>, IGamificationResultRepository
    {
        public GamificationResultRepository(VTSDbContext context, IMapper mapper) : base(context, mapper)
        {
        }
    }
}
