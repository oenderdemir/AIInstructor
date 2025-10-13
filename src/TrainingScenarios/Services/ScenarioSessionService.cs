using System.Collections.Concurrent;
using System.Text;
using AIInstructor.src.TrainingScenarios.DTO;
using AIInstructor.src.TrainingScenarios.Entity;
using AIInstructor.src.TrainingScenarios.Repository;
using Microsoft.Extensions.Logging;

namespace AIInstructor.src.TrainingScenarios.Services;

public interface IScenarioSessionService
{
    Task<StartScenarioResponse> StartScenarioAsync(string scenarioId, string studentId, CancellationToken cancellationToken = default);

    Task<ScenarioTurnResponse> SubmitStudentMessageAsync(Guid sessionId, string studentId, string message, CancellationToken cancellationToken = default);

    Task<ScenarioTurnResponse> CompleteScenarioAsync(Guid sessionId, string studentId, CancellationToken cancellationToken = default);

    Task<ScenarioTranscriptResponse?> GetTranscriptAsync(Guid sessionId, CancellationToken cancellationToken = default);
}

public sealed class ScenarioSessionService : IScenarioSessionService
{
    private readonly IScenarioRepository _scenarioRepository;
    private readonly IOpenAIChatClient _chatClient;
    private readonly IEvaluationService _evaluationService;
    private readonly IGamificationService _gamificationService;
    private readonly ILogger<ScenarioSessionService> _logger;
    private readonly ConcurrentDictionary<Guid, ScenarioSession> _sessions = new();

    public ScenarioSessionService(
        IScenarioRepository scenarioRepository,
        IOpenAIChatClient chatClient,
        IEvaluationService evaluationService,
        IGamificationService gamificationService,
        ILogger<ScenarioSessionService> logger)
    {
        _scenarioRepository = scenarioRepository;
        _chatClient = chatClient;
        _evaluationService = evaluationService;
        _gamificationService = gamificationService;
        _logger = logger;
    }

    public async Task<StartScenarioResponse> StartScenarioAsync(string scenarioId, string studentId, CancellationToken cancellationToken = default)
    {
        var scenario = await _scenarioRepository.GetByIdAsync(scenarioId, cancellationToken)
                      ?? throw new InvalidOperationException($"Scenario '{scenarioId}' not found.");

        var session = new ScenarioSession
        {
            Id = Guid.NewGuid(),
            ScenarioId = scenario.Id,
            StudentId = studentId,
            MaxTurns = scenario.MaxTurns,
            CurrentTurn = 0,
            IsCompleted = false
        };

        var systemPrompt = BuildSystemPrompt(scenario);
        session.Transcript.Add(new ScenarioMessage { Role = "system", Content = systemPrompt });

        var tutorIntro = await _chatClient.GetChatCompletionAsync(session.Transcript, new ChatCompletionRequest
        {
            AdditionalUserInstruction = "Senaryo için öğrenciye sıcak bir karşılama yap ve bağlamı açıkla."
        }, cancellationToken);

        session.Transcript.Add(new ScenarioMessage { Role = "assistant", Content = tutorIntro });
        _sessions[session.Id] = session;

        var summary = MapToSummary(scenario);
        return new StartScenarioResponse(session.Id, summary, tutorIntro, session.CurrentTurn, scenario.MaxTurns);
    }

    public async Task<ScenarioTurnResponse> SubmitStudentMessageAsync(Guid sessionId, string studentId, string message, CancellationToken cancellationToken = default)
    {
        if (!_sessions.TryGetValue(sessionId, out var session))
        {
            throw new InvalidOperationException("Session not found.");
        }

        if (!string.Equals(session.StudentId, studentId, StringComparison.Ordinal))
        {
            throw new InvalidOperationException("Student does not own this session.");
        }

        if (session.IsCompleted)
        {
            var transcriptResponse = await GetTranscriptAsync(sessionId, cancellationToken) ?? throw new InvalidOperationException("Transcript unavailable.");
            return new ScenarioTurnResponse(sessionId, string.Empty, session.CurrentTurn, session.MaxTurns, true, transcriptResponse.Evaluation, null);
        }

        session.Transcript.Add(new ScenarioMessage { Role = "user", Content = message });
        session.CurrentTurn++;

        var tutorResponse = await _chatClient.GetChatCompletionAsync(session.Transcript, new ChatCompletionRequest(), cancellationToken);
        session.Transcript.Add(new ScenarioMessage { Role = "assistant", Content = tutorResponse });

        var scenario = await _scenarioRepository.GetByIdAsync(session.ScenarioId, cancellationToken) ?? throw new InvalidOperationException("Scenario not found for session.");

        if (session.CurrentTurn >= scenario.MaxTurns)
        {
            return await FinalizeScenarioAsync(session, scenario, tutorResponse, cancellationToken);
        }

        return new ScenarioTurnResponse(session.Id, tutorResponse, session.CurrentTurn, scenario.MaxTurns, false, null, null);
    }

    public async Task<ScenarioTurnResponse> CompleteScenarioAsync(Guid sessionId, string studentId, CancellationToken cancellationToken = default)
    {
        if (!_sessions.TryGetValue(sessionId, out var session))
        {
            throw new InvalidOperationException("Session not found.");
        }

        if (!string.Equals(session.StudentId, studentId, StringComparison.Ordinal))
        {
            throw new InvalidOperationException("Student does not own this session.");
        }

        if (session.IsCompleted)
        {
            var transcript = await GetTranscriptAsync(sessionId, cancellationToken) ?? throw new InvalidOperationException("Transcript unavailable.");
            return new ScenarioTurnResponse(session.Id, string.Empty, session.CurrentTurn, session.MaxTurns, true, transcript.Evaluation, null);
        }

        var scenario = await _scenarioRepository.GetByIdAsync(session.ScenarioId, cancellationToken) ?? throw new InvalidOperationException("Scenario not found for session.");
        return await FinalizeScenarioAsync(session, scenario, string.Empty, cancellationToken);
    }

