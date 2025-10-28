using System;
using System.Collections.Generic;
using AIInstructor.src.OgrenciSenaryo.DTO;

namespace AIInstructor.src.OgrenciSenaryo.Service
{
    public interface IOgrenciSenaryoService
    {
        Task<IEnumerable<OgrenciSenaryoDto>> GetByOgrenciIdAsync(Guid ogrenciId);
        Task<OgrenciSenaryoDto> AssignAsync(OgrenciSenaryoAssignDto dto);
        Task<OgrenciSenaryoDto> CompleteAsync(OgrenciSenaryoCompleteDto dto);
    }
}
