using AutoMapper;
using AIInstructor.src.Context;
using AIInstructor.src.KullaniciGruplar.Entity;
using AIInstructor.src.KullaniciGrupRoller.Entity;
using AIInstructor.src.Shared.RDBMS.Repository;

namespace AIInstructor.src.KullaniciGruplar.Repository
{
    public class KullaniciGrupRepository:BaseRepository<KullaniciGrup>,IKullaniciGrupRepository
    {
        private readonly VTSDbContext context;
        public KullaniciGrupRepository(VTSDbContext context, IMapper mapper)
            : base(context, mapper)
        {
            this.context= context;
        }

        public void RemoveKullaniciGrupRollerRangeAsync(IEnumerable<KullaniciGrupRol> kullaniciGrupRoller)
        {
            this.context.KullaniciGrupRoller.RemoveRange(kullaniciGrupRoller);
        }
    }
}
