using AutoMapper;
using Microsoft.EntityFrameworkCore;
using AIInstructor.src.Context;
using AIInstructor.src.MenuItemRoller.Entity;
using AIInstructor.src.Roller.Entity;
using AIInstructor.src.Shared.RDBMS.Repository;
using AIInstructor.src.Shared.Repository;

namespace AIInstructor.src.Roller.Repository
{
    public class RolRepository : BaseRepository<Rol>, IRolRepository
    {
        private readonly VTSDbContext context;
        public RolRepository(VTSDbContext context, IMapper mapper)
            : base(context, mapper)
        {
            this.context = context;
        }     

        public async Task<Rol> GetByNameAsync(string name)
        {            
            return await base.FirstOrDefaultAsync(r => r.Ad == name);
        }

      

        public async  Task<IEnumerable<Rol>> ViewRolleriniGetir()
        {
            return await base.Where(e => e.Ad.Equals("View")).ToListAsync();
        }
    }
}

