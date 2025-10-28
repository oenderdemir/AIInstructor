using System;
using System.Collections.Generic;
using AIInstructor.src.SenaryoAdim.DTO;

namespace AIInstructor.src.SenaryoAdim.Service
{
    public interface ISenaryoAdimService
    {
        Task<IReadOnlyList<SenaryoAdimDto>> GetBySenaryoIdAsync(Guid senaryoId);
        Task<SenaryoAdimDto> CreateOrUpdateAsync(SenaryoAdimCreateDto dto);
        Task DeleteAsync(Guid id);
    }
}
