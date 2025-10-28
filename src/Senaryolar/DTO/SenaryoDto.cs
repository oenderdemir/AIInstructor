using System;
using System.Collections.Generic;
using AIInstructor.src.AIKisiOzellikler.DTO;
using AIInstructor.src.SenaryoAdimlar.DTO;
using AIInstructor.src.Shared.RDBMS.Dto;

namespace AIInstructor.src.Senaryolar.DTO
{
    public class SenaryoDto : BaseRDBMSDto
    {
        public string Ad { get; set; } = string.Empty;
        public string Aciklama { get; set; } = string.Empty;
        public Guid OlusturanKullaniciId { get; set; }
        public List<AIKisiOzellikDto> Ozellikler { get; set; } = new();
        public List<SenaryoAdimDto> Adimlar { get; set; } = new();
    }
}
