using System;
using System.Collections.Generic;
using AIInstructor.src.Senaryolar.Entity;
using AIInstructor.src.Shared.RDBMS.Repository;

namespace AIInstructor.src.Senaryolar.Repository
{
    public interface ISenaryoRepository : IBaseRepository<Senaryo>
    {
        Task<IEnumerable<Senaryo>> GetByOwnerAsync(Guid kullaniciId);
    }
}
