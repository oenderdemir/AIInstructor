using AIInstructor.src.Shared.RDBMS.Repository;
using AIInstructor.src.TrainingScenarios.Entity;

namespace AIInstructor.src.TrainingScenarios.Repository
{
    public interface IScenarioEvaluationRepository : IBaseRepository<ScenarioEvaluation>
    {
        Task<ScenarioEvaluation?> GetBySessionIdAsync(Guid sessionId);
    }
}
