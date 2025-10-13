using System.ComponentModel.DataAnnotations;
using AIInstructor.src.KullaniciGruplar.DTO;
using AIInstructor.src.Shared.RDBMS.Dto;

namespace AIInstructor.src.Kullanicilar.DTO
{
    public class KullaniciDTO: BaseRDBMSDto
    {

        
        public string KullaniciAdi { get; set; }

        
        public string? TCNO { get; set; }


        public string? Ad { get; set; }

        public string? Soyad { get; set; }

        public string? Email { get; set; }


        
        public string? AvatarPath { get; set; }

        public enumKullaniciStatus Status { get; set; } 

        public List<KullaniciGrupDTO> KullaniciGruplar { get; set; }
    }
}
