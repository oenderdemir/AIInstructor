using AIInstructor.src.KullaniciGruplar.DTO;
using AIInstructor.src.KullaniciGruplar.Entity;
using AIInstructor.src.Kullanicilar.DTO;
using AIInstructor.src.Shared.RDBMS.Dto;

namespace AIInstructor.src.KullaniciKullaniciGruplar.DTO
{
    public class KullaniciKullaniciGrupDTO: BaseRDBMSDto
    {
        public KullaniciDTO Kullanici { get; set; }
        public KullaniciGrupDTO KullaniciGrup { get; set; }
    }
}
