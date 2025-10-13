using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using AIInstructor.src.Auth.DTO;
using AIInstructor.src.Auth.Service;

namespace AIInstructor.src.Auth.Controller
{
    [Route("ui/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthenticationService authService;

        public AuthController(IAuthenticationService _authService)
        {
            this.authService = _authService;
        }

        [HttpPost("login")]
        public async Task<ActionResult<LoginResponseDTO>> Login([FromBody] LoginRequestDTO model)
        {

            var result = await this.authService.LoginAsync(model);
            if (result == null || result.AuthenticateResult == false)
            {
                return Unauthorized();
            }

            var logData = new
            {
                Type = "Login",
                Timestamp = DateTime.UtcNow,
                RequestBody = "Login başarılı",
                userName = model.KullaniciAdi
            };
            var m = new
            {
                userName = model.KullaniciAdi,
                LogData = logData
            };
            //LogContext.PushProperty("userName", model.KullaniciAdi);
            Log.Information("{@LogData}", logData);
            return result;
        }


        [HttpPost("changePassword")]
        [Authorize(Policy = "UIPolicy")]
        public async Task<ActionResult<LoginResponseDTO>> ChangePassword([FromBody] ChangePasswordRequestDTO model)
        {

            return await this.authService.ChangePassword(model);
           
            
        }
        [HttpGet("logout")]
        public async Task<ActionResult<LoginResponseDTO>> Logout()
        {
            var result = await this.authService.LogoutAsync();
            return result;
        }
    }
}
