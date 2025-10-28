using System;
using AIInstructor.src.Shared.RDBMS.Entity;
using AIInstructor.src.Senaryolar.Entity;

namespace AIInstructor.src.OgrenciSenaryolar.Entity
{
    public class OgrenciSenaryo : BaseEntity
    {
        public Guid OgrenciId { get; set; }
        public Guid SenaryoId { get; set; }
        public Senaryo? Senaryo { get; set; }
        public DateTime? BaslamaTarihi { get; set; }
        public DateTime? BitisTarihi { get; set; }
        public int? Puan { get; set; }
        public string? Badge { get; set; }
    }
}
