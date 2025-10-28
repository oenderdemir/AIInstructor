using System;
using AIInstructor.src.Shared.RDBMS.Dto;

namespace AIInstructor.src.AIKisiOzellik.DTO
{
    public class AIKisiOzellikDto : BaseRDBMSDto
    {
        public Guid SenaryoId { get; set; }
        public string OzellikTipi { get; set; } = string.Empty;
        public string Deger { get; set; } = string.Empty;
    }
}
