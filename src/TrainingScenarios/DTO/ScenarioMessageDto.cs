using AIInstructor.src.TrainingScenarios.Entity;

namespace AIInstructor.src.TrainingScenarios.DTO
{
    public class ScenarioMessageDto
    {
        public Guid Id { get; set; }
        public ScenarioMessageSender Sender { get; set; }
        public string Content { get; set; } = string.Empty;
        public int Sequence { get; set; }
        public DateTime SentAt { get; set; }
    }
}
