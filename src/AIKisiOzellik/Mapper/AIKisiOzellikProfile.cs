using AutoMapper;
using AIInstructor.src.AIKisiOzellik.DTO;
using AIInstructor.src.AIKisiOzellik.Entity;

namespace AIInstructor.src.AIKisiOzellik.Mapper
{
    public class AIKisiOzellikProfile : Profile
    {
        public AIKisiOzellikProfile()
        {
            CreateMap<AIKisiOzellik, AIKisiOzellikDto>().ReverseMap();
            CreateMap<AIKisiOzellikCreateDto, AIKisiOzellik>();
        }
    }
}
