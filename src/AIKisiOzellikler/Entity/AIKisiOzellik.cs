using System;
using AIInstructor.src.Shared.RDBMS.Entity;
using AIInstructor.src.Senaryolar.Entity;

namespace AIInstructor.src.AIKisiOzellikler.Entity
{
    public class AIKisiOzellik : BaseEntity
    {
        public Guid SenaryoId { get; set; }
        public Senaryo? Senaryo { get; set; }
        public string OzellikTipi { get; set; } = string.Empty;
        public string Deger { get; set; } = string.Empty;
    }
}
