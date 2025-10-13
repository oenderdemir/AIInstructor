using AIInstructor.src.Shared.RDBMS.Repository;
using AIInstructor.src.TrainingScenarios.Entity;

namespace AIInstructor.src.TrainingScenarios.Repository
{
    public interface IScenarioMessageRepository : IBaseRepository<ScenarioMessage>
    {
        Task<IReadOnlyList<ScenarioMessage>> GetBySessionAsync(Guid sessionId);
    }
}
