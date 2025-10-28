using System;
using System.Collections.Generic;
using AIInstructor.src.AIKisiOzellikler.DTO;

namespace AIInstructor.src.AIKisiOzellikler.Service
{
    public interface IAIKisiOzellikService
    {
        Task<IEnumerable<AIKisiOzellikDto>> GetBySenaryoIdAsync(Guid senaryoId);
        Task<AIKisiOzellikDto> CreateAsync(AIKisiOzellikCreateDto dto);
        Task DeleteAsync(Guid id);
    }
}
