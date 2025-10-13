using AutoMapper;
using Microsoft.EntityFrameworkCore;
using AIInstructor.src.Context;
using AIInstructor.src.Shared.RDBMS.Repository;
using AIInstructor.src.TrainingScenarios.Entity;

namespace AIInstructor.src.TrainingScenarios.Repository
{
    public class TrainingScenarioRepository : BaseRepository<TrainingScenario>, ITrainingScenarioRepository
    {
        public TrainingScenarioRepository(VTSDbContext context, IMapper mapper)
            : base(context, mapper)
        {
        }

        public async Task<TrainingScenario?> GetWithSessionsAsync(Guid id)
        {
            return await _context.TrainingScenarios
                .Include(s => s.Sessions)
                .ThenInclude(session => session.Messages)
                .FirstOrDefaultAsync(s => s.Id == id);
        }
    }
}
