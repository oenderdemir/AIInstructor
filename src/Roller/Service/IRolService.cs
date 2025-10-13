using AIInstructor.src.Roller.DTO;
using AIInstructor.src.Roller.Entity;
using AIInstructor.src.Shared.RDBMS.Service;

namespace AIInstructor.src.Roller.Service
{
    public interface IRolService : IBaseService<RolDto,Rol>
    {
        Task<RolDto> GetByNameAsync(string name);

        Task<IEnumerable<RolDto>> ViewRolleriniGetir();
    }
}
