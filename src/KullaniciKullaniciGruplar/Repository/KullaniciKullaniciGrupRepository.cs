using AutoMapper;
using AIInstructor.src.Context;
using AIInstructor.src.KullaniciKullaniciGruplar.Entitiy;
using AIInstructor.src.Shared.RDBMS.Repository;

namespace AIInstructor.src.KullaniciKullaniciGruplar.Repository
{
    public class KullaniciKullaniciGrupRepository:BaseRepository<KullaniciKullaniciGrup>,IKullaniciKullaniciGrupRepository
    {
        public KullaniciKullaniciGrupRepository(VTSDbContext context, IMapper mapper):base(context, mapper) 
        { 
        
        }
        
    }
}
