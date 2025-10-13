using AutoMapper;
using System.Collections.Generic;
using System.Threading.Tasks;
using AIInstructor.src.Roller.DTO;
using AIInstructor.src.Roller.Entity;
using AIInstructor.src.Roller.Service;


using AIInstructor.src.Shared.RDBMS.Service;
using AIInstructor.src.Shared.Repository;

namespace AIInstructor.src.Shared.Service
{
    public class RolService : BaseService<RolDto, Rol>, IRolService
    {
        private readonly IRolRepository _rolRepository;

        public RolService(IRolRepository rolRepository, IMapper mapper)
            : base(rolRepository, mapper)
        {
            _rolRepository = rolRepository;
        }

        public async Task<RolDto> GetByNameAsync(string name)
        {
            //_rolRepository.Where(e=>e.Ad.Equals(name));
            var entity = await _rolRepository.GetByNameAsync(name);
            return _mapper.Map<RolDto>(entity);
        }

        public async Task<IEnumerable<RolDto>> ViewRolleriniGetir()
        {
            var entities = await _rolRepository.ViewRolleriniGetir();
            return _mapper.Map<IEnumerable<RolDto>>(entities);
        }
    }

}
