using AutoMapper;
using AIInstructor.src.Kullanicilar.DTO;
using AIInstructor.src.Kullanicilar.Entity;


namespace AIInstructor.src.Kullanicilar.Mapper
{

    public class KullaniciProfile : Profile
    {

        public KullaniciProfile()
        {
            CreateMap<Kullanici, KullaniciDTO>()
            .ForMember(dest => dest.KullaniciGruplar,
                opt =>
                {
                    opt.Condition(src => src.KullaniciKullaniciGruplar != null);
                    opt.MapFrom(src => src.KullaniciKullaniciGruplar.Select(s => s.KullaniciGrup));
                })
            .ReverseMap()
            .ForMember(dest => dest.KullaniciKullaniciGruplar, opt => opt.Ignore());
        }
    }
}
