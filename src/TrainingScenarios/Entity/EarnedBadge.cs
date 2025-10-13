using System.ComponentModel.DataAnnotations;
using AIInstructor.src.Shared.RDBMS.Entity;

namespace AIInstructor.src.TrainingScenarios.Entity
{
    public class EarnedBadge : BaseEntity
    {
        [Required]
        public Guid GamificationProfileId { get; set; }

        public StudentGamificationProfile? GamificationProfile { get; set; }

        [Required]
        [MaxLength(150)]
        public string BadgeKey { get; set; } = string.Empty;

        [Required]
        [MaxLength(150)]
        public string BadgeName { get; set; } = string.Empty;

        [MaxLength(400)]
        public string? Description { get; set; }

        public DateTime EarnedAt { get; set; } = DateTime.UtcNow;
    }
}
