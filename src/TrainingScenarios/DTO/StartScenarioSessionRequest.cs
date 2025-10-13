using System.ComponentModel.DataAnnotations;

namespace AIInstructor.src.TrainingScenarios.DTO
{
    public class StartScenarioSessionRequest
    {
        [Required]
        public Guid StudentId { get; set; }

        [MaxLength(50)]
        public string? Language { get; set; }

        public IDictionary<string, string>? CustomContext { get; set; }
    }
}
