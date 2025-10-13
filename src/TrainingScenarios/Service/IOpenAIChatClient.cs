using System.Collections.Generic;

namespace AIInstructor.src.TrainingScenarios.Service
{
    public interface IOpenAIChatClient
    {
        Task<OpenAIChatCompletionResult> GetChatCompletionAsync(OpenAIChatCompletionRequest request, CancellationToken cancellationToken = default);
    }

    public class OpenAIChatCompletionRequest
    {
        public IList<OpenAIChatMessage> Messages { get; set; } = new List<OpenAIChatMessage>();
        public string? Model { get; set; }
        public double? Temperature { get; set; }
        public object? ResponseFormat { get; set; }
    }

    public record OpenAIChatMessage(string Role, string Content);

    public class OpenAIChatCompletionResult
    {
        public string Content { get; set; } = string.Empty;
    }
}
