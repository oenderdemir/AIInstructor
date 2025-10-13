using AIInstructor.src.KullaniciGrupRoller.Entity;
using AIInstructor.src.Shared.RDBMS.Entity;

namespace AIInstructor.src.KullaniciGruplar.Entity
{
    public class KullaniciGrup:BaseEntity
    {
        public string Ad { get; set; }
        public ICollection<KullaniciGrupRol> KullaniciGrupRoller { get; set; } = new List<KullaniciGrupRol>();
    }
}
