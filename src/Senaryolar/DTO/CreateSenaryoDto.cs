using AIInstructor.src.AIKisiOzellikler.DTO;
using AIInstructor.src.SenaryoAdimlar.DTO;
using System;
using System.Collections.Generic;

namespace AIInstructor.src.Senaryolar.DTO
{
    public class CreateSenaryoDto
    {
        public string Ad { get; set; } = string.Empty;
        public string Aciklama { get; set; } = string.Empty;
        public Guid OlusturanKullaniciId { get; set; }
        public List<AIKisiOzellikCreateDto> Ozellikler { get; set; } = new();
        public List<SenaryoAdimCreateDto> Adimlar { get; set; } = new();
    }
}
