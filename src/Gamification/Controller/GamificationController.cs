using System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using AIInstructor.src.Gamification.DTO;
using AIInstructor.src.Gamification.Service;

namespace AIInstructor.src.Gamification.Controller
{
    [ApiController]
    [Route("api/gamification")]
    public class GamificationController : ControllerBase
    {
        private readonly IGamificationResultService service;

        public GamificationController(IGamificationResultService service)
        {
            this.service = service;
        }

        [HttpGet("ogrenci-senaryo/{ogrenciSenaryoId:guid}/result")]
        [Authorize(Roles = "DersYetkilisi")]
        public async Task<ActionResult<GamificationResultDto>> GetResult(Guid ogrenciSenaryoId)
        {
            var result = await service.GetResultAsync(ogrenciSenaryoId);
            if (result == null)
            {
                return NotFound();
            }

            return Ok(result);
        }
    }
}
