using AutoMapper;
using AIInstructor.src.Kullanicilar.DTO;
using AIInstructor.src.Kullanicilar.Entity;
using AIInstructor.src.Kullanicilar.Repository;
using AIInstructor.src.Roller.DTO;
using AIInstructor.src.Roller.Entity;
using AIInstructor.src.Roller.Service;
using AIInstructor.src.Shared.RDBMS.Service;
using AIInstructor.src.Shared.Repository;

namespace AIInstructor.src.Kullanicilar.Service
{
    public class KullaniciService : BaseService<KullaniciDTO, Kullanici>, IKullaniciService
    {
        private readonly IKullaniciRepository kullaniciRepository;
        public KullaniciService(IKullaniciRepository kullaniciRepository, IMapper mapper)
           : base(kullaniciRepository, mapper)
        {
            this.kullaniciRepository = kullaniciRepository;
        }
    }
}
