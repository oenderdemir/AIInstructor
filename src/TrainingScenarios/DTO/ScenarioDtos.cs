using AIInstructor.src.TrainingScenarios.Entity;

namespace AIInstructor.src.TrainingScenarios.DTO;

public sealed record ScenarioSummaryDto(
    string Id,
    string Title,
    string Description,
    string Difficulty,
    string Language,
    IReadOnlyCollection<string> Tags);

public sealed record StartScenarioRequest(string StudentId);

public sealed record StartScenarioResponse(
    Guid SessionId,
    ScenarioSummaryDto Scenario,
    string TutorMessage,
    int CurrentTurn,
    int MaxTurns);

public sealed record StudentMessageRequest(string StudentId, string Message);

public sealed record ScenarioTurnResponse(
    Guid SessionId,
    string TutorMessage,
    int CurrentTurn,
    int MaxTurns,
    bool IsCompleted,
    EvaluationResult? Evaluation,
    GamificationProfile? GamificationProfile);

public sealed record CompleteScenarioRequest(string StudentId);

public sealed record ScenarioTranscriptEntry(string Role, string Content, DateTimeOffset Timestamp);

public sealed record ScenarioTranscriptResponse(
    Guid SessionId,
    string ScenarioId,
    IReadOnlyCollection<ScenarioTranscriptEntry> Transcript,
    EvaluationResult? Evaluation);
