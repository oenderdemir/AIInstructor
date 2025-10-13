using AIInstructor.src.Auth.DTO;

namespace AIInstructor.src.Auth.Service
{
    public interface IJWTTokenService
    {
        Task<GenerateTokenResponse> GenerateToken(GenerateTokenRequest request);
    }
}
