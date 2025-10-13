
using AIInstructor.src.KullaniciGruplar.Entity;
using AIInstructor.src.KullaniciGrupRoller.Entity;
using AIInstructor.src.Shared.RDBMS.Repository;

namespace AIInstructor.src.KullaniciGruplar.Repository
{
    public interface IKullaniciGrupRepository : IBaseRepository<KullaniciGrup>
    {
        void RemoveKullaniciGrupRollerRangeAsync(IEnumerable<KullaniciGrupRol> kullaniciGrupRoller);
    }
}
