using System;
using AIInstructor.src.Shared.RDBMS.Dto;

namespace AIInstructor.src.SenaryoAdimlar.DTO
{
    public class SenaryoAdimDto : BaseRDBMSDto
    {
        public Guid SenaryoId { get; set; }
        public int SiraNo { get; set; }
        public string SuccessCriteria { get; set; } = string.Empty;
        public string FailureCriteria { get; set; } = string.Empty;
    }
}
