using System;
using AIInstructor.src.Shared.RDBMS.Dto;

namespace AIInstructor.src.OgrenciSenaryo.DTO
{
    public class OgrenciSenaryoDto : BaseRDBMSDto
    {
        public Guid OgrenciId { get; set; }
        public Guid SenaryoId { get; set; }
        public DateTime? BaslamaTarihi { get; set; }
        public DateTime? BitisTarihi { get; set; }
        public int? Puan { get; set; }
        public string? Badge { get; set; }
    }
}
