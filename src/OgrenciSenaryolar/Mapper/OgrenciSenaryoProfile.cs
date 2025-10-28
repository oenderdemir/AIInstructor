using AutoMapper;
using AIInstructor.src.OgrenciSenaryolar.DTO;
using AIInstructor.src.OgrenciSenaryolar.Entity;

namespace AIInstructor.src.OgrenciSenaryolar.Mapper
{
    public class OgrenciSenaryoProfile : Profile
    {
        public OgrenciSenaryoProfile()
        {
            CreateMap<OgrenciSenaryo, OgrenciSenaryoDto>().ReverseMap();
        }
    }
}
