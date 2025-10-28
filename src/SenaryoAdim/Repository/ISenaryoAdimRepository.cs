using System;
using System.Collections.Generic;
using AIInstructor.src.SenaryoAdim.Entity;
using AIInstructor.src.Shared.RDBMS.Repository;

namespace AIInstructor.src.SenaryoAdim.Repository
{
    public interface ISenaryoAdimRepository : IBaseRepository<SenaryoAdim>
    {
        Task<IReadOnlyList<SenaryoAdim>> GetBySenaryoIdAsync(Guid senaryoId);
    }
}
