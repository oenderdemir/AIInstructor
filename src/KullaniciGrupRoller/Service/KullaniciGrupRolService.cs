using AutoMapper;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using AIInstructor.src.KullaniciGrupRoller.DTO;
using AIInstructor.src.KullaniciGrupRoller.Entity;
using AIInstructor.src.KullaniciGrupRoller.Repository;
using AIInstructor.src.Shared.RDBMS.Service;

namespace AIInstructor.src.KullaniciGrupRoller.Service
{
    public class KullaniciGrupRolService:BaseService<KullaniciGrupRolDTO,KullaniciGrupRol>,IKullaniciGrupRolService
    {
        private readonly IKullaniciGrupRolRepository kullaniciGrupRolRepository;
        public KullaniciGrupRolService(IKullaniciGrupRolRepository kullaniciGrupRolRepository,IMapper mapper)
            :base(kullaniciGrupRolRepository,mapper)
        {
            this.kullaniciGrupRolRepository = kullaniciGrupRolRepository;
        }
    }
}
