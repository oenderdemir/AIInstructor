
using AIInstructor.src.Kullanicilar.Entity;
using AIInstructor.src.Shared.RDBMS.Repository;

namespace AIInstructor.src.Kullanicilar.Repository
{
    public interface IKullaniciRepository : IBaseRepository<Kullanici>
    {
        Task<Kullanici> GetByKullaniciAdiAsync(string kullaniciAdi);
        Task<Kullanici?> GetByEmailAsync(string email);
    }
}
