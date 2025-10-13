using AIInstructor.src.Roller.DTO;
using AIInstructor.src.Shared.RDBMS.Dto;

namespace AIInstructor.src.KullaniciGruplar.DTO
{
    public class KullaniciGrupDTO : BaseRDBMSDto
    {

        public string Ad { get;set; }

        public List<RolDto>? Roller { get; set; } = new();
    }
}
