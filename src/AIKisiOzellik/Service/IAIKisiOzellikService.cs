using System;
using System.Collections.Generic;
using AIInstructor.src.AIKisiOzellik.DTO;

namespace AIInstructor.src.AIKisiOzellik.Service
{
    public interface IAIKisiOzellikService
    {
        Task<IEnumerable<AIKisiOzellikDto>> GetBySenaryoIdAsync(Guid senaryoId);
        Task<AIKisiOzellikDto> CreateAsync(AIKisiOzellikCreateDto dto);
        Task DeleteAsync(Guid id);
    }
}
