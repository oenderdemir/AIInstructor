using AutoMapper;
using AIInstructor.src.KullaniciGruplar.DTO;
using AIInstructor.src.KullaniciGruplar.Entity;


namespace AIInstructor.src.KullaniciGruplar.Mapper
{
    public class KullaniciGrupProfile:Profile
    {
        public KullaniciGrupProfile()
        {
            CreateMap<KullaniciGrup, KullaniciGrupDTO>()
                .ForMember(dest => dest.Roller, opt => opt.MapFrom(src => src.KullaniciGrupRoller.Select(x => x.Rol)))
                .ReverseMap()
                .ForMember(dest => dest.KullaniciGrupRoller, opt => opt.Ignore()); ;
        }
    }
}
