using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.Json;
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
            AdditionalUserInstruction = "Senaryoyu değerlendir ve sadece geçerli JSON döndür."
        }, cancellationToken);

        try
        {
            var parsed = ParseEvaluationResponse(response);

            return new EvaluationResult
            {
                Communication = parsed.Communication,
                ProblemSolving = parsed.ProblemSolving,
                LanguageUse = parsed.LanguageUse,
                Professionalism = parsed.Professionalism,
                OverallScore = parsed.OverallScore,
                Strengths = string.Join("\n", parsed.Strengths),
                ImprovementAreas = string.Join("\n", parsed.Improvements)
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

    private static ParsedEvaluation ParseEvaluationResponse(string response)
    {
        if (string.IsNullOrWhiteSpace(response))
        {
            return ParsedEvaluation.Empty;
        }

        var trimmed = response.Trim();

        if (TryParseJson(trimmed, out var root))
        {
            return ExtractFromJson(root, trimmed);
        }

        var jsonSlice = ExtractFirstJsonObject(trimmed);

        if (jsonSlice is not null && TryParseJson(jsonSlice, out var slicedRoot))
        {
            return ExtractFromJson(slicedRoot, trimmed);
        }

        return ParsedEvaluation.FromUnstructured(trimmed);
    }

    private static ParsedEvaluation ExtractFromJson(JsonElement root, string raw)
    {
        var overall = GetDouble(root, "overallScore", "overall_score", "overall", "totalScore");

        var scoresElement = GetProperty(root, "scores", "scoreBreakdown", "score", "scoresBreakdown");

        double? communication = null;
        double? problemSolving = null;
        double? languageUse = null;
        double? professionalism = null;

        if (scoresElement.HasValue)
        {
            var scoreValue = scoresElement.Value;

            if (scoreValue.ValueKind == JsonValueKind.Array && scoreValue.GetArrayLength() > 0)
            {
                scoreValue = scoreValue[0];
            }

            communication = GetDouble(scoreValue, "communication", "communicationScore", "communication_score");
            problemSolving = GetDouble(scoreValue, "problemSolving", "problem_solving", "problemSolvingScore");
            languageUse = GetDouble(scoreValue, "languageUse", "language_use", "language", "languageScore");
            professionalism = GetDouble(scoreValue, "professionalism", "professionalismScore", "professionalism_score", "serviceQuality");
        }

        communication ??= GetDouble(root, "communication", "communicationScore");
        problemSolving ??= GetDouble(root, "problemSolving", "problem_solving");
        languageUse ??= GetDouble(root, "languageUse", "language_use");
        professionalism ??= GetDouble(root, "professionalism", "professionalismScore");

        var feedbackElement = GetProperty(root, "feedback", "comments", "evaluation", "notes");

        var strengths = ExtractFeedback(feedbackElement, raw, positive: true);
        var improvements = ExtractFeedback(feedbackElement, raw, positive: false);

        return ParsedEvaluation.FromValues(overall, communication, problemSolving, languageUse, professionalism, strengths, improvements);
    }

    private static bool TryParseJson(string json, out JsonElement root)
    {
        try
        {
            using var document = JsonDocument.Parse(json, new JsonDocumentOptions
            {
                AllowTrailingCommas = true,
                CommentHandling = JsonCommentHandling.Skip
            });

            root = document.RootElement.Clone();
            return true;
        }
        catch (JsonException)
        {
            root = default;
            return false;
        }
    }

    private static string? ExtractFirstJsonObject(string text)
    {
        var firstBrace = text.IndexOf('{');
        var lastBrace = text.LastIndexOf('}');

        if (firstBrace < 0 || lastBrace <= firstBrace)
        {
            return null;
        }

        return text[firstBrace..(lastBrace + 1)];
    }

    private static JsonElement? GetProperty(JsonElement element, params string[] propertyNames)
    {
        if (element.ValueKind != JsonValueKind.Object)
        {
            return null;
        }

        foreach (var candidate in element.EnumerateObject())
        {
            foreach (var propertyName in propertyNames)
            {
                if (MatchesPropertyName(candidate.Name, propertyName))
                {
                    return candidate.Value;
                }
            }
        }

        return null;
    }

    private static bool MatchesPropertyName(string actual, string expected)
    {
        if (string.Equals(actual, expected, StringComparison.OrdinalIgnoreCase))
        {
            return true;
        }

        var normalizedActual = actual.Replace("_", string.Empty, StringComparison.Ordinal);
        var normalizedExpected = expected.Replace("_", string.Empty, StringComparison.Ordinal);

        return string.Equals(normalizedActual, normalizedExpected, StringComparison.OrdinalIgnoreCase);
    }

    private static double? GetDouble(JsonElement element, params string[] propertyNames)
    {
        if (element.ValueKind == JsonValueKind.Number)
        {
            return element.GetDouble();
        }

        if (element.ValueKind == JsonValueKind.String && double.TryParse(element.GetString(), NumberStyles.Float, CultureInfo.InvariantCulture, out var parsed))
        {
            return parsed;
        }

        foreach (var property in propertyNames)
        {
            var nested = GetProperty(element, property);

            if (nested.HasValue)
            {
                var nestedElement = nested.Value;

                if (nestedElement.ValueKind == JsonValueKind.Number)
                {
                    return nestedElement.GetDouble();
                }

                if (nestedElement.ValueKind == JsonValueKind.String && double.TryParse(nestedElement.GetString(), NumberStyles.Float, CultureInfo.InvariantCulture, out var nestedParsed))
                {
                    return nestedParsed;
                }
            }
        }

        return null;
    }

    private static IReadOnlyCollection<string> ExtractFeedback(JsonElement? feedbackElement, string raw, bool positive)
    {
        if (feedbackElement.HasValue)
        {
            var element = feedbackElement.Value;

            var property = positive
                ? GetProperty(element, "strengths", "positives", "positive", "strength")
                : GetProperty(element, "improvements", "areasForImprovement", "suggestions", "improvement", "negatives");

            if (property.HasValue)
            {
                return ExtractStringCollection(property.Value);
            }

            if (element.ValueKind == JsonValueKind.String)
            {
                return ExtractStringCollection(element);
            }
        }

        if (!positive)
        {
            return new[] { raw };
        }

        return Array.Empty<string>();
    }

    private static IReadOnlyCollection<string> ExtractStringCollection(JsonElement element)
    {
        switch (element.ValueKind)
        {
            case JsonValueKind.Array:
                return element.EnumerateArray()
                    .Select(item => item.ValueKind == JsonValueKind.String ? item.GetString() : item.ToString())
                    .Where(text => !string.IsNullOrWhiteSpace(text))
                    .Select(text => text!.Trim())
                    .ToArray();
            case JsonValueKind.String:
                var value = element.GetString();

                if (string.IsNullOrWhiteSpace(value))
                {
                    return Array.Empty<string>();
                }

                return value
                    .Split(new[] { '\n', '\r', ';' }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(part => part.Trim())
                    .Where(part => !string.IsNullOrWhiteSpace(part))
                    .ToArray();
            default:
                var fallback = element.ToString();

                return string.IsNullOrWhiteSpace(fallback)
                    ? Array.Empty<string>()
                    : new[] { fallback.Trim() };
        }
    }

    private readonly record struct ParsedEvaluation(
        double OverallScore,
        double Communication,
        double ProblemSolving,
        double LanguageUse,
        double Professionalism,
        IReadOnlyCollection<string> Strengths,
        IReadOnlyCollection<string> Improvements)
    {
        public static ParsedEvaluation Empty => new(0, 0, 0, 0, 0, Array.Empty<string>(), Array.Empty<string>());

        public static ParsedEvaluation FromValues(
            double? overall,
            double? communication,
            double? problemSolving,
            double? languageUse,
            double? professionalism,
            IReadOnlyCollection<string> strengths,
            IReadOnlyCollection<string> improvements)
        {
            var breakdown = new[] { communication, problemSolving, languageUse, professionalism }
                .Where(v => v.HasValue)
                .Select(v => v!.Value)
                .ToArray();

            var computedOverall = overall ?? (breakdown.Length > 0 ? breakdown.Average() : 0);

            return new ParsedEvaluation(
                computedOverall,
                communication ?? computedOverall,
                problemSolving ?? computedOverall,
                languageUse ?? computedOverall,
                professionalism ?? computedOverall,
                strengths,
                improvements);
        }

        public static ParsedEvaluation FromUnstructured(string raw)
        {
            return new ParsedEvaluation(0, 0, 0, 0, 0, Array.Empty<string>(), new[] { raw });
        }
    }
}
