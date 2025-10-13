using System.ComponentModel.DataAnnotations;

namespace AIInstructor.src.TrainingScenarios.DTO
{
    public class SubmitScenarioTurnRequest
    {
        [Required]
        public Guid StudentId { get; set; }

        [Required]
        [MaxLength(4000)]
        public string StudentMessage { get; set; } = string.Empty;

        public bool ForceComplete { get; set; }
    }
}
