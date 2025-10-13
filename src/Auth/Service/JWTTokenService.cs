using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AIInstructor.src.Auth.DTO;

namespace AIInstructor.src.Auth.Service
{
    public class JWTTokenService : IJWTTokenService
    {
        readonly IConfiguration configuration;

        public JWTTokenService(IConfiguration configuration)
        {
            this.configuration = configuration;
        }
        public Task<GenerateTokenResponse> GenerateToken(GenerateTokenRequest request)
        {
            var key = configuration["Jwt:Key"];
            if (string.IsNullOrEmpty(key))
            {
                throw new InvalidOperationException("JWT Key is not configured properly.");
            }

            var symmetricSecurityKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(key));
            var dateTimeNow = DateTime.UtcNow;
            var tokenExpirationHours = 8; // Token geçerlilik süresi, saat olarak

            var claims = new List<Claim>
        {
            new Claim("userName", request.KullaniciAdi),
            new Claim(ClaimTypes.Name, request.Ad),
            new Claim(ClaimTypes.Email, request.EMail),
            new Claim(ClaimTypes.Surname, request.Soyad),
            new Claim(ClaimTypes.NameIdentifier, request.KullaniciAdi)
        };
           
            foreach (var r in request.Roller)
            {
                claims.Add(new Claim("permission", $"{r.Domain}.{r.Ad}"));
                if (r.Ad == "Manage")
                    claims.Add(new Claim("permission", $"{r.Domain}.View"));
            }

            var jwt = new JwtSecurityToken(
                issuer: configuration["Jwt:Issuer"],
                audience: configuration["Jwt:Audience"],
                claims: claims,
                notBefore: dateTimeNow,
                expires: dateTimeNow.AddHours(tokenExpirationHours),
                signingCredentials: new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256)
            );

            var token = new JwtSecurityTokenHandler().WriteToken(jwt);

            return Task.FromResult(new GenerateTokenResponse
            {
                Token = token,
                TokenExpireDate = dateTimeNow.AddHours(tokenExpirationHours) // Token süresini tutarlı hale getirdik
            });
        }
    }
}
