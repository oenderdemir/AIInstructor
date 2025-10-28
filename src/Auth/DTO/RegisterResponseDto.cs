using System;

namespace AIInstructor.src.Auth.DTO
{
    public class RegisterResponseDto
    {
        public Guid KullaniciId { get; set; }
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
    }
}
