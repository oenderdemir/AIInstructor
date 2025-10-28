using AutoMapper;
using AIInstructor.src.SenaryoAdim.DTO;
using AIInstructor.src.SenaryoAdim.Entity;

namespace AIInstructor.src.SenaryoAdim.Mapper
{
    public class SenaryoAdimProfile : Profile
    {
        public SenaryoAdimProfile()
        {
            CreateMap<SenaryoAdim, SenaryoAdimDto>().ReverseMap();
            CreateMap<SenaryoAdimCreateDto, SenaryoAdim>();
        }
    }
}
