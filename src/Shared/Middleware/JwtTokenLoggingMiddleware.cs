using Serilog.Context;
using System.IdentityModel.Tokens.Jwt;

namespace AIInstructor.src.Shared.Middleware
{
    public class JwtTokenLoggingMiddleware
    {
        private readonly RequestDelegate _next;

        public JwtTokenLoggingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
            if (token != null)
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                if (tokenHandler.CanReadToken(token))
                {
                    var jwtToken = tokenHandler.ReadJwtToken(token);

                    // Örnek olarak 'sub' (subject) ve 'email' claim'lerini ekliyoruz
                    var userName = jwtToken.Claims.FirstOrDefault(claim => claim.Type == "userName")?.Value;

                    using (LogContext.PushProperty("userName", userName))
                    {
                        await _next(context);
                    }
                }
            }
            else
            {
                await _next(context);
            }
        }
    }
}
