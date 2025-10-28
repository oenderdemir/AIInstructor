using System;
using System.Collections.Generic;

namespace AIInstructor.src.AIInstructor.DTO
{
    public class AIInstructorEvaluateRequest
    {
        public Guid OgrenciSenaryoId { get; set; }
        public List<string> Messages { get; set; } = new();
    }
}
