using System.Collections.Generic;

namespace AIInstructor.src.TrainingScenarios.DTO
{
    public class ScenarioTurnResponseDto
    {
        public Guid SessionId { get; set; }
        public Guid ScenarioId { get; set; }
        public int CurrentTurn { get; set; }
        public int MaxTurns { get; set; }
        public ScenarioMessageDto? TutorMessage { get; set; }
        public TutorTurnDto? TutorTurn { get; set; }
        public IReadOnlyCollection<ScenarioMessageDto> Messages { get; set; } = Array.Empty<ScenarioMessageDto>();
        public bool IsCompleted { get; set; }
        public ScenarioEvaluationDto? Evaluation { get; set; }
        public GamificationProfileDto? Gamification { get; set; }
    }

    public class TutorTurnDto
    {
        public string CustomerDialogue { get; set; } = string.Empty;
        public string TutorGuidance { get; set; } = string.Empty;
        public string? CoachingTips { get; set; }
        public IReadOnlyCollection<string> VocabularySuggestions { get; set; } = Array.Empty<string>();
        public string? NextExpectedAction { get; set; }
    }
}