    public Task<ScenarioTranscriptResponse?> GetTranscriptAsync(Guid sessionId, CancellationToken cancellationToken = default)
    {
        if (!_sessions.TryGetValue(sessionId, out var session))
        {
            return Task.FromResult<ScenarioTranscriptResponse?>(null);
        }

        var transcript = session.Transcript
            .Where(m => !string.Equals(m.Role, "system", StringComparison.OrdinalIgnoreCase))
            .Select(m => new ScenarioTranscriptEntry(m.Role, m.Content, m.Timestamp))
            .ToList();

        var evaluation = session.IsCompleted ? session.Transcript
            .Where(m => m.Role == "evaluation")
            .Select(m => System.Text.Json.JsonSerializer.Deserialize<EvaluationResult>(m.Content, new System.Text.Json.JsonSerializerOptions(System.Text.Json.JsonSerializerDefaults.Web)))
            .LastOrDefault() : null;

        return Task.FromResult<ScenarioTranscriptResponse?>(new ScenarioTranscriptResponse(session.Id, session.ScenarioId, transcript, evaluation));
    }

    private async Task<ScenarioTurnResponse> FinalizeScenarioAsync(ScenarioSession session, ScenarioDefinition scenario, string lastTutorMessage, CancellationToken cancellationToken)
    {
        session.IsCompleted = true;
        session.CompletedAt = DateTimeOffset.UtcNow;

        try
        {
            var evaluation = await _evaluationService.EvaluateAsync(session, scenario, cancellationToken);
            var gamification = await _gamificationService.RegisterCompletionAsync(session, scenario, evaluation, cancellationToken);

            session.Transcript.Add(new ScenarioMessage
            {
                Role = "evaluation",
                Content = System.Text.Json.JsonSerializer.Serialize(evaluation, new System.Text.Json.JsonSerializerOptions(System.Text.Json.JsonSerializerDefaults.Web))
            });

            return new ScenarioTurnResponse(session.Id, lastTutorMessage, session.CurrentTurn, scenario.MaxTurns, true, evaluation, gamification);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to finalize scenario {ScenarioId} for session {SessionId}", scenario.Id, session.Id);
            throw;
        }
    }

    private static ScenarioSummaryDto MapToSummary(ScenarioDefinition scenario)
    {
        return new ScenarioSummaryDto(
            scenario.Id,
            scenario.Title,
            scenario.Description,
            scenario.Difficulty,
            scenario.Language,
            scenario.Tags.ToList());
    }

    private static string BuildSystemPrompt(ScenarioDefinition scenario)
    {
        var builder = new StringBuilder();
        builder.AppendLine("Sen bir AI Tutor'sun ve aşağıdaki otelcilik senaryosunu yürütüyorsun.");
        builder.AppendLine();
        builder.AppendLine($"Senaryo: {scenario.Title}");
        builder.AppendLine($"Açıklama: {scenario.Description}");
        builder.AppendLine("Müşteri Profili:");
        builder.AppendLine($"- İsim: {scenario.CustomerProfile.Name}");
        builder.AppendLine($"- Arka Plan: {scenario.CustomerProfile.Background}");
        if (scenario.CustomerProfile.PersonalityTraits.Any())
        {
            builder.AppendLine("- Kişilik Özellikleri: " + string.Join(", ", scenario.CustomerProfile.PersonalityTraits));
        }

        builder.AppendLine();
        builder.AppendLine("Senaryonun hedefleri:");
        foreach (var goal in scenario.Goals)
        {
            builder.AppendLine("- " + goal);
        }

        builder.AppendLine();
        builder.AppendLine("Kurallar:");
        builder.AppendLine("1. Diyalog boyunca öğrenciyi cesaretlendir ve geri bildirim ver.");
        builder.AppendLine("2. Öğrencinin hatalarını kibarca düzelt, doğru örnekler ver.");
        builder.AppendLine("3. Her turda müşteri rolüne sadık kal ve doğal konuş.");
        builder.AppendLine("4. Gerektiğinde senaryoyu zenginleştirmek için ek ayrıntılar sağla.");
        builder.AppendLine("5. Senaryo maksimum tur sayısına ulaşınca, öğrenciden son çözümü iste ve değerlendirme için hazır olduğunu belirt.");

        if (scenario.SuccessCriteria.Any())
        {
            builder.AppendLine();
            builder.AppendLine("Başarı kriterleri:");
            foreach (var criteria in scenario.SuccessCriteria)
            {
                builder.AppendLine("- " + criteria);
            }
        }

        if (scenario.Steps.Any())
        {
            builder.AppendLine();
            builder.AppendLine("Senaryo adımları rehberi:");
            foreach (var step in scenario.Steps.OrderBy(s => s.Order))
            {
                builder.AppendLine($"Adım {step.Order}: {step.TutorPrompt}");
                if (step.ExpectedStudentActions.Any())
                {
                    builder.AppendLine("Beklenen öğrenci davranışları: " + string.Join(", ", step.ExpectedStudentActions));
                }
            }
        }

        return builder.ToString();
    }
}
