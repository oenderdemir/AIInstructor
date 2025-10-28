using System;
using System.Collections.Generic;
using AIInstructor.src.AIKisiOzellikler.Entity;
using AIInstructor.src.Shared.RDBMS.Repository;

namespace AIInstructor.src.AIKisiOzellikler.Repository
{
    public interface IAIKisiOzellikRepository : IBaseRepository<AIKisiOzellik>
    {
        Task<IEnumerable<AIKisiOzellik>> GetBySenaryoIdAsync(Guid senaryoId);
    }
}
