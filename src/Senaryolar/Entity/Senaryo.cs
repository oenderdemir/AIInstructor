using System;
using System.Collections.Generic;
using AIInstructor.src.AIKisiOzellikler.Entity;
using AIInstructor.src.Kullanicilar.Entity;
using AIInstructor.src.SenaryoAdimlar.Entity;
using AIInstructor.src.Shared.RDBMS.Entity;

namespace AIInstructor.src.Senaryolar.Entity
{
    public class Senaryo : BaseEntity
    {
        public string Ad { get; set; } = string.Empty;
        public string Aciklama { get; set; } = string.Empty;
        public Guid OlusturanKullaniciId { get; set; }
        public Kullanici? OlusturanKullanici { get; set; }
        public ICollection<AIKisiOzellik> Ozellikler { get; set; } = new List<AIKisiOzellik>();
        public ICollection<SenaryoAdim> Adimlar { get; set; } = new List<SenaryoAdim>();
    }
}
