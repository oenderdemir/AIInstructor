using AutoMapper;
using AIInstructor.src.AIKisiOzellikler.DTO;
using AIInstructor.src.AIKisiOzellikler.Entity;

namespace AIInstructor.src.AIKisiOzellikler.Mapper
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
