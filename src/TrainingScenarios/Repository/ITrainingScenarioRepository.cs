using AIInstructor.src.Shared.RDBMS.Repository;
using AIInstructor.src.TrainingScenarios.Entity;

namespace AIInstructor.src.TrainingScenarios.Repository
{
    public interface ITrainingScenarioRepository : IBaseRepository<TrainingScenario>
    {
        Task<TrainingScenario?> GetWithSessionsAsync(Guid id);
    }
}
