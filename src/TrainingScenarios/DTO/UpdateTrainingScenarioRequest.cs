using System.ComponentModel.DataAnnotations;

namespace AIInstructor.src.TrainingScenarios.DTO
{
    public class UpdateTrainingScenarioRequest
    {
        [Required]
        [MaxLength(200)]
        public string Title { get; set; } = string.Empty;

        [Required]
        [MaxLength(2000)]
        public string Description { get; set; } = string.Empty;

        [MaxLength(4000)]
        public string? CustomerProfile { get; set; }

        [MaxLength(2000)]
        public string? LearningObjectives { get; set; }

        [MaxLength(2000)]
        public string? SuccessCriteria { get; set; }

        [MaxLength(1000)]
        public string? Tags { get; set; }

        [MaxLength(50)]
        public string Language { get; set; } = "tr-TR";

        [Range(1, 20)]
        public int InteractionRounds { get; set; } = 4;

        [MaxLength(4000)]
        public string? ScenarioData { get; set; }
    }
}
