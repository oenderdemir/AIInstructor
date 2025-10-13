using AutoMapper;
using AIInstructor.src.Roller.DTO;
using AIInstructor.src.Roller.Entity;

namespace AIInstructor.src.Roller.Mapper
{
    public class RollerProfile : Profile
    {
        public RollerProfile()
        {
            CreateMap<Rol, RolDto>().ReverseMap();
        }
    }
}
