using AIInstructor.src.KullaniciGruplar.Entity;
using AIInstructor.src.Kullanicilar.Entity;
using AIInstructor.src.Shared.RDBMS.Entity;

namespace AIInstructor.src.KullaniciKullaniciGruplar.Entitiy
{
    public class KullaniciKullaniciGrup:BaseEntity
    {
        public Kullanici Kullanici { get; set; }
        public KullaniciGrup KullaniciGrup { get; set; }
    }
}
