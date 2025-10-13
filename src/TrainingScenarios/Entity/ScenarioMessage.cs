using System.ComponentModel.DataAnnotations;
using AIInstructor.src.Shared.RDBMS.Entity;

namespace AIInstructor.src.TrainingScenarios.Entity
{
    public class ScenarioMessage : BaseEntity
    {
        [Required]
        public Guid SessionId { get; set; }

        public ScenarioSession? Session { get; set; }

        public ScenarioMessageSender Sender { get; set; }

        [Required]
        [MaxLength(4000)]
        public string Content { get; set; } = string.Empty;

        public int Sequence { get; set; }

        public DateTime SentAt { get; set; } = DateTime.UtcNow;
    }

    public enum ScenarioMessageSender
    {
        Tutor = 0,
        Customer = 1,
        Student = 2
    }
}
