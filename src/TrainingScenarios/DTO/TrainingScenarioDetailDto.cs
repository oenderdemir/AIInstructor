namespace AIInstructor.src.TrainingScenarios.DTO
{
    public class TrainingScenarioDetailDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string? CustomerProfile { get; set; }
        public string? LearningObjectives { get; set; }
        public string? SuccessCriteria { get; set; }
        public string? Tags { get; set; }
        public string Language { get; set; } = string.Empty;
        public int InteractionRounds { get; set; }
        public string? ScenarioData { get; set; }
    }
}
