namespace AIInstructor.src.TrainingScenarios.Entity;

public sealed class EvaluationResult
{
    public required double Communication { get; init; }

    public required double ProblemSolving { get; init; }

    public required double LanguageUse { get; init; }

    public required double Professionalism { get; init; }

    public required double OverallScore { get; init; }

    public required string Strengths { get; init; }

    public required string ImprovementAreas { get; init; }
}

public sealed class GamificationProfile
{
    public required string StudentId { get; init; }

    public int TotalPoints { get; set; }

    public int Level { get; set; } = 1;

    public HashSet<string> Badges { get; } = new(StringComparer.OrdinalIgnoreCase);

    public List<ScenarioHistoryEntry> CompletedScenarios { get; } = new();
}

public sealed class ScenarioHistoryEntry
{
    public required string ScenarioId { get; init; }

    public required double Score { get; init; }

    public required DateTimeOffset CompletedAt { get; init; }

    public required IReadOnlyCollection<string> EarnedBadges { get; init; }
}
