using System;
using System.Collections.Generic;

namespace AIInstructor.src.Senaryolar.DTO
{
    public class CreateSenaryoDto
    {
        public string Ad { get; set; } = string.Empty;
        public string Aciklama { get; set; } = string.Empty;
        public Guid OlusturanKullaniciId { get; set; }
        public List<AIKisiOzellik.DTO.AIKisiOzellikCreateDto> Ozellikler { get; set; } = new();
        public List<SenaryoAdim.DTO.SenaryoAdimCreateDto> Adimlar { get; set; } = new();
    }
}
