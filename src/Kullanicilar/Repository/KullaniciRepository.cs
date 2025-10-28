using AutoMapper;
using AIInstructor.src.Context;
using AIInstructor.src.Kullanicilar.Entity;
using AIInstructor.src.Roller.Entity;
using AIInstructor.src.Shared.RDBMS.Repository;

namespace AIInstructor.src.Kullanicilar.Repository
{
    public class KullaniciRepository : BaseRepository<Kullanici>, IKullaniciRepository
    {
        public KullaniciRepository(VTSDbContext context, IMapper mapper)
            : base(context, mapper)
        {
        }

        public async Task<Kullanici> GetByKullaniciAdiAsync(string kullaniciAdi)
        {
            return await base.FirstOrDefaultAsync(e=>e.KullaniciAdi.Equals(kullaniciAdi));
        }

        public async Task<Kullanici?> GetByEmailAsync(string email)
        {
            return await base.FirstOrDefaultAsync(e => e.Email != null && e.Email.Equals(email));
        }
    }
}
