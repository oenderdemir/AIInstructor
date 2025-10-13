using AIInstructor.src.Shared.RDBMS.Repository;
using AIInstructor.src.TrainingScenarios.Entity;

namespace AIInstructor.src.TrainingScenarios.Repository
{
    public interface IScenarioSessionRepository : IBaseRepository<ScenarioSession>
    {
        Task<ScenarioSession?> GetSessionWithDetailsAsync(Guid sessionId);
        Task<IReadOnlyList<ScenarioSession>> GetActiveSessionsForStudentAsync(Guid studentId);
    }
}
