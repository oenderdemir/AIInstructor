using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using AIInstructor.src.Shared.Controller;
using AIInstructor.src.TrainingScenarios.DTO;
using AIInstructor.src.TrainingScenarios.Service;

namespace AIInstructor.src.TrainingScenarios.Controller
{
    public class TrainingScenariosController : UIController
    {
        private readonly ITrainingScenarioService trainingScenarioService;
        private readonly ILogger<TrainingScenariosController> logger;

        public TrainingScenariosController(
            ITrainingScenarioService trainingScenarioService,
            ILogger<TrainingScenariosController> logger)
        {
            this.trainingScenarioService = trainingScenarioService;
            this.logger = logger;
        }

        [HttpGet]
        public async Task<IEnumerable<TrainingScenarioSummaryDto>> GetAllAsync()
        {
            return await trainingScenarioService.GetAllAsync();
        }

        [HttpGet("{id:guid}")]
        public async Task<ActionResult<TrainingScenarioDetailDto>> GetAsync(Guid id)
        {
            try
            {
                var scenario = await trainingScenarioService.GetByIdAsync(id);
                return Ok(scenario);
            }
            catch (KeyNotFoundException ex)
            {
                logger.LogWarning(ex, "Senaryo bulunamadı: {ScenarioId}", id);
                return NotFound();
            }
        }

        [HttpPost]
        public async Task<ActionResult<TrainingScenarioDetailDto>> CreateAsync([FromBody] CreateTrainingScenarioRequest request)
        {
            var scenario = await trainingScenarioService.CreateAsync(request);
            return CreatedAtAction(nameof(GetAsync), new { id = scenario.Id }, scenario);
        }

        [HttpPut("{id:guid}")]
        public async Task<ActionResult<TrainingScenarioDetailDto>> UpdateAsync(Guid id, [FromBody] UpdateTrainingScenarioRequest request)
        {
            try
            {
                var scenario = await trainingScenarioService.UpdateAsync(id, request);
                return Ok(scenario);
            }
            catch (KeyNotFoundException ex)
            {
                logger.LogWarning(ex, "Güncellenecek senaryo bulunamadı: {ScenarioId}", id);
                return NotFound();
            }
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> DeleteAsync(Guid id)
        {
            try
            {
                await trainingScenarioService.DeleteAsync(id);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                logger.LogWarning(ex, "Silinecek senaryo bulunamadı: {ScenarioId}", id);
                return NotFound();
            }
        }
    }
}
