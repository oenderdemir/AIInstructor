namespace AIInstructor.src.TrainingScenarios.DTO
{
    public class ScenarioEvaluationDto
    {
        public int CommunicationScore { get; set; }
        public string? CommunicationFeedback { get; set; }
        public int ProblemSolvingScore { get; set; }
        public string? ProblemSolvingFeedback { get; set; }
        public int LanguageScore { get; set; }
        public string? LanguageFeedback { get; set; }
        public int ProfessionalismScore { get; set; }
        public string? ProfessionalismFeedback { get; set; }
        public int TotalScore { get; set; }
        public string? OverallFeedback { get; set; }
        public string? Strengths { get; set; }
        public string? AreasForImprovement { get; set; }
        public string? SuggestedNextSteps { get; set; }
        public int PointsAwarded { get; set; }
    }
}
