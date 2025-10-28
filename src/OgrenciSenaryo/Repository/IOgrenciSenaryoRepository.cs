using System;
using System.Collections.Generic;
using AIInstructor.src.OgrenciSenaryo.Entity;
using AIInstructor.src.Shared.RDBMS.Repository;

namespace AIInstructor.src.OgrenciSenaryo.Repository
{
    public interface IOgrenciSenaryoRepository : IBaseRepository<OgrenciSenaryo>
    {
        Task<IEnumerable<OgrenciSenaryo>> GetByOgrenciIdAsync(Guid ogrenciId);
    }
}
