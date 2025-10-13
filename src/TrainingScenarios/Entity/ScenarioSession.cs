using System.ComponentModel.DataAnnotations;
using AIInstructor.src.Shared.RDBMS.Entity;

namespace AIInstructor.src.TrainingScenarios.Entity
{
    public class ScenarioSession : BaseEntity
    {
        [Required]
        public Guid ScenarioId { get; set; }

        public TrainingScenario? Scenario { get; set; }

        [Required]
        public Guid StudentId { get; set; }

        [MaxLength(50)]
        public string Language { get; set; } = "tr-TR";

        public int CurrentTurn { get; set; }

        public int MaxTurns { get; set; }

        public bool IsCompleted { get; set; }

        public DateTime? CompletedAt { get; set; }

        [MaxLength(4000)]
        public string? SystemPrompt { get; set; }

        [MaxLength(4000)]
        public string? SessionState { get; set; }

        public ICollection<ScenarioMessage> Messages { get; set; } = new List<ScenarioMessage>();

        public ScenarioEvaluation? Evaluation { get; set; }
    }
}
