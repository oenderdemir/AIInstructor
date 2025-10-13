using System.ComponentModel.DataAnnotations;
using AIInstructor.src.Shared.RDBMS.Entity;

namespace AIInstructor.src.TrainingScenarios.Entity
{
    public class StudentGamificationProfile : BaseEntity
    {
        [Required]
        public Guid StudentId { get; set; }

        public int TotalPoints { get; set; }

        public int CurrentLevel { get; set; } = 1;

        public int TotalScenariosCompleted { get; set; }

        public int ExperiencePoints { get; set; }

        public DateTime? LastUpdatedAt { get; set; }

        public ICollection<EarnedBadge> Badges { get; set; } = new List<EarnedBadge>();
    }
}
