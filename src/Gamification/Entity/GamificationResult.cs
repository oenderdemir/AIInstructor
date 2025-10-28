using System;
using AIInstructor.src.Shared.RDBMS.Entity;

namespace AIInstructor.src.Gamification.Entity
{
    public class GamificationResult : BaseEntity
    {
        public Guid OgrenciSenaryoId { get; set; }
        public int Puan { get; set; }
        public string Badge { get; set; } = string.Empty;
        public string Detay { get; set; } = string.Empty;
    }
}
