using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using AIInstructor.src.AIInstructor.DTO;
using AIInstructor.src.AIInstructor.Service;
using AIInstructor.src.Gamification.Service;

namespace AIInstructor.src.AIInstructor.Controller
{
    [ApiController]
    [Route("api/ai-instructor")]
    public class AIInstructorController : ControllerBase
    {
        private readonly IAIInstructorService instructorService;
        private readonly IGamificationResultService gamificationService;

        public AIInstructorController(IAIInstructorService instructorService, IGamificationResultService gamificationService)
        {
            this.instructorService = instructorService;
            this.gamificationService = gamificationService;
        }

        [HttpPost("evaluate")]
        [Authorize(Roles = "DersYetkilisi,Ogrenci")]
        public async Task<ActionResult<AIInstructorEvaluateResponse>> Evaluate([FromBody] AIInstructorEvaluateRequest request)
        {
            var session = await instructorService.EvaluateAsync(request.OgrenciSenaryoId, request.Messages ?? new List<string>());
            var gamification = await gamificationService.GetResultAsync(request.OgrenciSenaryoId);

            var response = new AIInstructorEvaluateResponse
            {
                Success = true,
                Hints = session.GeriBildirimler.Where(e => !e.Success).Select(e => e.Mesaj).ToList(),
                Badge = gamification?.Badge,
                Puan = gamification?.Puan
            };

            return Ok(response);
        }
    }
}
