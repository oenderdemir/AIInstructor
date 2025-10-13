namespace AIInstructor.src.TrainingScenarios.Services;

public sealed class OpenAIOptions
{
    public string ApiKey { get; set; } = string.Empty;

    public string BaseUrl { get; set; } = "https://api.openai.com/";

    public string DefaultModel { get; set; } = "gpt-4.1-mini";

    public double Temperature { get; set; } = 0.7;

    public int MaxOutputTokens { get; set; } = 512;
}
