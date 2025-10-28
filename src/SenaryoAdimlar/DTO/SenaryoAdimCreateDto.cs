using System;

namespace AIInstructor.src.SenaryoAdimlar.DTO
{
    public class SenaryoAdimCreateDto
    {
        public Guid? Id { get; set; }
        public Guid SenaryoId { get; set; }
        public int SiraNo { get; set; }
        public string SuccessCriteria { get; set; } = string.Empty;
        public string FailureCriteria { get; set; } = string.Empty;
    }
}
