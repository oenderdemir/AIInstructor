using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using AIInstructor.src.Context;
using AIInstructor.src.Shared.RDBMS.Repository;
using AIInstructor.src.TrainingScenarios.Entity;

namespace AIInstructor.src.TrainingScenarios.Repository
{
    public class ScenarioMessageRepository : BaseRepository<ScenarioMessage>, IScenarioMessageRepository
    {
        public ScenarioMessageRepository(VTSDbContext context, IMapper mapper)
            : base(context, mapper)
        {
        }

        public async Task<IReadOnlyList<ScenarioMessage>> GetBySessionAsync(Guid sessionId)
        {
            return await _context.ScenarioMessages
                .Where(message => message.SessionId == sessionId)
                .OrderBy(message => message.Sequence)
                .ToListAsync();
        }
    }
}
