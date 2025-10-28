using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using AIInstructor.src.Context;
using AIInstructor.src.Senaryolar.Entity;
using AIInstructor.src.Shared.RDBMS.Repository;

namespace AIInstructor.src.Senaryolar.Repository
{
    public class SenaryoRepository : BaseRepository<Senaryo>, ISenaryoRepository
    {
        public SenaryoRepository(VTSDbContext context, IMapper mapper) : base(context, mapper)
        {
        }

        public async Task<IEnumerable<Senaryo>> GetByOwnerAsync(Guid kullaniciId)
        {
            return await _dbSet.Where(e => e.OlusturanKullaniciId == kullaniciId).ToListAsync();
        }
    }
}
