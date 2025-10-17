using System.Text.Json;
using System.Text.Json.Serialization;
using AIInstructor.src.TrainingScenarios.Entity;
using Microsoft.Extensions.Logging;

namespace AIInstructor.src.TrainingScenarios.Services;

public interface IEvaluationService
{
    Task<EvaluationResult> EvaluateAsync(ScenarioSession session, ScenarioDefinition scenario, CancellationToken cancellationToken = default);
}

public sealed class EvaluationService : IEvaluationService
{
    private readonly IOpenAIChatClient _chatClient;
    private readonly ILogger<EvaluationService> _logger;

    public EvaluationService(IOpenAIChatClient chatClient, ILogger<EvaluationService> logger)
    {
        _chatClient = chatClient;
        _logger = logger;
    }

    public async Task<EvaluationResult> EvaluateAsync(ScenarioSession session, ScenarioDefinition scenario, CancellationToken cancellationToken = default)
    {
        var transcript = session.Transcript
            .Where(m => m.Role is "assistant" or "user")
            .Select(m => new { m.Role, m.Content, Timestamp = m.Timestamp.ToString("O") })
            .ToList();

        var evaluationPrompt = BuildEvaluationPrompt(scenario, transcript);
        var evaluationMessage = new ScenarioMessage { Role = "system", Content = evaluationPrompt };

        var response = await _chatClient.GetChatCompletionAsync(new[] { evaluationMessage }, new ChatCompletionRequest
        {
            ResponseFormat = new
            {
                type = "json_object"
            },
            AdditionalUserInstruction = @"
                                        Senaryoyu değerlendir ve sadece aşağıdaki JSON yapısında geçerli bir nesne döndür:
                                        {
                                          ""scores"": {
                                            ""communication"": <0-10 arasında bir sayı>,
                                            ""problemSolving"": <0-10 arasında bir sayı>,
                                            ""languageUse"": <0-10 arasında bir sayı>,
                                            ""professionalism"": <0-10 arasında bir sayı>
                                          },
                                          ""overallScore"": <0-10 arasında bir sayı>,
                                          ""feedback"": {
                                            ""strengths"": [""güçlü yön 1"", ""güçlü yön 2"", ...],
                                            ""improvements"": [""iyileştirme alanı 1"", ""iyileştirme alanı 2"", ...]
                                          }
                                        }

                                        Yalnızca yukarıdaki formata tam uygun geçerli JSON döndür, Dil Türkçe olsun, açıklama veya metin ekleme.
                                        "
        }, cancellationToken);

        try
        {
            var result = JsonSerializer.Deserialize<EvaluationPayload>(response, new JsonSerializerOptions(JsonSerializerDefaults.Web)
            {
                ReadCommentHandling = JsonCommentHandling.Skip,
                AllowTrailingCommas = true
            });

            if (result is null)
            {
                throw new InvalidOperationException("Evaluation JSON could not be parsed.");
            }

            return new EvaluationResult
            {
                Communication = result.Scores.Communication,
                ProblemSolving = result.Scores.ProblemSolving,
                LanguageUse = result.Scores.LanguageUse,
                Professionalism = result.Scores.Professionalism,
                OverallScore = result.OverallScore,
                Strengths = string.Join("\n", result.Feedback.Strengths ?? Array.Empty<string>()),
                ImprovementAreas = string.Join("\n", result.Feedback.Improvements ?? Array.Empty<string>())
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Evaluation parsing failed for session {SessionId}", session.Id);
            throw;
        }
    }

    private static string BuildEvaluationPrompt(ScenarioDefinition scenario, IReadOnlyCollection<object> transcript)
    {
        var prompt = new
        {
            instruction = "Aşağıdaki otelcilik senaryosunda öğrenciyi değerlendir. Sadece json formatında cevap ver.",
            scenario = new
            {
                scenario.Title,
                scenario.Description,
                Goals = scenario.Goals,
                SuccessCriteria = scenario.SuccessCriteria,
                CustomerProfile = scenario.CustomerProfile,
            },
            transcript
        };

        return JsonSerializer.Serialize(prompt, new JsonSerializerOptions(JsonSerializerDefaults.Web));
    }

    private sealed class EvaluationPayload
    {
        [JsonPropertyName("overallScore")]
        public required double OverallScore { get; init; }

        [JsonPropertyName("scores")]
        public required ScorePayload Scores { get; init; }

        [JsonPropertyName("feedback")]
        public required FeedbackPayload Feedback { get; init; }
    }

    private sealed class ScorePayload
    {
        [JsonPropertyName("communication")]
        public required double Communication { get; init; }

        [JsonPropertyName("problemSolving")]
        public required double ProblemSolving { get; init; }

        [JsonPropertyName("languageUse")]
        public required double LanguageUse { get; init; }

        [JsonPropertyName("professionalism")]
        public required double Professionalism { get; init; }
    }

    private sealed class FeedbackPayload
    {
        [JsonPropertyName("strengths")]
        public IReadOnlyCollection<string>? Strengths { get; init; }

        [JsonPropertyName("improvements")]
        public IReadOnlyCollection<string>? Improvements { get; init; }
    }
}
