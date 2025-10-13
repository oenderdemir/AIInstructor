using System;
using System.Collections.Generic;
using AIInstructor.src.Shared.RDBMS.Dto;
using AIInstructor.src.TrainingScenarios.Entity;

namespace AIInstructor.src.TrainingScenarios.DTO;

public sealed class ScenarioSummaryDto : BaseRDBMSDto
{
    public ScenarioSummaryDto()
    {
        Id = Guid.NewGuid();
    }

    public required string ScenarioCode { get; init; }
    public required string Title { get; init; }
    public required string Description { get; init; }
    public required string Difficulty { get; init; }
    public required string Language { get; init; }
    public IReadOnlyCollection<string> Tags { get; init; } = Array.Empty<string>();
}

public sealed class StartScenarioRequest : BaseRDBMSDto
{
    public StartScenarioRequest()
    {
        Id = Guid.NewGuid();
    }

    public required string StudentId { get; init; }
}

public sealed class StartScenarioResponse : BaseRDBMSDto
{
    private Guid _sessionId;

    public Guid SessionId
    {
        get => _sessionId;
        init
        {
            _sessionId = value;
            Id = value;
        }
    }

    public required ScenarioSummaryDto Scenario { get; init; }
    public required string TutorMessage { get; init; }
    public required int CurrentTurn { get; init; }
    public required int MaxTurns { get; init; }
}

public sealed class StudentMessageRequest : BaseRDBMSDto
{
    public StudentMessageRequest()
    {
        Id = Guid.NewGuid();
    }

    public required string StudentId { get; init; }
    public required string Message { get; init; }
}

public sealed class ScenarioTurnResponse : BaseRDBMSDto
{
    private Guid _sessionId;

    public Guid SessionId
    {
        get => _sessionId;
        init
        {
            _sessionId = value;
            Id = value;
        }
    }

    public required string TutorMessage { get; init; }
    public required int CurrentTurn { get; init; }
    public required int MaxTurns { get; init; }
    public required bool IsCompleted { get; init; }
    public EvaluationResult? Evaluation { get; init; }
    public GamificationProfile? GamificationProfile { get; init; }
}

public sealed class CompleteScenarioRequest : BaseRDBMSDto
{
    public CompleteScenarioRequest()
    {
        Id = Guid.NewGuid();
    }

    public required string StudentId { get; init; }
}

public sealed class ScenarioTranscriptEntry : BaseRDBMSDto
{
    public ScenarioTranscriptEntry()
    {
        Id = Guid.NewGuid();
    }

    public required string Role { get; init; }
    public required string Content { get; init; }
    public required DateTimeOffset Timestamp { get; init; }
}

public sealed class ScenarioTranscriptResponse : BaseRDBMSDto
{
    private Guid _sessionId;

    public Guid SessionId
    {
        get => _sessionId;
        init
        {
            _sessionId = value;
            Id = value;
        }
    }

    public required string ScenarioCode { get; init; }
    public IReadOnlyCollection<ScenarioTranscriptEntry> Transcript { get; init; } = Array.Empty<ScenarioTranscriptEntry>();
    public EvaluationResult? Evaluation { get; init; }
}
