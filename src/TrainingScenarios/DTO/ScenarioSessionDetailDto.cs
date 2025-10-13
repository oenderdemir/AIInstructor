namespace AIInstructor.src.TrainingScenarios.DTO
{
    public class ScenarioSessionDetailDto
    {
        public Guid Id { get; set; }
        public Guid ScenarioId { get; set; }
        public Guid StudentId { get; set; }
        public string Language { get; set; } = string.Empty;
        public int CurrentTurn { get; set; }
        public int MaxTurns { get; set; }
        public bool IsCompleted { get; set; }
        public DateTime? CompletedAt { get; set; }
        public IReadOnlyCollection<ScenarioMessageDto> Messages { get; set; } = Array.Empty<ScenarioMessageDto>();
        public ScenarioEvaluationDto? Evaluation { get; set; }
    }
}
