using AutoMapper;
using AIInstructor.src.SenaryoAdimlar.DTO;
using AIInstructor.src.SenaryoAdimlar.Entity;

namespace AIInstructor.src.SenaryoAdimlar.Mapper
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
