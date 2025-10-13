using AutoMapper;
using AIInstructor.src.MenuItemler.DTO;
using AIInstructor.src.MenuItemler.Entity;

namespace AIInstructor.src.MenuItemler.Mapper
{
    public class MenuItemProfile : Profile
    {
        public MenuItemProfile()
        {
            CreateMap<MenuItem, MenuItemDto>()
            .ForMember(dest => dest.ParentId, opt => opt.MapFrom(src => src.Parent != null ? src.Parent.Id : (Guid?)null))
            .ForMember(dest => dest.Roles, opt => opt.MapFrom(src => src.MenuItemRoller.Select(x => x.Rol)))
            .ReverseMap()
            .ForMember(dest => dest.Parent, opt => opt.Ignore())
            .ForMember(dest => dest.MenuItemRoller, opt => opt.Ignore());
        }
    }
}
