using System.Collections.Generic;

namespace AIInstructor.src.AIInstructor.DTO
{
    public class AIInstructorEvaluateResponse
    {
        public bool Success { get; set; }
        public List<string> Hints { get; set; } = new();
        public string? Badge { get; set; }
        public int? Puan { get; set; }
    }
}
