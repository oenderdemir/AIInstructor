using System;

namespace AIInstructor.src.OgrenciSenaryo.DTO
{
    public class OgrenciSenaryoCompleteDto
    {
        public Guid OgrenciSenaryoId { get; set; }
        public int Puan { get; set; }
        public string Badge { get; set; } = string.Empty;
        public DateTime? BitisTarihi { get; set; }
    }
}
