using Microsoft.AspNetCore.Mvc;
using AIInstructor.src.Auth.DTO;
using AIInstructor.src.Shared.Service;

namespace AIInstructor.src.Auth.Service
{
    public interface IAuthenticationService
    {
        Task<LoginResponseDTO> ChangePassword(ChangePasswordRequestDTO model);
        Task<LoginResponseDTO> LoginAsync(LoginRequestDTO request);
        Task<LoginResponseDTO> LogoutAsync();
    }
}
