using System;
using AIInstructor.src.Shared.RDBMS.Entity;
using AIInstructor.src.Senaryolar.Entity;

namespace AIInstructor.src.SenaryoAdimlar.Entity
{
    public class SenaryoAdim : BaseEntity
    {
        public Guid SenaryoId { get; set; }
        public Senaryo? Senaryo { get; set; }
        public int SiraNo { get; set; }
        public string SuccessCriteria { get; set; } = string.Empty;
        public string FailureCriteria { get; set; } = string.Empty;
    }
}
