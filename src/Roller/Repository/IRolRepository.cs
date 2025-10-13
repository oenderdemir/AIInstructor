using System.Threading.Tasks;
using AIInstructor.src.MenuItemRoller.Entity;
using AIInstructor.src.Roller.Entity;

using AIInstructor.src.Shared.RDBMS.Repository;

namespace AIInstructor.src.Shared.Repository
{
    public interface IRolRepository : IBaseRepository<Rol>
    {
        Task<Rol> GetByNameAsync(string name);
        Task<IEnumerable<Rol>> ViewRolleriniGetir();

        
    }
}
