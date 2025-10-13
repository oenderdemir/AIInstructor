using System;
using System.Collections.Generic;
using AIInstructor.src.Shared.RDBMS.Entity;

namespace AIInstructor.src.TrainingScenarios.Entity;

public sealed class ScenarioSession : BaseEntity
{
    public ScenarioSession()
    {
        Id = Guid.NewGuid();
    }

    public required string ScenarioId { get; init; }

    public required string StudentId { get; init; }

    public DateTimeOffset StartedAt { get; init; } = DateTimeOffset.UtcNow;

    public DateTimeOffset? CompletedAt { get; set; }

    public int MaxTurns { get; init; }

    public int CurrentTurn { get; set; }

    public bool IsCompleted { get; set; }

    public List<ScenarioMessage> Transcript { get; } = new();
}

public sealed class ScenarioMessage : BaseEntity
{
    public ScenarioMessage()
    {
        Id = Guid.NewGuid();
    }

    public required string Role { get; init; }

    public required string Content { get; init; }

    public DateTimeOffset Timestamp { get; init; } = DateTimeOffset.UtcNow;
}
