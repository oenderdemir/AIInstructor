using System.Collections.Concurrent;
using AIInstructor.src.TrainingScenarios.Entity;
using Microsoft.Extensions.Logging;

namespace AIInstructor.src.TrainingScenarios.Services;

public interface IGamificationService
{
    Task<GamificationProfile> RegisterCompletionAsync(ScenarioSession session, ScenarioDefinition scenario, EvaluationResult evaluation, CancellationToken cancellationToken = default);

    GamificationProfile GetProfile(string studentId);
}

public sealed class GamificationService : IGamificationService
{
    private readonly ConcurrentDictionary<string, GamificationProfile> _profiles = new(StringComparer.OrdinalIgnoreCase);
    private readonly ILogger<GamificationService> _logger;

    public GamificationService(ILogger<GamificationService> logger)
    {
        _logger = logger;
    }

    public Task<GamificationProfile> RegisterCompletionAsync(ScenarioSession session, ScenarioDefinition scenario, EvaluationResult evaluation, CancellationToken cancellationToken = default)
    {
        var profile = _profiles.GetOrAdd(session.StudentId, id => new GamificationProfile { StudentId = id });

        var pointsEarned = (int)Math.Round(evaluation.OverallScore);
        profile.TotalPoints += pointsEarned;
        profile.Level = CalculateLevel(profile.TotalPoints);

        var earnedBadges = DetermineBadges(scenario, evaluation, profile);
        foreach (var badge in earnedBadges)
        {
            profile.Badges.Add(badge);
        }

        profile.CompletedScenarios.Add(new ScenarioHistoryEntry
        {
            ScenarioId = scenario.Id,
            Score = evaluation.OverallScore,
            CompletedAt = DateTimeOffset.UtcNow,
            EarnedBadges = earnedBadges
        });

        _logger.LogInformation("Student {StudentId} completed scenario {ScenarioId} with score {Score}", session.StudentId, scenario.Id, evaluation.OverallScore);

        return Task.FromResult(profile);
    }

    public GamificationProfile GetProfile(string studentId)
    {
        return _profiles.GetOrAdd(studentId, id => new GamificationProfile { StudentId = id });
    }

    private static int CalculateLevel(int totalPoints)
    {
        return Math.Max(1, (totalPoints / 500) + 1);
    }

    private static IReadOnlyCollection<string> DetermineBadges(ScenarioDefinition scenario, EvaluationResult evaluation, GamificationProfile profile)
    {
        var badges = new List<string>();

        if (evaluation.OverallScore >= 90)
        {
            badges.Add("Mükemmel Performans");
        }

        if (evaluation.ProblemSolving >= 85)
        {
            badges.Add("Zor Müşteri Ustası");
        }

        if (evaluation.Communication >= 85 && evaluation.LanguageUse >= 85)
        {
            badges.Add("Mükemmel Check-in");
        }

        if (profile.CompletedScenarios.Count >= 5)
        {
            badges.Add("Deneyimli Rehber");
        }

        return badges;
    }
}
