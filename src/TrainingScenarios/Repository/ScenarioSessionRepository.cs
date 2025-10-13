using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using AIInstructor.src.Context;
using AIInstructor.src.Shared.RDBMS.Repository;
using AIInstructor.src.TrainingScenarios.Entity;

namespace AIInstructor.src.TrainingScenarios.Repository
{
    public class ScenarioSessionRepository : BaseRepository<ScenarioSession>, IScenarioSessionRepository
    {
        public ScenarioSessionRepository(VTSDbContext context, IMapper mapper)
            : base(context, mapper)
        {
        }

        public async Task<IReadOnlyList<ScenarioSession>> GetActiveSessionsForStudentAsync(Guid studentId)
        {
            return await _context.ScenarioSessions
                .Include(s => s.Scenario)
                .Where(s => s.StudentId == studentId && !s.IsCompleted)
                .ToListAsync();
        }

        public async Task<ScenarioSession?> GetSessionWithDetailsAsync(Guid sessionId)
        {
            return await _context.ScenarioSessions
                .Include(s => s.Scenario)
                .Include(s => s.Messages)
                .Include(s => s.Evaluation)
                .FirstOrDefaultAsync(s => s.Id == sessionId);
        }
    }
}
