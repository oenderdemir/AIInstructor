using System;
using System.Collections.Generic;
using AIInstructor.src.Shared.RDBMS.Entity;

namespace AIInstructor.src.AIInstructor.Entity
{
    public class AIInstructorSession : BaseEntity
    {
        public Guid OgrenciSenaryoId { get; set; }
        public Guid SenaryoId { get; set; }
        public Guid OgrenciId { get; set; }
        public ICollection<AIInstructorStepFeedback> GeriBildirimler { get; set; } = new List<AIInstructorStepFeedback>();
        public string Durum { get; set; } = string.Empty;
    }

    public class AIInstructorStepFeedback : BaseEntity
    {
        public Guid AIInstructorSessionId { get; set; }
        public string Mesaj { get; set; } = string.Empty;
        public bool Success { get; set; }
    }
}
