using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace AIInstructor.src.TrainingScenarios.Service
{
    public class OpenAIOptions
    {
        public string ApiKey { get; set; } = string.Empty;
        public string BaseUrl { get; set; } = "https://api.openai.com/v1";
        public string ChatEndpoint { get; set; } = "/chat/completions";
        public string DefaultModel { get; set; } = "gpt-4.1-mini";
        public string? EvaluationModel { get; set; }
        public double Temperature { get; set; } = 0.7;
        public double EvaluationTemperature { get; set; } = 0.2;
    }

    public class OpenAIChatClient : IOpenAIChatClient
    {
        private readonly IHttpClientFactory httpClientFactory;
        private readonly IOptionsMonitor<OpenAIOptions> optionsMonitor;
        private readonly ILogger<OpenAIChatClient> logger;

        private static readonly JsonSerializerOptions serializerOptions = new()
        {
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        };

        public OpenAIChatClient(
            IHttpClientFactory httpClientFactory,
            IOptionsMonitor<OpenAIOptions> optionsMonitor,
            ILogger<OpenAIChatClient> logger)
        {
            this.httpClientFactory = httpClientFactory;
            this.optionsMonitor = optionsMonitor;
            this.logger = logger;
        }

        public async Task<OpenAIChatCompletionResult> GetChatCompletionAsync(OpenAIChatCompletionRequest request, CancellationToken cancellationToken = default)
        {
            var options = optionsMonitor.CurrentValue;

            if (string.IsNullOrWhiteSpace(options.ApiKey))
            {
                logger.LogWarning("OpenAI API anahtarı yapılandırılmamış. Varsayılan yanıt döndürülüyor.");
                return new OpenAIChatCompletionResult
                {
                    Content = "{\"message\":\"OpenAI API anahtarı tanımlanmadığı için varsayılan bir yanıt dönüldü.\"}"
                };
            }

            var client = httpClientFactory.CreateClient(nameof(OpenAIChatClient));

            if (client.BaseAddress == null)
            {
                client.BaseAddress = new Uri(options.BaseUrl);
            }

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", options.ApiKey);

            var body = new Dictionary<string, object?>
            {
                ["model"] = string.IsNullOrWhiteSpace(request.Model) ? options.DefaultModel : request.Model,
                ["messages"] = request.Messages.Select(message => new Dictionary<string, string>
                {
                    ["role"] = message.Role,
                    ["content"] = message.Content
                }).ToList(),
                ["temperature"] = request.Temperature ?? options.Temperature,
            };

            if (request.ResponseFormat != null)
            {
                body["response_format"] = request.ResponseFormat;
            }

            var payload = JsonSerializer.Serialize(body, serializerOptions);
            var content = new StringContent(payload, Encoding.UTF8, "application/json");

            var endpoint = options.ChatEndpoint.StartsWith("/") ? options.ChatEndpoint[1..] : options.ChatEndpoint;
            var httpRequest = new HttpRequestMessage(HttpMethod.Post, endpoint)
            {
                Content = content
            };

            var response = await client.SendAsync(httpRequest, cancellationToken);
            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync(cancellationToken);
                logger.LogError("OpenAI isteği başarısız oldu. StatusCode: {StatusCode}, Response: {Response}", response.StatusCode, error);
                throw new InvalidOperationException($"OpenAI isteği başarısız oldu: {response.StatusCode}");
            }

            using var stream = await response.Content.ReadAsStreamAsync(cancellationToken);
            using var document = await JsonDocument.ParseAsync(stream, cancellationToken: cancellationToken);

            var contentValue = document
                .RootElement
                .GetProperty("choices")[0]
                .GetProperty("message")
                .GetProperty("content")
                .GetString() ?? string.Empty;

            return new OpenAIChatCompletionResult
            {
                Content = contentValue
            };
        }
    }
}
