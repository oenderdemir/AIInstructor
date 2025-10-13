using AIInstructor.src.TrainingScenarios.DTO;
using AIInstructor.src.TrainingScenarios.Entity;

namespace AIInstructor.src.TrainingScenarios.Service
{
    public interface IGamificationService
    {
        Task<GamificationProfileDto> ApplyScenarioResultAsync(Guid studentId, ScenarioEvaluation evaluation, TrainingScenario scenario);
    }
}
