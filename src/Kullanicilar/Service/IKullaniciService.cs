using AIInstructor.src.Kullanicilar.DTO;
using AIInstructor.src.Kullanicilar.Entity;
using AIInstructor.src.Roller.DTO;
using AIInstructor.src.Shared.RDBMS.Service;

namespace AIInstructor.src.Kullanicilar.Service
{
    public interface IKullaniciService : IBaseService<KullaniciDTO,Kullanici>
    {
    }
}
