using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using AIInstructor.src.KullaniciGrupRoller.Entity;
using AIInstructor.src.KullaniciGruplar.Entity;
using AIInstructor.src.KullaniciKullaniciGruplar.Entitiy;
using AIInstructor.src.Shared.RDBMS.Entity;

namespace AIInstructor.src.Kullanicilar.Entity
{
    public class Kullanici : BaseEntity
    {


        [Required]
        public string KullaniciAdi { get; set; } = string.Empty;
        
        [MaxLength(11)]
        public string? TCNO { get; set; }


        public string? Ad { get; set; }

        public string? Soyad { get; set; }

        public string? Email { get; set; }


        //1
        public string Parola { get; set; } = "4DFF4EA340F0A823F15D3F4F01AB62EAE0E5DA579CCB851F8DB9DFE84C58B2B37B89903A740E1EE172DA793A6E79D560E5F7F9BD058A12A280433ED6FA46510A";
  
        public string? AvatarPath { get; set; }

        public enumKullaniciStatus Status { get; set; }=enumKullaniciStatus.SifreDegistirmeli;

        public ICollection<KullaniciKullaniciGrup> KullaniciKullaniciGruplar { get; set; } = new List<KullaniciKullaniciGrup>();
    }
}
