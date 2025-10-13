namespace AIInstructor.src.TrainingScenarios.DTO
{
    public class GamificationProfileDto
    {
        public Guid StudentId { get; set; }
        public int TotalPoints { get; set; }
        public int CurrentLevel { get; set; }
        public int TotalScenariosCompleted { get; set; }
        public int ExperiencePoints { get; set; }
        public DateTime? LastUpdatedAt { get; set; }
        public IReadOnlyCollection<EarnedBadgeDto> Badges { get; set; } = Array.Empty<EarnedBadgeDto>();
    }

    public class EarnedBadgeDto
    {
        public string BadgeKey { get; set; } = string.Empty;
        public string BadgeName { get; set; } = string.Empty;
        public string? Description { get; set; }
        public DateTime EarnedAt { get; set; }
    }
}
