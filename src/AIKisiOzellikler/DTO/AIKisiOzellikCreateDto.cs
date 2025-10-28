using System;

namespace AIInstructor.src.AIKisiOzellikler.DTO
{
    public class AIKisiOzellikCreateDto
    {
        public Guid? Id { get; set; }
        public Guid SenaryoId { get; set; }
        public string OzellikTipi { get; set; } = string.Empty;
        public string Deger { get; set; } = string.Empty;
    }
}
