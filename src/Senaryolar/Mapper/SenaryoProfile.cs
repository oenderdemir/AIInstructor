using AutoMapper;
using AIInstructor.src.Senaryolar.DTO;
using AIInstructor.src.Senaryolar.Entity;

namespace AIInstructor.src.Senaryolar.Mapper
{
    public class SenaryoProfile : Profile
    {
        public SenaryoProfile()
        {
            CreateMap<Senaryo, SenaryoDto>().ReverseMap();
            CreateMap<CreateSenaryoDto, Senaryo>();
        }
    }
}
