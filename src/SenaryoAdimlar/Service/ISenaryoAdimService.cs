using System;
using System.Collections.Generic;
using AIInstructor.src.SenaryoAdimlar.DTO;

namespace AIInstructor.src.SenaryoAdimlar.Service
{
    public interface ISenaryoAdimService
    {
        Task<IReadOnlyList<SenaryoAdimDto>> GetBySenaryoIdAsync(Guid senaryoId);
        Task<SenaryoAdimDto> CreateOrUpdateAsync(SenaryoAdimCreateDto dto);
        Task DeleteAsync(Guid id);
    }
}
