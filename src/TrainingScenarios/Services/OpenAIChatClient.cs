using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using AIInstructor.src.TrainingScenarios.Entity;
using Microsoft.Extensions.Options;

namespace AIInstructor.src.TrainingScenarios.Services;

public interface IOpenAIChatClient
{
    Task<string> GetChatCompletionAsync(IEnumerable<ScenarioMessage> messages, ChatCompletionRequest request, CancellationToken cancellationToken = default);
}

public sealed class OpenAIChatClient : IOpenAIChatClient
{
    private readonly HttpClient _httpClient;
    private readonly OpenAIOptions _options;

    public OpenAIChatClient(HttpClient httpClient, IOptions<OpenAIOptions> options)
    {
        _httpClient = httpClient;
        _options = options.Value;
        if (!string.IsNullOrWhiteSpace(_options.BaseUrl))
        {
            _httpClient.BaseAddress = new Uri(_options.BaseUrl);
        }
    }

    public async Task<string> GetChatCompletionAsync(IEnumerable<ScenarioMessage> messages, ChatCompletionRequest request, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(_options.ApiKey))
        {
            throw new InvalidOperationException("OpenAI API key is not configured.");
        }

        using var httpRequest = new HttpRequestMessage(HttpMethod.Post, "v1/chat/completions");
        httpRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _options.ApiKey);

        var payload = new
        {
            model = request.Model ?? _options.DefaultModel,
            temperature = request.Temperature ?? _options.Temperature,
            max_tokens = request.MaxOutputTokens ?? _options.MaxOutputTokens,
            messages = BuildMessages(messages, request.AdditionalUserInstruction),
            response_format = request.ResponseFormat
        };

        httpRequest.Content = JsonContent.Create(payload, options: new JsonSerializerOptions(JsonSerializerDefaults.Web));

        using var response = await _httpClient.SendAsync(httpRequest, cancellationToken);
        response.EnsureSuccessStatusCode();
        using var document = await JsonDocument.ParseAsync(await response.Content.ReadAsStreamAsync(cancellationToken), cancellationToken: cancellationToken);
        var content = document.RootElement.GetProperty("choices")[0].GetProperty("message").GetProperty("content").GetString();
        return content ?? string.Empty;
    }

    private static IEnumerable<object> BuildMessages(IEnumerable<ScenarioMessage> messages, string? additionalUserInstruction)
    {
        foreach (var message in messages)
        {
            yield return new { role = message.Role, content = message.Content };
        }

        if (!string.IsNullOrWhiteSpace(additionalUserInstruction))
        {
            yield return new { role = "user", content = additionalUserInstruction };
        }
    }
}

public sealed class ChatCompletionRequest
{
    public string? Model { get; init; }

    public double? Temperature { get; init; }

    public int? MaxOutputTokens { get; init; }

    public object? ResponseFormat { get; init; }

    public string? AdditionalUserInstruction { get; init; }
}
