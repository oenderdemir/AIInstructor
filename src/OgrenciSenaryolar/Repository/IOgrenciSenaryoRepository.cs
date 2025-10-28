using System;
using System.Collections.Generic;
using AIInstructor.src.OgrenciSenaryolar.Entity;
using AIInstructor.src.Shared.RDBMS.Repository;

namespace AIInstructor.src.OgrenciSenaryolar.Repository
{
    public interface IOgrenciSenaryoRepository : IBaseRepository<OgrenciSenaryo>
    {
        Task<IEnumerable<OgrenciSenaryo>> GetByOgrenciIdAsync(Guid ogrenciId);
    }
}
