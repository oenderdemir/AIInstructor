using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using AIInstructor.src.Context;
using AIInstructor.src.OgrenciSenaryolar.Entity;
using AIInstructor.src.Shared.RDBMS.Repository;

namespace AIInstructor.src.OgrenciSenaryolar.Repository
{
    public class OgrenciSenaryoRepository : BaseRepository<OgrenciSenaryo>, IOgrenciSenaryoRepository
    {
        public OgrenciSenaryoRepository(VTSDbContext context, IMapper mapper) : base(context, mapper)
        {
        }

        public async Task<IEnumerable<OgrenciSenaryo>> GetByOgrenciIdAsync(Guid ogrenciId)
        {
            return await _dbSet.Where(e => e.OgrenciId == ogrenciId).ToListAsync();
        }
    }
}
