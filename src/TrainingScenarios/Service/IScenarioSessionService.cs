using AIInstructor.src.TrainingScenarios.DTO;

namespace AIInstructor.src.TrainingScenarios.Service
{
    public interface IScenarioSessionService
    {
        Task<ScenarioSessionDetailDto> GetSessionAsync(Guid sessionId);
        Task<ScenarioTurnResponseDto> StartSessionAsync(Guid scenarioId, StartScenarioSessionRequest request, CancellationToken cancellationToken = default);
        Task<ScenarioTurnResponseDto> SubmitTurnAsync(Guid sessionId, SubmitScenarioTurnRequest request, CancellationToken cancellationToken = default);
    }
}
