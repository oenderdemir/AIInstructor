using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using AIInstructor.src.Context;
using AIInstructor.src.SenaryoAdimlar.Entity;
using AIInstructor.src.Shared.RDBMS.Repository;

namespace AIInstructor.src.SenaryoAdimlar.Repository
{
    public class SenaryoAdimRepository : BaseRepository<SenaryoAdim>, ISenaryoAdimRepository
    {
        public SenaryoAdimRepository(VTSDbContext context, IMapper mapper) : base(context, mapper)
        {
        }

        public async Task<IReadOnlyList<SenaryoAdim>> GetBySenaryoIdAsync(Guid senaryoId)
        {
            return await _dbSet.Where(e => e.SenaryoId == senaryoId).OrderBy(e => e.SiraNo).ToListAsync();
        }
    }
}
