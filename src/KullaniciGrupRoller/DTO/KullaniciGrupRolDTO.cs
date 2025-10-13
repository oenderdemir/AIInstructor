using AIInstructor.src.KullaniciGruplar.DTO;
using AIInstructor.src.Roller.DTO;
using AIInstructor.src.Shared.RDBMS.Dto;


namespace AIInstructor.src.KullaniciGrupRoller.DTO
{
    public class KullaniciGrupRolDTO: BaseRDBMSDto
    {

        public KullaniciGrupDTO KullaniciGrup { get; set; }
        public RolDto Rol { get; set; }

    }
}
