using System;
using System.Collections.Generic;
using AIInstructor.src.AIKisiOzellik.Entity;
using AIInstructor.src.Shared.RDBMS.Repository;

namespace AIInstructor.src.AIKisiOzellik.Repository
{
    public interface IAIKisiOzellikRepository : IBaseRepository<AIKisiOzellik>
    {
        Task<IEnumerable<AIKisiOzellik>> GetBySenaryoIdAsync(Guid senaryoId);
    }
}
