using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using AIInstructor.src.AIKisiOzellikler.Entity;
using AIInstructor.src.Context;
using AIInstructor.src.Shared.RDBMS.Repository;

namespace AIInstructor.src.AIKisiOzellikler.Repository
{
    public class AIKisiOzellikRepository : BaseRepository<AIKisiOzellik>, IAIKisiOzellikRepository
    {
        public AIKisiOzellikRepository(VTSDbContext context, IMapper mapper) : base(context, mapper)
        {
        }

        public async Task<IEnumerable<AIKisiOzellik>> GetBySenaryoIdAsync(Guid senaryoId)
        {
            return await _dbSet.Where(e => e.SenaryoId == senaryoId).ToListAsync();
        }
    }
}
