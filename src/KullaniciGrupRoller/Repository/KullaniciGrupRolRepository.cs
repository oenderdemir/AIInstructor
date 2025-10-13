using AutoMapper;
using AIInstructor.src.Context;
using AIInstructor.src.KullaniciGrupRoller.Entity;
using AIInstructor.src.Shared.RDBMS.Repository;

namespace AIInstructor.src.KullaniciGrupRoller.Repository
{
    public class KullaniciGrupRolRepository:BaseRepository<KullaniciGrupRol>,IKullaniciGrupRolRepository
    {
        public KullaniciGrupRolRepository(VTSDbContext context, IMapper mapper):base(context, mapper) 
        {

        }
    }
}
