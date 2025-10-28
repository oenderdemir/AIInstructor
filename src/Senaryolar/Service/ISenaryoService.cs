using System;
using System.Collections.Generic;
using AIInstructor.src.Senaryolar.DTO;

namespace AIInstructor.src.Senaryolar.Service
{
    public interface ISenaryoService
    {
        Task<IEnumerable<SenaryoDto>> GetAllAsync();
        Task<IEnumerable<SenaryoDto>> GetByOwnerAsync(Guid ownerId);
        Task<SenaryoDto> CreateAsync(CreateSenaryoDto dto);
        Task<SenaryoDto> UpdateAsync(Guid id, CreateSenaryoDto dto);
        Task DeleteAsync(Guid id);
        Task<SenaryoDto?> GetByIdAsync(Guid id);
    }
}
