using System;
using System.Collections.Generic;
using AIInstructor.src.OgrenciSenaryolar.DTO;

namespace AIInstructor.src.OgrenciSenaryolar.Service
{
    public interface IOgrenciSenaryoService
    {
        Task<IEnumerable<OgrenciSenaryoDto>> GetByOgrenciIdAsync(Guid ogrenciId);
        Task<OgrenciSenaryoDto> AssignAsync(OgrenciSenaryoAssignDto dto);
        Task<OgrenciSenaryoDto> CompleteAsync(OgrenciSenaryoCompleteDto dto);
    }
}
