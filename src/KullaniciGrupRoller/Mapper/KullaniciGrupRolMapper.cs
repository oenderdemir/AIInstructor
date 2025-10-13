using AutoMapper;
using AIInstructor.src.KullaniciGrupRoller.DTO;
using AIInstructor.src.KullaniciGrupRoller.Entity;


namespace AIInstructor.src.KullaniciGrupRoller.Mapper
{
    public class KullaniciGrupRolMapper:Profile
    {
        public KullaniciGrupRolMapper() 
        {
            CreateMap<KullaniciGrupRol, KullaniciGrupRolDTO>().ReverseMap();
        }
    }
}
