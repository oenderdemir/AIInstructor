using System;
using System.Collections.Generic;
using AIInstructor.src.SenaryoAdimlar.Entity;
using AIInstructor.src.Shared.RDBMS.Repository;

namespace AIInstructor.src.SenaryoAdimlar.Repository
{
    public interface ISenaryoAdimRepository : IBaseRepository<SenaryoAdim>
    {
        Task<IReadOnlyList<SenaryoAdim>> GetBySenaryoIdAsync(Guid senaryoId);
    }
}
