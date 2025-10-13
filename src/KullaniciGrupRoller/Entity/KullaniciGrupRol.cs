using AIInstructor.src.KullaniciGruplar.Entity;
using AIInstructor.src.Roller.Entity;
using AIInstructor.src.Shared.RDBMS.Entity;

namespace AIInstructor.src.KullaniciGrupRoller.Entity
{
    public class KullaniciGrupRol:BaseEntity
    {
        public KullaniciGrup KullaniciGrup { get; set; }
        public Rol Rol { get; set; }
    }
}
