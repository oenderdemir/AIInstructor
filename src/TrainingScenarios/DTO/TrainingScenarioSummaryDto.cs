namespace AIInstructor.src.TrainingScenarios.DTO
{
    public class TrainingScenarioSummaryDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Language { get; set; } = string.Empty;
        public int InteractionRounds { get; set; }
        public string? Tags { get; set; }
    }
}
