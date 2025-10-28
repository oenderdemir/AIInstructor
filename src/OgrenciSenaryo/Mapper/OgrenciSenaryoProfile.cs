using AutoMapper;
using AIInstructor.src.OgrenciSenaryo.DTO;
using AIInstructor.src.OgrenciSenaryo.Entity;

namespace AIInstructor.src.OgrenciSenaryo.Mapper
{
    public class OgrenciSenaryoProfile : Profile
    {
        public OgrenciSenaryoProfile()
        {
            CreateMap<OgrenciSenaryo, OgrenciSenaryoDto>().ReverseMap();
        }
    }
}
