using System;
using AIInstructor.src.Shared.RDBMS.Dto;

namespace AIInstructor.src.Gamification.DTO
{
    public class GamificationResultDto : BaseRDBMSDto
    {
        public Guid OgrenciSenaryoId { get; set; }
        public int Puan { get; set; }
        public string Badge { get; set; } = string.Empty;
        public string Detay { get; set; } = string.Empty;
    }
}
