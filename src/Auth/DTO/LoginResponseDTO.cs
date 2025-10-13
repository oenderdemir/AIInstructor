namespace AIInstructor.src.Auth.DTO
{
    public class LoginResponseDTO
    {
        public bool AuthenticateResult { get; set; }
        public string AuthToken { get; set; }
        public DateTime AccessTokenExpireDate { get; set; }
        public enumKullaniciStatus KullaniciStatus { get; set; }
    }
}
