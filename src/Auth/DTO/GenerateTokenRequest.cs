using AIInstructor.src.Roller.Entity;

namespace AIInstructor.src.Auth.DTO
{
    public class GenerateTokenRequest
    {
        public string KullaniciAdi { get; set; }
        public string Ad { get; set; }
        public string Soyad { get; set; }
        public string EMail { get; set; }

        public List<Rol> Roller { get; set; }

        public GenerateTokenRequest()
        {
            Roller = new List<Rol>();
        }
    }
}
