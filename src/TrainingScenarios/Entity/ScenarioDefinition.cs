using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using AIInstructor.src.Shared.RDBMS.Entity;

namespace AIInstructor.src.TrainingScenarios.Entity;

public sealed class ScenarioDefinition : BaseEntity
{
    public ScenarioDefinition()
    {
        Id = Guid.NewGuid();
    }

    [JsonPropertyName("id")]
    public required string ScenarioCode { get; init; }

    [JsonPropertyName("title")]
    public required string Title { get; init; }

    [JsonPropertyName("description")]
    public required string Description { get; init; }

    [JsonPropertyName("language")]
    public string Language { get; init; } = "en";

    [JsonPropertyName("customerProfile")]
    public required CustomerProfile CustomerProfile { get; init; }

    [JsonPropertyName("goals")]
    public IReadOnlyCollection<string> Goals { get; init; } = Array.Empty<string>();

    [JsonPropertyName("steps")]
    public IReadOnlyCollection<ScenarioStep> Steps { get; init; } = Array.Empty<ScenarioStep>();

    [JsonPropertyName("maxTurns")]
    public int MaxTurns { get; init; } = 6;

    [JsonPropertyName("difficulty")]
    public string Difficulty { get; init; } = "Intermediate";

    [JsonPropertyName("tags")]
    public IReadOnlyCollection<string> Tags { get; init; } = Array.Empty<string>();

    [JsonPropertyName("successCriteria")]
    public IReadOnlyCollection<string> SuccessCriteria { get; init; } = Array.Empty<string>();
}

public sealed class CustomerProfile : BaseEntity
{
    public CustomerProfile()
    {
        Id = Guid.NewGuid();
    }

    [JsonPropertyName("name")]
    public required string Name { get; init; }

    [JsonPropertyName("background")]
    public required string Background { get; init; }

    [JsonPropertyName("personalityTraits")]
    public IReadOnlyCollection<string> PersonalityTraits { get; init; } = Array.Empty<string>();

    [JsonPropertyName("preferredLanguage")]
    public string PreferredLanguage { get; init; } = "en";
}

public sealed class ScenarioStep : BaseEntity
{
    public ScenarioStep()
    {
        Id = Guid.NewGuid();
    }

    [JsonPropertyName("order")]
    public required int Order { get; init; }

    [JsonPropertyName("tutorPrompt")]
    public required string TutorPrompt { get; init; }

    [JsonPropertyName("expectedStudentActions")]
    public IReadOnlyCollection<string> ExpectedStudentActions { get; init; } = Array.Empty<string>();

    [JsonPropertyName("knowledgeHints")]
    public IReadOnlyCollection<string> KnowledgeHints { get; init; } = Array.Empty<string>();
}
