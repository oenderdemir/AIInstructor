using AutoMapper;
using Microsoft.EntityFrameworkCore;
using AIInstructor.src.KullaniciKullaniciGruplar.DTO;
using AIInstructor.src.KullaniciKullaniciGruplar.Entitiy;
using AIInstructor.src.KullaniciKullaniciGruplar.Repository;
using AIInstructor.src.Shared.RDBMS.Service;

namespace AIInstructor.src.KullaniciKullaniciGruplar.Service
{
    public class KullaniciKullaniciGrupService:BaseService<KullaniciKullaniciGrupDTO,KullaniciKullaniciGrup>,IKullaniciKullaniciGrupService
    {
        private readonly IKullaniciKullaniciGrupRepository kullaniciKullaniciGrupRepository;
        public KullaniciKullaniciGrupService(IKullaniciKullaniciGrupRepository kullaniciKullaniciGrupRepository,IMapper mapper)
            :base(kullaniciKullaniciGrupRepository,mapper)
        {
            this.kullaniciKullaniciGrupRepository = kullaniciKullaniciGrupRepository;
        }

        public override async Task<IEnumerable<KullaniciKullaniciGrupDTO>> GetAllAsync(Func<IQueryable<KullaniciKullaniciGrup>, IQueryable<KullaniciKullaniciGrup>>? include = null)
        {
            return await  base.GetAllAsync(q => q.Include(x => x.Kullanici).Include(x => x.KullaniciGrup));
        }
    }
}
