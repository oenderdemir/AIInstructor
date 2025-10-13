using AutoMapper;
using Microsoft.EntityFrameworkCore;
using AIInstructor.src.KullaniciGruplar.DTO;
using AIInstructor.src.KullaniciGruplar.Entity;
using AIInstructor.src.KullaniciGruplar.Repository;
using AIInstructor.src.KullaniciGrupRoller.Entity;
using AIInstructor.src.Kullanicilar.Repository;
using AIInstructor.src.MenuItemler.DTO;
using AIInstructor.src.Shared.RDBMS.Service;
using AIInstructor.src.Shared.Repository;

namespace AIInstructor.src.KullaniciGruplar.Service
{
    public class KullaniciGrupService:BaseService<KullaniciGrupDTO,KullaniciGrup>,IKullaniciGrupService
    {
        private readonly IKullaniciGrupRepository kullaniciGrupRepository;
        private readonly IRolRepository rolRepository;
        public KullaniciGrupService(IKullaniciGrupRepository kullaniciGrupRepository
            ,IRolRepository rolRepository
            ,IMapper mapper) 
            : base(kullaniciGrupRepository, mapper)
        { 
            this.kullaniciGrupRepository = kullaniciGrupRepository;
            this.rolRepository = rolRepository;
        }


        public override async Task<KullaniciGrupDTO> AddAsync(KullaniciGrupDTO dto)
        {
            var kullaniciGrup = _mapper.Map<KullaniciGrup>(dto);

           
            // Gelen Rolleri işle
            kullaniciGrup.KullaniciGrupRoller = new List<KullaniciGrupRol>();

            foreach (var roleDto in dto.Roller)
            {
                var rol = await this.rolRepository.GetByIdAsync(roleDto.Id.Value);
                kullaniciGrup.KullaniciGrupRoller.Add(new KullaniciGrupRol
                {
                    KullaniciGrup = kullaniciGrup,
                    Rol = rol
                });
            }

            await _repository.AddAsync(kullaniciGrup);
            await _repository.SaveChangesAsync();

            return _mapper.Map<KullaniciGrupDTO>(kullaniciGrup);
        }

        public override async Task<KullaniciGrupDTO> UpdateAsync(KullaniciGrupDTO dto)
        {
            var kullaniciGrup = await _repository.GetByIdAsync(dto.Id.Value, q => q.Include(x => x.KullaniciGrupRoller));

            if (kullaniciGrup == null)
                throw new Exception("MenuItem bulunamadı!");

            // 1. Alanları güncelle
            kullaniciGrup.Ad = dto.Ad;
          
         

            // 2. Mevcut MenuItemRoller temizle
            if (kullaniciGrup.KullaniciGrupRoller != null && kullaniciGrup.KullaniciGrupRoller.Any())
            {
                this.kullaniciGrupRepository.RemoveKullaniciGrupRollerRangeAsync(kullaniciGrup.KullaniciGrupRoller);
            }

            // 3. Yeni roller ekle
            if (dto.Roller != null && dto.Roller.Any())
            {
                kullaniciGrup.KullaniciGrupRoller = new List<KullaniciGrupRol>();
                var roleIds = dto.Roller.Select(e => e.Id).Distinct().ToList();
                foreach (var roleId in roleIds)
                {
                    var rol = await this.rolRepository.GetByIdAsync(roleId.Value);
                    kullaniciGrup.KullaniciGrupRoller.Add(new KullaniciGrupRol
                    {
                        KullaniciGrup = kullaniciGrup,
                        Rol = rol
                    });
                }
            }

            _repository.Update(kullaniciGrup);
            await _repository.SaveChangesAsync();
            return dto;
        }
    }
}
