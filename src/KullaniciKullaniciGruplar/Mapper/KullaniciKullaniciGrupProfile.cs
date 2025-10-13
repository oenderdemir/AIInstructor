using AutoMapper;
using AIInstructor.src.KullaniciGruplar.DTO;
using AIInstructor.src.KullaniciGruplar.Entity;
using AIInstructor.src.KullaniciKullaniciGruplar.DTO;
using AIInstructor.src.KullaniciKullaniciGruplar.Entitiy;

namespace AIInstructor.src.KullaniciKullaniciGruplar.Mapper
{
    public class KullaniciKullaniciGrupProfile:Profile
    {
        public KullaniciKullaniciGrupProfile()
        {
            CreateMap<KullaniciKullaniciGrup, KullaniciKullaniciGrupDTO>().ReverseMap();
        }
        
    }
}
