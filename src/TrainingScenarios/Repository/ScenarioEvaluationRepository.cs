using AutoMapper;
using Microsoft.EntityFrameworkCore;
using AIInstructor.src.Context;
using AIInstructor.src.Shared.RDBMS.Repository;
using AIInstructor.src.TrainingScenarios.Entity;

namespace AIInstructor.src.TrainingScenarios.Repository
{
    public class ScenarioEvaluationRepository : BaseRepository<ScenarioEvaluation>, IScenarioEvaluationRepository
    {
        public ScenarioEvaluationRepository(VTSDbContext context, IMapper mapper)
            : base(context, mapper)
        {
        }

        public async Task<ScenarioEvaluation?> GetBySessionIdAsync(Guid sessionId)
        {
            return await _context.ScenarioEvaluations.FirstOrDefaultAsync(evaluation => evaluation.SessionId == sessionId);
        }
    }
}
