using System;

namespace AIInstructor.src.OgrenciSenaryo.DTO
{
    public class OgrenciSenaryoAssignDto
    {
        public Guid OgrenciId { get; set; }
        public Guid SenaryoId { get; set; }
        public DateTime? BaslamaTarihi { get; set; }
    }
}
