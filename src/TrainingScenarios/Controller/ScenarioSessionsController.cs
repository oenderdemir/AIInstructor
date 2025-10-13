using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using AIInstructor.src.TrainingScenarios.DTO;
using AIInstructor.src.TrainingScenarios.Service;

namespace AIInstructor.src.TrainingScenarios.Controller
{
    [Authorize(Policy = "UIPolicy")]
    [ApiController]
    [Route("ui/training-scenarios")]
    public class ScenarioSessionsController : ControllerBase
    {
        private readonly IScenarioSessionService scenarioSessionService;
        private readonly ILogger<ScenarioSessionsController> logger;

        public ScenarioSessionsController(
            IScenarioSessionService scenarioSessionService,
            ILogger<ScenarioSessionsController> logger)
        {
            this.scenarioSessionService = scenarioSessionService;
            this.logger = logger;
        }

        [HttpPost("{scenarioId:guid}/sessions")]
        public async Task<ActionResult<ScenarioTurnResponseDto>> StartSessionAsync(Guid scenarioId, [FromBody] StartScenarioSessionRequest request, CancellationToken cancellationToken = default)
        {
            try
            {
                var response = await scenarioSessionService.StartSessionAsync(scenarioId, request, cancellationToken);
                return Ok(response);
            }
            catch (KeyNotFoundException ex)
            {
                logger.LogWarning(ex, "Senaryo bulunamadı: {ScenarioId}", scenarioId);
                return NotFound();
            }
        }

        [HttpPost("sessions/{sessionId:guid}/turns")]
        public async Task<ActionResult<ScenarioTurnResponseDto>> SubmitTurnAsync(Guid sessionId, [FromBody] SubmitScenarioTurnRequest request, CancellationToken cancellationToken = default)
        {
            try
            {
                var response = await scenarioSessionService.SubmitTurnAsync(sessionId, request, cancellationToken);
                return Ok(response);
            }
            catch (KeyNotFoundException ex)
            {
                logger.LogWarning(ex, "Oturum bulunamadı: {SessionId}", sessionId);
                return NotFound();
            }
            catch (InvalidOperationException ex)
            {
                logger.LogWarning(ex, "Oturum için işlem gerçekleştirilemedi: {SessionId}", sessionId);
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("sessions/{sessionId:guid}")]
        public async Task<ActionResult<ScenarioSessionDetailDto>> GetSessionAsync(Guid sessionId)
        {
            try
            {
                var session = await scenarioSessionService.GetSessionAsync(sessionId);
                return Ok(session);
            }
            catch (KeyNotFoundException ex)
            {
                logger.LogWarning(ex, "Oturum bulunamadı: {SessionId}", sessionId);
                return NotFound();
            }
        }
    }
}
