using AIInstructor.src.TrainingScenarios.Entity;

namespace AIInstructor.src.TrainingScenarios.Repository;

public interface IScenarioRepository
{
    Task<IReadOnlyCollection<ScenarioDefinition>> GetAllAsync(CancellationToken cancellationToken = default);

    Task<ScenarioDefinition?> GetByIdAsync(string id, CancellationToken cancellationToken = default);
}
