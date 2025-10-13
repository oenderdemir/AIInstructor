using System.ComponentModel.DataAnnotations;
using AIInstructor.src.Shared.RDBMS.Entity;

namespace AIInstructor.src.TrainingScenarios.Entity
{
    public class ScenarioEvaluation : BaseEntity
    {
        [Required]
        public Guid SessionId { get; set; }

        public ScenarioSession? Session { get; set; }

        [Range(0, 100)]
        public int CommunicationScore { get; set; }

        [MaxLength(1000)]
        public string? CommunicationFeedback { get; set; }

        [Range(0, 100)]
        public int ProblemSolvingScore { get; set; }

        [MaxLength(1000)]
        public string? ProblemSolvingFeedback { get; set; }

        [Range(0, 100)]
        public int LanguageScore { get; set; }

        [MaxLength(1000)]
        public string? LanguageFeedback { get; set; }

        [Range(0, 100)]
        public int ProfessionalismScore { get; set; }

        [MaxLength(1000)]
        public string? ProfessionalismFeedback { get; set; }

        [Range(0, 100)]
        public int TotalScore { get; set; }

        [MaxLength(2000)]
        public string? OverallFeedback { get; set; }

        [MaxLength(2000)]
        public string? Strengths { get; set; }

        [MaxLength(2000)]
        public string? AreasForImprovement { get; set; }

        [MaxLength(4000)]
        public string? SuggestedNextSteps { get; set; }

        [MaxLength(4000)]
        public string? RawEvaluationJson { get; set; }

        public int PointsAwarded { get; set; }
    }
}
