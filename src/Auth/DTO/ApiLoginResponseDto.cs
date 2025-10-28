namespace AIInstructor.src.Auth.DTO
{
    public class ApiLoginResponseDto
    {
        public bool Success { get; set; }
        public string Token { get; set; } = string.Empty;
        public DateTime ExpiresAt { get; set; }
        public string Role { get; set; } = string.Empty;
    }
}
